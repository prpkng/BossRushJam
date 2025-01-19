using Game.Systems;
using Game.Systems.Common;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Bosses.Snooker
{
    public class SnookerBallHealth : HealthBehavior
    {
        public BallDeathParticles deathParticles;

        protected override void OnDeath()
        {
            deathParticles.Play();
            Destroy(gameObject);
            base.OnDeath();
        }
    }
}