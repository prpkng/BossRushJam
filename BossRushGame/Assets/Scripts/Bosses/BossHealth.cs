using BRJ.Systems.Common;
using UnityEngine.Events;

namespace BRJ.Bosses
{
    using BRJ.Systems;
    using UnityEngine;

    public class BossHealth : HealthBehavior
    {
        
        public UnityEvent onThreeQuartersHealth;
        public UnityEvent onHalfHealth;
        public UnityEvent onQuarterHealth;
        
        public float Defense
        {
            set => damageMultiplier = 1f / value;
        }
        
        public override void ApplyDamage(float damage)
        {
            var healthBefore = currentHealth;
            base.ApplyDamage(damage);

            Game.Instance.World.BossBarController.With(b =>
            {
                b.SetHealthPercentage(currentHealth / totalHealth * 100f);
            });
            
            if (healthBefore > totalHealth * 0.75f && currentHealth <= totalHealth * 0.75f) onThreeQuartersHealth.Invoke();
            if (healthBefore > totalHealth * 0.5f && currentHealth <= totalHealth * 0.5f) onHalfHealth.Invoke();
            if (healthBefore > totalHealth * 0.25f && currentHealth <= totalHealth * 0.25f) onQuarterHealth.Invoke();
        }
    }
}