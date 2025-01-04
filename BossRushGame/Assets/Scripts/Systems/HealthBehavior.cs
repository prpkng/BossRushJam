namespace Game.Systems
{
    using UnityEngine;

    public class HealthBehavior : MonoBehaviour
    {
        public float totalHealth = 100;
        public float damageMultiplier = 1;
        public float currentHealth = 0;

        private void Start()
        {
            currentHealth = totalHealth;
        }

        public virtual void OnDeath() { }

        public virtual void ApplyDamage(float damage)
        {
            currentHealth -= damage * damageMultiplier;
            if (currentHealth <= 0)
                OnDeath();
        }
    }
}