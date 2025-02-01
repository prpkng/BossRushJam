using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using BRJ.Bosses.Joker;
using BRJ.Systems;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using PrimeTween;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityHFSM;

namespace BRJ.Bosses.Poker
{
    public class PokerBoss : MonoBehaviour
    {
        [System.Serializable]
        public struct PhaseData
        {
            public int addCardCount;
        }
        public enum States
        {
            RefillCards,
            ChooseCard,
            ChooseCardCooldown,
            StartState,
            PanicState,
            ShootKingCards,
            DeathState
        }
        public PhaseData phase1;
        public PhaseData phase2;
        public PhaseData phase3;
        public Transform bossSprite;
        public BossEyes bossEyes;
        public DeckHolder deck;
        public Transform deckOffset;
        public Vector3 defendingDeckPosition;
        public Vector3 openDeckPosition;

        public CardAttackManager attackManager;

        [SerializedDictionary("Card Type", "Sprite")]
        public SerializedDictionary<Card.Suits, Sprite> cardSprites = new()
        {
            {Card.Suits.Clubs, null},
            {Card.Suits.Spades, null},
            {Card.Suits.Hearts, null},
            {Card.Suits.Diamonds, null},
        };

        public BossHealth bossHealth;
        public float heartsHealthRecover = 20f;

        [Header("Pick Card")]
        public int startCardCount = 4;
        public TweenSettings<Vector3> pickCardTween;
        public TweenSettings<Vector3> pickCardRotationTween;
        public TweenSettings<Vector3> revealCardTween;
        public TweenSettings<Vector3> positionCardTween;
        public Transform pickedCardLocation;
        public Transform[] possibleCardLocations;

        public float pickCardCooldown = 4f;
        [Header("Panic")]
        public TweenSettings<Vector3> revealBossTween;
        public float panicWaitTime = 4f;
        [Header("Shoot King")]
        public TweenSettings shootCardTween;
        public GameObject kingCardAttackPrefab;
        public float kingAttackDelay = 1f;

        [SerializedDictionary("Card Type", "Sprite")]
        public List<Sprite> kingSprites = new() { };

        [Header("Death State")]

        public CinemachineCamera deathCamera;
        public CinemachineBasicMultiChannelPerlin deathCameraNoise;
        public float deathCameraNoiseIntensity = 2.5f;
        public float panWaitTime = 4f;
        public TweenSettings zoomInTween;
        public TweenSettings flashOutTween;
        public Vector3 jokerCardSpawnLocation;
        public GameObject jokerCardPrefab;
        public TweenSettings revealJokerTween;
        public float firstTwoRotation = 15f;
        public float lastRotation = 30f;

        [Space]
        public Transform spiralCenter;
        public float spiralRadius = 10f;
        public TweenSettings moveJokerTween;

        public float spiralAccelerationTime;

        public float spiralAccelerationSpeed;
        public float spiralDecelerationSpeed;

        public float slowSpiralSpeed = 10;

        [Header("Sounds")]
        public StudioEventEmitter cardShotEmitter;
        public EventReference throwKingCardSound;
        public EventReference jokerLaughSound;


        [Header("Editor")] public Card.Suits overrideCardType;



        private List<Vector3> usedCardLocations = new();
        private StateMachine<States> fsm;

        private List<Card.Suits> pickedCards = new();
        private List<GameObject> activeCards = new();

        private void Start()
        {
            fsm = new StateMachine<States>();

            // Start state
            fsm.AddState(States.StartState);
            fsm.AddTransition(new TransitionAfter<States>(States.StartState, States.RefillCards, 1f));

            // Refill Cards
            fsm.AddState(
                States.RefillCards,
                onEnter: async _ =>
                {
                    usedCardLocations.Clear();
                    deckOffset.localPosition = defendingDeckPosition;
                    for (int i = 0; i < startCardCount; i++)
                    {
                        deck.AddCard();
                        await UniTask.WaitForSeconds(0.25f);
                    }

                    _.fsm.StateCanExit();
                },
                needsExitTime: true
            );
            fsm.AddTransition(States.RefillCards, States.ChooseCard);

            // Choose card state

            fsm.AddState(States.ChooseCard, new CoState<States>(this, ChooseCardCoroutine, loop: false, needsExitTime: true));
            fsm.AddTransition(States.ChooseCard, States.ChooseCardCooldown);

            fsm.AddState(States.ChooseCardCooldown);
            fsm.AddTransition(States.ChooseCardCooldown, States.PanicState, _ => deck.cards.Count <= 0);
            fsm.AddTransition(new TransitionAfter<States>(States.ChooseCardCooldown, States.ChooseCard, pickCardCooldown));

            // Panic state
            // TODO: Boss new eyes
            fsm.AddState(States.PanicState, onEnter: _ =>
            {
                Tween.LocalPosition(bossSprite, revealBossTween);

                bossEyes.SetAngry();
                Game.Instance.Sound.BossMusic.With(b => b.SetAggressive());

            });

            fsm.AddTransition(new TransitionAfter<States>(States.PanicState, States.ShootKingCards, panicWaitTime));

            // Shoot king cards
            fsm.AddState(States.ShootKingCards, new CoState<States>(this, ShootKingCards, needsExitTime: true, loop: false));
            fsm.AddTransition(States.ShootKingCards, States.RefillCards);

            // Death state
            fsm.AddState(
                States.DeathState,
                new CoState<States>(this, DeathStateCoroutine, loop: false, needsExitTime: true)
            );

            fsm.AddTriggerTransitionFromAny("Death", States.DeathState, forceInstantly: true);

            fsm.SetStartState(States.StartState);

            fsm.Init();
        }

        public void ShakeDeathCamera()
        {
            Tween.Custom(
                deathCameraNoiseIntensity,
                0,
                .25f,
                f => deathCameraNoise.AmplitudeGain = f,
                Ease.OutSine
            );
        }
        public void BossDeath()
        {
            fsm.Trigger("Death");
        }

        #region < == BOSS PHASES 
        public void Phase1() => ApplyPhase(phase1);
        public void Phase2() => ApplyPhase(phase2);
        public void Phase3() => ApplyPhase(phase3);

        public void ApplyPhase(PhaseData phaseData)
        {
            startCardCount += phaseData.addCardCount;
        }

        #endregion
        public void DestroyAllCards()
        {
            activeCards.ForEach(c =>
            {
                if (c)
                    Destroy(c);
            });
            activeCards.Clear();
        }

        #region == STATE ==> Death
        private IEnumerator DeathStateCoroutine(CoState<States, string> state)
        {
            DestroyAllCards();

            bossEyes.SetAngry();
            Game.Instance.Sound.BossMusic.With(b => b.SetAggressive());

            deathCamera.Priority = 10;

            Tween.Custom(
                1.5f,
                2f,
                zoomInTween,
                f => Game.Instance.World.RenderTextureZoom = f
            );

            yield return new WaitForSeconds(panWaitTime);

            var mat = Game.Instance.World.RenderTextureMaterial;
            mat.SetFloat("_Force", 1);
            mat.SetInt("_Flash", 1);
            yield return null;
            bossSprite.gameObject.SetActive(false);

            var range = Enumerable.Range(0, 4);
            GameObject[] jokers = (from i in range
                                   select Instantiate(jokerCardPrefab, jokerCardSpawnLocation - Vector3.forward * i, Quaternion.identity))
                                  .ToArray();

            yield return new WaitForSeconds(.05f);
            ShakeDeathCamera();

            yield return Tween.Custom(
                1,
                0,
                flashOutTween,
                f => mat.SetFloat("_Force", f)
            ).ToYieldInstruction();

            ShakeDeathCamera();
            var current = jokers[2].transform;
            Tween.Rotation(
                current,
                current.transform.eulerAngles,
                new Vector3(0, 0, firstTwoRotation),
                revealJokerTween
            );
            yield return Tween.PositionX(
                current,
                current.position.x,
                current.position.x - .8f,
                revealJokerTween
            ).ToYieldInstruction();
            ShakeDeathCamera();

            current = jokers[1].transform;
            Tween.Rotation(
                current,
                current.transform.eulerAngles,
                new Vector3(0, 0, -firstTwoRotation),
                revealJokerTween
            );
            yield return Tween.PositionX(
                current,
                current.position.x,
                current.position.x + .8f,
                revealJokerTween
            ).ToYieldInstruction();
            ShakeDeathCamera();
            deathCamera.Priority = -10;
            Tween.Custom(
                2f,
                1.5f,
                zoomInTween,
                f => Game.Instance.World.RenderTextureZoom = f
            );

            current = jokers[0].transform;
            Tween.Rotation(
                current,
                current.transform.eulerAngles,
                new Vector3(0, 0, lastRotation),
                revealJokerTween
            );
            yield return Tween.PositionX(
                current,
                current.position.x,
                current.position.x - 1.6f,
                revealJokerTween
            ).ToYieldInstruction();

            yield return new WaitForSeconds(.5f);



            for (int i = 0; i < jokers.Length; i++)
            {
                float a = i * 90f;
                var dest = spiralCenter.position + (Vector3)Utilities.FromDegrees(a) * spiralRadius;

                Tween.Position(
                    jokers[i].transform,
                    dest,
                    moveJokerTween
                );

                Tween.Rotation(
                    jokers[i].transform,
                    Quaternion.identity,
                    moveJokerTween
                );
            }

            yield return new WaitForSeconds(moveJokerTween.duration);

            var jokerComponents = (from jk in jokers select jk.GetComponent<JokerCard>()).ToArray();
            int attempts = 0;
            while (true)
            {
                attempts++;
                float currentAngle = 0;
                float currentAngleSpeed = 0;
                float counter = 0;
                while (true)
                {
                    counter += Time.deltaTime;

                    if (counter < spiralAccelerationTime)
                        currentAngleSpeed += spiralAccelerationSpeed * Time.deltaTime;
                    else if (currentAngleSpeed > 0)
                        currentAngleSpeed -= spiralDecelerationSpeed * Time.deltaTime;
                    else
                        break;

                    currentAngle += currentAngleSpeed * Time.deltaTime;

                    for (int i = 0; i < jokers.Length; i++)
                        jokers[i].transform.position = spiralCenter.position +
                        (Vector3)Utilities.FromDegrees((currentAngle + i) * 90f) * spiralRadius;

                    yield return null;
                }

                print("Finished spin");

                int success = 0;

                var correct = jokerComponents.ChooseRandom();

                foreach (var jk in jokerComponents.Where(j => j != correct))
                    jk.OnHit += () => success = -1;

                correct.OnHit += () => success = 1;

                while (success == 0)
                {
                    currentAngle += slowSpiralSpeed * Time.deltaTime;

                    for (int i = 0; i < jokers.Length; i++)
                        jokers[i].transform.position = spiralCenter.position +
                        (Vector3)Utilities.FromDegrees((currentAngle + i) * 90f) * spiralRadius;

                    yield return null;
                }

                print($"Passed, success is {success}");

                foreach (var jk in jokerComponents)
                    jk.ClearCallbacks();

                mat.SetFloat("_Force", 1);
                mat.SetInt("_Flash", 1);

                Tween.Custom(
                    1,
                    0,
                    flashOutTween,
                    f => mat.SetFloat("_Force", f)
                );

                if (attempts >= 4 || success == 1)
                    break;
            }

            print("FINISHED");

            // Provisory
            Game.Instance.Transition.TransitionToScene("Lobby");

            yield break;
        }

        #endregion

        #region == STATE ==> ChooseCard
        private IEnumerator ChooseCardCoroutine(CoState<States, string> state)
        {
            yield return new WaitForSeconds(.5f);
            deckOffset.localPosition = defendingDeckPosition;
            Game.Instance.Sound.BossMusic.With(b => b.SetKidding());

            var card = deck.TakeCard();
            activeCards.Add(card.gameObject);
            if (pickedCards.Count >= 4) pickedCards.Clear();

            Card.Suits type;
            if (overrideCardType == Card.Suits.None)
            {
                var values = Enumerable.Range(0, Card.CardCount)
                    .Select(c => (Card.Suits)c)
                    .Except(pickedCards)
                    .ToArray();
                type = values.ChooseRandom();
            }
            else
                type = overrideCardType;

            pickedCards.Add(type);
            card.WithComponent<Card>(c => c.SetClass(type, cardSprites[type]));

            cardShotEmitter.Play();
            yield return Tween.Position(card, card.position + card.up * 2f, .35f, Ease.OutCubic).ToYieldInstruction();

            pickCardTween.endValue = pickedCardLocation.position;
            pickCardTween.startFromCurrent = true;
            pickCardRotationTween.startFromCurrent = true;

            var sequence = Sequence.Create(Tween.Position(card, pickCardTween));
            sequence.Group(Tween.Rotation(card, pickCardRotationTween));
            yield return sequence.ToYieldInstruction();

            yield return new WaitForSeconds(.25f);

            Tween.Delay(revealCardTween.settings.duration / 2f, () => cardShotEmitter.Play());
            yield return Tween.EulerAngles(card, revealCardTween).ToYieldInstruction();

            var position = possibleCardLocations
                .Select(p => p.position)
                .Except(usedCardLocations)
                .ToArray()
                .ChooseRandom();

            positionCardTween.endValue = position;
            positionCardTween.startFromCurrent = true;
            usedCardLocations.Add(position);

            cardShotEmitter.Play();

            yield return Tween.Position(card, positionCardTween).ToYieldInstruction();

            card.WithComponent((Card c) => c.Activate(this));

            state.fsm.StateCanExit();
        }

        #endregion

        #region == STATE ==> ShootKingCards
        private IEnumerator ShootKingCards(CoState<States, string> state)
        {

            deckOffset.localPosition = openDeckPosition;

            for (int i = 0; i < 5; i++)
            {
                deck.AddCard();
                yield return new WaitForSeconds(.5f);
            }

            yield return new WaitForSeconds(3);


            for (int i = 0; i < 5; i++)
            {
                var card = deck.TakeCard();
                activeCards.Add(card.gameObject);
                Destroy(card.gameObject, 10);
                card.WithComponent((Card c) =>
                {
                    c.frontSprite.sprite = kingSprites.ChooseRandom();
                    Destroy(c.collider);
                    Destroy(c.cardSine);
                    Destroy(c);
                });

                var destRot = new Vector3(0, 180, Random.Range(-165, 165));

                Vector2 dest = WorldManager.PlayerPosition;
                // if (Game.Instance.World.Player.IsMoving)
                //     dest += Game.Instance.World.Player.movementSpeed * shootCardTween.duration
                //              * InputManager.MoveVector.x * Vector2.right;

                dest += (Vector2)(Quaternion.Euler(destRot) * Vector3.down * 1.5f);

                RuntimeManager.PlayOneShot(throwKingCardSound, dest);

                Tween.Position(card, card.position, (Vector3)dest, shootCardTween);
                yield return Tween.Rotation(
                    card,
                    card.transform.eulerAngles,
                    destRot,
                    shootCardTween
                ).ToYieldInstruction();

                yield return new WaitForSeconds(kingAttackDelay);
                var cardAttack = Instantiate(kingCardAttackPrefab, card);
                activeCards.Add(cardAttack);
                yield return new WaitForSeconds(1.5f);
            }
            yield return null;

            Tween.LocalPosition(bossSprite, revealBossTween.WithDirection(false));
            bossEyes.SetNormal();

            state.fsm.StateCanExit();


        }

        #endregion

        private void FixedUpdate()
        {
            fsm.OnLogic();
        }
    }
}