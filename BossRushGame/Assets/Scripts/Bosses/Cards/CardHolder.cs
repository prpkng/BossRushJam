using System;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using UnityEngine;

namespace Game.Bosses.Cards
{
    public class CardHolder : MonoBehaviour
    {
        public List<Transform> cards;
        public Transform cardPrefab;
        
        public float defaultRange = 30f;
        public float cardsRangeSum = 5f;
        
        [Space]
        public float cardTweenDuration;
        public Ease cardTweenEase;
        [Space]
        public float addCardTweenDuration;
        public Ease addCardTweenEase;
        

        private void Start()
        {
            RecalculateCardsPosition();
        }

        private void RecalculateCardsPosition()
        {
            float range = defaultRange + cardsRangeSum * cards.Count;
            for (int i = 0; i < cards.Count; i++)
            {
                float a = Mathf.Deg2Rad * (Mathf.Lerp(-range, range, i / (cards.Count - 1f)) + 90f);
                var dir = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0);
                Tween.LocalPosition(
                    cards[i],
                    dir * 4 + Vector3.down * 4f + Vector3.forward * i,
                    cardTweenDuration,
                    cardTweenEase
                );
                cards[i].up = dir;
            }
        }

        public void AddCard()
        {
            var card = Instantiate(cardPrefab, transform);
            
            float range = defaultRange + cardsRangeSum * cards.Count;
            float a = Mathf.Deg2Rad * (-range + 90f);
            var dir = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0);
            card.localPosition = dir * 4 + Vector3.down * 4f;

            Tween.Scale(card, Vector3.zero, Vector3.one, addCardTweenDuration, addCardTweenEase);
            
            cards.Insert(0, card);
            RecalculateCardsPosition();
        }

        public Transform TakeCard()
        {
            var card = cards.First();
            cards.RemoveAt(0);
            return card;
        }
        public void RemoveCard()
        {
            var card = TakeCard();
            card.gameObject.SetActive(false);

            RecalculateCardsPosition();
        }
    }
}