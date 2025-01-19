using System;
using UnityEngine;

namespace Game.Systems.Common
{
    public class HealthBehavior : MonoBehaviour
    {
        public float totalHealth = 100;
        public float damageMultiplier = 1;
        public float currentHealth = 0;
        public bool destroyOnDeath = false;


        public event Action<float> OnHealthChanged;

        public float HealthPercentage => currentHealth / totalHealth;

        private void Start()
        {
            currentHealth = totalHealth;
        }

        protected virtual void OnDeath()
        {
            if (destroyOnDeath) Destroy(gameObject);
        }


        public virtual void AddHealth(float amount)
        {
            if (!enabled) return;
            if ((currentHealth + amount) > totalHealth)
            {
                currentHealth = totalHealth;
                OnHealthChanged?.Invoke(currentHealth + amount);
                return;
            }
            OnHealthChanged?.Invoke(currentHealth + amount);

            currentHealth += amount;
        }

        public virtual void ApplyDamage(float damage)
        {
            if (!enabled) return;
            OnHealthChanged?.Invoke(currentHealth - damage * damageMultiplier);

            currentHealth -= damage * damageMultiplier;
            if (currentHealth <= 0)
                OnDeath();
        }
    }
}