using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.Serialization;

namespace BRJ.Systems.Visual
{
    public class FlashSprite : MonoBehaviour
    {
        protected static readonly int FlashID = Shader.PropertyToID("_Flash");

        public SpriteRenderer SpriteRenderer { get; set; }
        [SerializeField] protected float flashDuration = .05f;

        private void Awake() => SpriteRenderer = GetComponent<SpriteRenderer>();

        public virtual void Flash()
        {
            if (!SpriteRenderer) return;
            SpriteRenderer.material.SetInt(FlashID, 1);
            Tween.Delay(this, flashDuration, self =>
            {
                if (!self) return;
                SpriteRenderer.material.SetInt(FlashID, 0);
            });
        }
    }
}