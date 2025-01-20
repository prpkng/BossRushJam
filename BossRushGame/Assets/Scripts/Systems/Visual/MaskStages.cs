using System;
using System.Collections.Generic;
using FMODUnity;
using Game.Systems.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Systems.Visual
{
    public class MaskStages : MonoBehaviour
    {
        public HealthBehavior health;
        public List<Sprite> stageSprites;
        public SpriteMask mask;
        public StudioEventEmitter soundEmitter;
        
        private int lastIndex = 0;
        private void Start()
        {
            health.OnHealthChanged += SetSprite;
        }

        
        public void SetSprite(float _)
        {
            int i = (int)Mathf.Lerp(stageSprites.Count, 0, health.HealthPercentage);
            if (lastIndex != i)
                soundEmitter.Play();
            lastIndex = i;
            i = Mathf.Min(i, stageSprites.Count - 1);
            mask.sprite = stageSprites[i];
        }
    }
}