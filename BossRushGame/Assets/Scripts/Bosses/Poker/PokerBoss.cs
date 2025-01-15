using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Game.Systems;
using PrimeTween;
using UnityEngine;
using UnityHFSM;

namespace Game.Bosses.Poker
{
    public class PokerBoss : MonoBehaviour
    {
        public enum States { ChooseCard, ChooseCardCooldown }
        public DeckHolder deck;

        [SerializedDictionary("Card Type", "Sprite")]
        public SerializedDictionary<Card.Type, Sprite> cardSprites = new()
        {
            {Card.Type.ClubsAce, null},
            {Card.Type.SpadesAce, null},
            {Card.Type.HeartsAce, null},
            {Card.Type.DiamondsAce, null},
        };
        
        [Header("PickCard")]
        public TweenSettings<Vector3> pickCardTween;
        public TweenSettings<Vector3> pickCardRotationTween;
        public TweenSettings<Vector3> revealCardTween;
        public Transform pickedCardLocation;
        public Transform[] possibleCardLocations;

        public TweenSettings<Vector3> positionCardTween;
        public float pickCardCooldown = 4f;

        [Header("Editor")] public Card.Type overrideCardType;
        
        private List<Vector3> usedCardLocations = new();
        private StateMachine<States> fsm;
        
        private void Start()
        {
            fsm = new StateMachine<States>();
            fsm.AddState(States.ChooseCard, new CoState<States>(this, ChooseCardCoroutine, loop: false, needsExitTime: true));
            fsm.AddTransition(States.ChooseCard, States.ChooseCardCooldown);
            
            fsm.AddState(States.ChooseCardCooldown);
            fsm.AddTransition(new TransitionAfter<States>(States.ChooseCardCooldown, States.ChooseCard, pickCardCooldown));
            
            fsm.Init();
        }

        private IEnumerator ChooseCardCoroutine(CoState<States, string> state)
        {
            yield return new WaitForSeconds(.5f);
            var card = deck.TakeCard();

            Card.Type type;
            if (overrideCardType == Card.Type.None)
            {
                var values = System.Enum.GetValues(typeof(Card.Type));
                type = (Card.Type)values.GetValue(Random.Range(0, values.Length - 1));
            }
            else 
                type = overrideCardType;

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

            card.WithComponent((Card c) => c.Activate());
            
            state.fsm.StateCanExit();
        }

        private void FixedUpdate()
        {
            fsm.OnLogic();
        }
    }
}