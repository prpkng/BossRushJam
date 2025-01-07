using Game.Systems;
using UnityEngine;

namespace Game.Bosses.Snooker
{
    public class SnookerBallHitbox : MonoBehaviour
    {
        public Transform healthBarSprite;
        public HealthBehavior health;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            health.ApplyDamage(GameManager.Instance.Player.activeGun.bulletDamage);
            healthBarSprite.localScale = new Vector3(health.currentHealth / health.totalHealth, 1f);
            
            Destroy(other.gameObject);
        }
    }
}