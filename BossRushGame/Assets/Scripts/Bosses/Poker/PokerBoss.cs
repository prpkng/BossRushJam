using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using BRJ.Systems;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using UnityHFSM;

namespace BRJ.Bosses.Poker
{
    public class PokerBoss : MonoBehaviour
    {
        public enum States
        {
            RefillCards,
            ChooseCard,
            ChooseCardCooldown,
            StartState,
            PanicState,
            ShootKingCards
        }
        public Transform bossSprite;
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

        [Header("Editor")] public Card.Suits overrideCardType;



        private List<Vector3> usedCardLocations = new();
        private StateMachine<States> fsm;

        private List<Card.Suits> pickedCards = new();
        private List<Card> activeCards = new();


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
                Game.Instance.Sound.BossMusic.With(b => b.SetAggressive());

            });

            fsm.AddTransition(new TransitionAfter<States>(States.PanicState, States.ShootKingCards, panicWaitTime));

            // Shoot king cards
            fsm.AddState(States.ShootKingCards, new CoState<States>(this, ShootKingCards, needsExitTime: true, loop: false));
            fsm.AddTransition(States.ShootKingCards, States.RefillCards);

            fsm.SetStartState(States.StartState);

            fsm.Init();
        }

        private IEnumerator ChooseCardCoroutine(CoState<States, string> state)
        {
            yield return new WaitForSeconds(.5f);
            deckOffset.localPosition = defendingDeckPosition;

            var card = deck.TakeCard();
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

            yield return Tween.Position(card, card.position + card.up * 2f, .35f, Ease.OutCubic).ToYieldInstruction();

            pickCardTween.endValue = pickedCardLocation.position;
            pickCardTween.startFromCurrent = true;
            pickCardRotationTween.startFromCurrent = true;

            var sequence = Sequence.Create(Tween.Position(card, pickCardTween));
            sequence.Group(Tween.Rotation(card, pickCardRotationTween));
            yield return sequence.ToYieldInstruction();

            yield return new WaitForSeconds(.25f);

            yield return Tween.EulerAngles(card, revealCardTween).ToYieldInstruction();

            var position = possibleCardLocations
                .Select(p => p.position)
                .Except(usedCardLocations)
                .ToArray()
                .ChooseRandom();

            positionCardTween.endValue = position;
            positionCardTween.startFromCurrent = true;
            usedCardLocations.Add(position);

            yield return Tween.Position(card, positionCardTween).ToYieldInstruction();

            card.WithComponent((Card c) => c.Activate(this));
            activeCards.Add(card.GetComponent<Card>());

            state.fsm.StateCanExit();
        }

        private IEnumerator ShootKingCards(CoState<States, string> state)
        {
            // activeCards.ForEach(c => {
            //     if (c)
            //         Destroy(c.gameObject);
            // });

            deckOffset.localPosition = openDeckPosition;

            activeCards.Clear();

            for (int i = 0; i < 5; i++)
            {
                deck.AddCard();
                yield return new WaitForSeconds(.5f);
            }

            yield return new WaitForSeconds(3);


            for (int i = 0; i < 5; i++)
            {
                var card = deck.TakeCard();
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


                Tween.Position(card, card.position, (Vector3)dest, shootCardTween);
                yield return Tween.Rotation(
                    card,
                    card.transform.eulerAngles,
                    destRot,
                    shootCardTween
                ).ToYieldInstruction();

                yield return new WaitForSeconds(kingAttackDelay);
                Instantiate(kingCardAttackPrefab, card).transform.localPosition = Vector3.up * 1.5f;
                yield return new WaitForSeconds(1.5f);
            }
            yield return null;

            state.fsm.StateCanExit();


        }
        private void FixedUpdate()
        {
            fsm.OnLogic();
        }
    }
}