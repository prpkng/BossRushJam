using System.Collections.Generic;
using System.Linq;
using BRJ.Systems;
using FMODUnity;
using PrimeTween;
using UnityEngine;

namespace BRJ.Bosses.Poker
{
    public class DeckHolder : MonoBehaviour
    {
        public List<Transform> cards;
        public Transform cardPrefab;

        public float defaultRange = 30f;
        public float cardsRangeSum = 5f;

        [Space] public float cardTweenDuration;
        public Ease cardTweenEase;
        [Space] public float addCardTweenDuration;
        public Ease addCardTweenEase;

        public EventReference addCardEvent;

        private void RecalculateCardsPosition()
        {
            float range = defaultRange + cardsRangeSum * cards.Count;
            for (int i = 0; i < cards.Count; i++)
            {
                float a = Mathf.Deg2Rad * (Mathf.Lerp(-range, range, (i + .5f) / cards.Count) + 90f);
                var dir = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0);
                Tween.LocalPosition(
                    cards[i],
                    dir * 4 + Vector3.down * 4f + Vector3.forward * i,
                    cardTweenDuration,
                    cardTweenEase
                );
                cards[i].localRotation = Quaternion.Euler(0, 0, a * Mathf.Rad2Deg - 90f);
            }
        }

        public void AddCard(int count = 1)
        {
            RuntimeManager.PlayOneShotAttached(addCardEvent, gameObject);
            for (int i = 0; i < count; i++)
            {
                var card = Instantiate(cardPrefab, transform);

                float range = defaultRange + cardsRangeSum * cards.Count;
                float a = Mathf.Deg2Rad * (-range + 90f);
                var dir = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0);
                card.localPosition = dir * 4 + Vector3.down * 4f;

                Tween.Scale(card, Vector3.zero, Vector3.one, addCardTweenDuration, addCardTweenEase);

                cards.Insert(0, card);
            }


            RecalculateCardsPosition();
        }

        public Transform TakeCard()
        {
            var card = cards.First();
            card.parent = null;
            Tween.CompleteAll(card.transform);
            card.transform.position -= Vector3.forward * 10f;
            card.WithComponent<Card>(c => c.enabled = true);
            cards.RemoveAt(0);
            RecalculateCardsPosition();
            return card;
        }

        public void RemoveCard()
        {
            var card = TakeCard();
            card.gameObject.SetActive(false);
        }
    }
}