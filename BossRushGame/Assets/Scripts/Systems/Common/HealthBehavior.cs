using System;
using UnityEngine;

namespace Game.Systems.Common
{
    public class HealthBehavior : MonoBehaviour
    {
        public float totalHealth = 100;
        public float damageMultiplier = 1;
        public float currentHealth = 0;

        public event Action<float> OnHealthChanged;
        
        public float HealthPercentage => currentHealth / totalHealth;
        
        private void Start()
        {
            currentHealth = totalHealth;
        }

        public virtual void OnDeath() { }

        public virtual void ApplyDamage(float damage)
        {
            if (!enabled) return;
            currentHealth -= damage * damageMultiplier;
            if (currentHealth <= 0)
                OnDeath();
            
            OnHealthChanged?.Invoke(currentHealth);
        }
    }
}