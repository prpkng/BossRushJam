using BRJ.Systems;
using BRJ.Systems.Common;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

namespace BRJ.Bosses.Snooker
{
    public class SnookerBallHealth : HealthBehavior
    {
        public BallDeathParticles deathParticles;
        public EventReference deathEvent;

        protected override void OnDeath()
        {
            deathParticles.Play();
            RuntimeManager.CreateInstance(deathEvent).start();
            Destroy(gameObject);
            base.OnDeath();
        }
    }
}