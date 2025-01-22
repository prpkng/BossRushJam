using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.Serialization;

namespace BRJ.Systems.Visual
{
    public class FlashMultipleSprites : FlashSprite
    {
        public SpriteRenderer[] sprites;

        public override void Flash()
        {
            if (sprites.Length <= 0) return;
            foreach (var spr in sprites)
                spr.material.SetInt(FlashID, 1);
            Tween.Delay(this, flashDuration, self =>
            {
                if (!self) return;
                foreach (var spr in sprites)
                    spr.material.SetInt(FlashID, 0);
            });
        }
    }
}