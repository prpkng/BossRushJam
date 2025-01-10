using Game.Systems;
using UnityEngine;
using UnityEngine.Events;

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