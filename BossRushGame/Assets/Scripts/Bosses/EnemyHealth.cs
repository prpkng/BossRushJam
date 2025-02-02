using BRJ.Systems.Common;
using UnityEngine.Events;

namespace BRJ.Bosses
{
    public class EnemyHealth : HealthBehavior
    {
        public UnityEvent onDeath;

        protected override void OnDeath()
        {
            onDeath.Invoke();
        }

    }
}