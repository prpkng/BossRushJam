using System;
using UnityEngine;

namespace BRJ.Systems.Visual
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class MirrorSprite : MonoBehaviour
    {
        private new SpriteRenderer renderer;

        public SpriteRenderer mirrorSprite;

        private void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            renderer.sprite = mirrorSprite.sprite;
            renderer.flipX = mirrorSprite.flipX;
            renderer.flipY = mirrorSprite.flipY;
        }
    }
}