using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using BRJ.Systems;
using PrimeTween;
using UnityEngine;
using UnityHFSM;

namespace BRJ.Bosses.Poker
{
    public class PokerBoss : MonoBehaviour
    {
        public enum States
        {
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

        [Header("PickCard")]
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
        [Header("ShootKing")]
        public TweenSettings shootCardTween;

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
            fsm.AddTransition(new TransitionAfter<States>(States.StartState, States.ChooseCard, 1f));

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

            fsm.SetStartState(States.StartState);

            fsm.Init();
        }

        private IEnumerator ChooseCardCoroutine(CoState<States, string> state)
        {
            yield return new WaitForSeconds(.5f);
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

        private IEnumerator ShootKingCards()
        {
            // activeCards.ForEach(c => {
            //     if (c)
            //         Destroy(c.gameObject);
            // });

            deckOffset.localPosition = openDeckPosition;

            activeCards.Clear();
            deck.AddCard(5);

            yield return new WaitForSeconds(3);


            for (int i = 0; i < 5; i++)
            {
                var card = deck.TakeCard();
                card.WithComponent((Card c) => c.frontSprite.sprite = kingSprites.ChooseRandom());


                Vector2 dest = WorldManager.PlayerPosition;
                if (Game.Instance.World.Player.IsMoving)
                    dest += Game.Instance.World.Player.movementSpeed * shootCardTween.duration
                             * InputManager.MoveVector;


                Tween.Position(card, card.position, (Vector3)dest - card.up * 1.5f, shootCardTween);
                Tween.Rotation(
                    card,
                    card.transform.eulerAngles,
                    new Vector3(0, 180, Random.Range(-165, 165)),
                    shootCardTween
                );

                yield return new WaitForSeconds(3);
            }
            yield return null;
        }
        private void FixedUpdate()
        {
            fsm.OnLogic();
        }
    }
}