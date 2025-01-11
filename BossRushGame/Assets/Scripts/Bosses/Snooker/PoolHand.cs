using System;
using UnityEngine;

namespace Game.Bosses.Snooker
{
    public enum HandType
    {
        Idle,
        PoolHand,
        HoldingStick,
        HoldingStomp,
        HoldingBall,
    }

    [RequireComponent(typeof(SpriteRenderer))]
    public class PoolHand : MonoBehaviour
    {
        [SerializeField] private Sprite idleHand;
        [SerializeField] private Sprite poolHand;
        [SerializeField] private Sprite holdStickHand;
        [SerializeField] private Sprite holdStompHand;
        [SerializeField] private Sprite holdBallHand;

        private SpriteRenderer spr;
        private void Awake() => spr = gameObject.GetComponent<SpriteRenderer>();

        public void SetOrder(int order)
        {
            spr.sortingOrder = order;
        }
        public void SetHand(HandType handType)
        {
            spr.sprite = handType switch
            {
                HandType.Idle => idleHand,
                HandType.PoolHand => poolHand,
                HandType.HoldingStick => holdStickHand,
                HandType.HoldingStomp => holdStompHand,
                HandType.HoldingBall => holdBallHand,
                _ => null
            };
        }
    }
}