using System;
using System.Collections.Generic;
using FMODUnity;
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
        public StudioEventEmitter soundEmitter;
        
        private int lastIndex = 0;
        private void Start()
        {
            health.OnHealthChanged += SetSprite;
            renderer.material = ballSprite.material;
        }

        
        public void SetSprite(float _)
        {
            int i = (int)Mathf.Lerp(stageSprites.Count - 1, 0, health.HealthPercentage);
            if (lastIndex != i)
                soundEmitter.Play();
            lastIndex = i;
            renderer.sprite = stageSprites[i];
        }
    }
}