using System;
using UnityEngine;

namespace Game.Bosses.Snooker
{
    public enum HandType
    {
        Idle,
        PoolHand,
        HoldingStick
    }

    [RequireComponent(typeof(SpriteRenderer))]
    public class PoolHand : MonoBehaviour
    {
        [SerializeField] private Sprite idleHand;
        [SerializeField] private Sprite poolHand;
        [SerializeField] private Sprite holdStickHand;

        private SpriteRenderer _spr;
        private void Awake() => _spr = gameObject.GetComponent<SpriteRenderer>();

        public void SetHand(HandType handType)
        {
            _spr.sprite = handType switch
            {
                HandType.Idle => idleHand,
                HandType.PoolHand => poolHand,
                HandType.HoldingStick => holdStickHand,
                _ => null
            };
        }
    }
}