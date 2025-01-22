namespace BRJ.Bosses.Poker
{
    using System.Collections;
    using System.Collections.Generic;
    using PrimeTween;
    using UnityEngine;

    public class CardAttackManager : MonoBehaviour
    {
        public float cardAttackDelay = .5f;
        public TweenSettings cardSelectTween;
        public float cardSelectOffset = 2f;
        public List<ICardAttack> currentCards = new();

        public void AddCard(ICardAttack card)
        {
            currentCards.Add(card);
        }

        private IEnumerator Start()
        {

            yield return new WaitUntil(() => currentCards.Count > 0);
            for (int i = 0; i < currentCards.Count; i++)
            {
                var target = (MonoBehaviour) currentCards[i];
                var startPos = target.transform.position.y;
                yield return Tween.PositionY(
                    target.transform,
                    startPos,
                    startPos + cardSelectOffset,
                    cardSelectTween
                ).ToYieldInstruction();

                print($"Started attack on card: ${currentCards[i].GetType().Name}");
                var attackDuration = currentCards[i].StartAttack();
                yield return new WaitForSeconds(attackDuration);
                print($"Stopped attack on card: ${currentCards[i].GetType().Name}");
                currentCards[i].StopAttack();

                Tween.PositionY(
                    target.transform,
                    startPos + cardSelectOffset,
                    startPos,
                    cardSelectTween
                );

                yield return new WaitForSeconds(cardAttackDelay);

            }
            yield return StartCoroutine(Start());
        }
    }
}