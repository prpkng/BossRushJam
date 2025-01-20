using BRJ.Systems;
using BRJ.Systems.Common;
using UnityEngine;
using UnityEngine.Events;

namespace BRJ.Bosses.Snooker
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