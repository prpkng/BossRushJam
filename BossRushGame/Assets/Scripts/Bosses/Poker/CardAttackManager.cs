namespace BRJ.Bosses.Poker
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;
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

        private async void Start()
        {
            if (!this) return;
            if (!gameObject) return;
            try
            {
                currentCards = currentCards.Where(c => (MonoBehaviour)c).ToList();

                await UniTask.WaitUntil(() => currentCards.Count > 0);
                await UniTask.WaitUntil(() => enabled);
                currentCards = currentCards.Where(c => (MonoBehaviour)c).ToList();
                var tokenSource = new CancellationTokenSource();
                for (int i = 0; i < currentCards.Count; i++)
                {
                    var target = (MonoBehaviour)currentCards[i];
                    var card = target.GetComponent<Card>();
                    if (!target)
                        continue;
                    var startPos = target.transform.position.y;
                    await Tween.PositionY(
                        target.transform,
                        startPos,
                        startPos + cardSelectOffset,
                        cardSelectTween
                    );

                    print($"Started attack on card: ${currentCards[i].GetType().Name}");
                    var attackDuration = currentCards[i].StartAttack();
                    card.exclamationMark.SetActive(true);

                    var time = Time.time;
                    while (Time.time < time + attackDuration)
                    {
                        if (!target)
                            break;
                        await UniTask.WaitForEndOfFrame();
                    }
                    if (!target)
                        continue;

                    print($"Stopped attack on card: ${currentCards[i].GetType().Name}");
                    currentCards[i].StopAttack();
                    card.exclamationMark.SetActive(false);

                    Tween.PositionY(
                        target.transform,
                        startPos + cardSelectOffset,
                        startPos,
                        cardSelectTween
                    ).ToUniTask().Forget();

                    await UniTask.WaitForSeconds(cardAttackDelay);

                }
            }
            finally
            {
                Start();
            }
        }
    }
}