using Game.Systems.Visual;
using UnityEngine;

namespace Game.Systems.Common
{
    public class EnemyHitbox : MonoBehaviour
    {
        public HealthBehavior health;
        public FlashSprite flash;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            health.ApplyDamage(WorldManager.Instance.Player.activeGun.bulletDamage);
            Destroy(other.gameObject);
            if (flash) flash.Flash();
        }
    }
}