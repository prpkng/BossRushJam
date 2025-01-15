using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Systems.Visual
{
    public class FlashSprite : MonoBehaviour
    {
        private static readonly int FlashID = Shader.PropertyToID("_Flash");

        [FormerlySerializedAs("spr")] public SpriteRenderer spriteRenderer;
        [SerializeField] private float flashDuration = .05f;

        private void Awake() => spriteRenderer = GetComponent<SpriteRenderer>();

        public void Flash()
        {
            if (!spriteRenderer) return;
            spriteRenderer.material.SetInt(FlashID, 1);
            Tween.Delay(flashDuration, () => spriteRenderer.material.SetInt(FlashID, 0));
        }
    }
}