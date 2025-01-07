using Game.Systems;
using UnityEngine;

namespace Game.Bosses.Snooker
{
    public class SnookerBallHealth : HealthBehavior
    {
        public override void OnDeath()
        {
            Destroy(gameObject);
            base.OnDeath();
        }
    }
}