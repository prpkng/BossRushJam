using BRJ.Systems.Visual;
using UnityEngine;

namespace BRJ.Systems.Common
{
    public class EnemyHitbox : MonoBehaviour
    {
        public HealthBehavior health;
        public FlashSprite flash;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            health.ApplyDamage(Game.Instance.World.Player.activeGun.bulletDamage);
            Destroy(other.gameObject);
            if (flash) flash.Flash();
        }
    }
}