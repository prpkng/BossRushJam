using System;
using System.Collections.Generic;
using FMODUnity;
using BRJ.Systems.Common;
using UnityEngine;
using UnityEngine.Serialization;
using FMOD.Studio;

namespace BRJ.Systems.Visual
{
    public class AnimatorStages : MonoBehaviour
    {
        public HealthBehavior health;
        public Animator animator;
        public EventReference stageSound;
        public int stageCount = 5;
        public string stateName;
        private int lastIndex = 0;
        private EventInstance? stageEvent;
        private void Start()
        {
            health.OnHealthChanged += SetSprite;
            if (stageSound.Path != "")
                stageEvent = RuntimeManager.CreateInstance(stageSound);
        }


        public void SetSprite(float _)
        {
            int i = Mathf.FloorToInt(health.HealthPercentage * stageCount);
            if (lastIndex != i)
            {
                stageEvent?.start();
                animator.Play(stateName, 0, 1f - health.HealthPercentage);
            }
            lastIndex = i;

        }
    }
}