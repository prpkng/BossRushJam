using System.Collections;
using AYellowpaper.SerializedCollections;
using Game.Systems;
using PrimeTween;
using UnityEngine;
using UnityHFSM;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Game.Bosses.Cards
{
    public class PokerBoss : MonoBehaviour
    {
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
        
        
        
        private StateMachine fsm;
        
        private void Start()
        {
            fsm = new StateMachine();
            fsm.AddState("ChooseCard", new CoState(this, ChooseCardCoroutine, loop: false));
            
            fsm.Init();
        }

        private IEnumerator ChooseCardCoroutine(CoState<string, string> arg)
        {
            var card = deck.TakeCard();
            yield return new WaitForSeconds(1f);

            var values = System.Enum.GetValues(typeof(Card.Type));
            var type = (Card.Type)values.GetValue(Random.Range(0, values.Length - 1));
            card.WithComponent<Card>(c => c.SetClass(type, cardSprites[type]));

            yield return Tween.Position(card, card.position + card.up * 2f, .35f, Ease.OutCubic).ToYieldInstruction();

            pickCardTween.endValue = pickedCardLocation.position;
            pickCardTween.startFromCurrent = true;
            pickCardRotationTween.startFromCurrent = true;

            var sequence = Sequence.Create(Tween.Position(card, pickCardTween));
            sequence.Group(Tween.Rotation(card, pickCardRotationTween));
            yield return sequence.ToYieldInstruction();

            yield return new WaitForSeconds(.25f);

            Tween.EulerAngles(card, revealCardTween);
        }

        private void FixedUpdate()
        {
            fsm.OnLogic();
        }
    }
}