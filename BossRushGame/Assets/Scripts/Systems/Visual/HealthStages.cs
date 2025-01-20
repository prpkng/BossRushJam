using System;
using System.Collections.Generic;
using FMODUnity;
using Game.Systems.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Systems.Visual
{
    public class HealthStages : MonoBehaviour
    {
        [FormerlySerializedAs("ballSprite")] public SpriteRenderer objectSprite;
        public HealthBehavior health;
        public List<Sprite> stageSprites;
        public new SpriteRenderer renderer;
        public StudioEventEmitter soundEmitter;
        
        private int lastIndex = 0;
        private void Start()
        {
            health.OnHealthChanged += SetSprite;
            renderer.material = objectSprite.material;
        }

        
        public void SetSprite(float _)
        {
            int i = (int)Mathf.Lerp(stageSprites.Count, 0, health.HealthPercentage);
            if (lastIndex != i)
                soundEmitter.Play();
            lastIndex = i;
            i = Mathf.Min(i, stageSprites.Count - 1);
            renderer.sprite = stageSprites[i];
        }
    }
}