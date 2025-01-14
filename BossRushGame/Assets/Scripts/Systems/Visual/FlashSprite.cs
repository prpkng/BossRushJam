using System;
using PrimeTween;
using UnityEngine;

namespace Game.Systems.Visual
{
    public class FlashSprite : MonoBehaviour
    {
        private static readonly int FlashID = Shader.PropertyToID("_Flash");

        private SpriteRenderer spr;
        [SerializeField] private float flashDuration = .05f;

        private void Awake() => spr = GetComponent<SpriteRenderer>();

        public void Flash()
        {
            spr.material.SetInt(FlashID, 1);
            Tween.Delay(flashDuration, () => spr.material.SetInt(FlashID, 0));
        }
    }
}