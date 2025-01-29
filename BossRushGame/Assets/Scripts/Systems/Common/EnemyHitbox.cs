using BRJ.Systems.Visual;
using UnityEngine;
using UnityEngine.Events;

namespace BRJ.Systems.Common
{
    public class EnemyHitbox : MonoBehaviour
    {
        public HealthBehavior health;
        public FlashSprite flash;
        public UnityEvent onHit;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (health) health.ApplyDamage(Game.Instance.World.Player.activeGun.bulletDamage);
            Destroy(other.gameObject);
            onHit.Invoke();
            if (flash) flash.Flash();
        }
    }
}