using System;
using System.Collections.Generic;
using Game.Systems.Common;
using UnityEngine;

namespace Game.Systems.Visual
{
    public class HealthStages : MonoBehaviour
    {
        public SpriteRenderer ballSprite;
        public HealthBehavior health;
        public List<Sprite> stageSprites;
        public new SpriteRenderer renderer;
        private void Start()
        {
            health.OnHealthChanged += SetSprite;
            renderer.material = ballSprite.material;
        }

        public void SetSprite(float _)
        {
            int i = (int)Mathf.Lerp(stageSprites.Count - 1, 0, health.HealthPercentage);
            renderer.sprite = stageSprites[i];
        }
    }
}