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
        public UnityEvent onDefenseHit;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (health && health.enabled)
            {
                health.ApplyDamage(Game.Instance.World.Player.activeGun.bulletDamage);
                onHit.Invoke();
            }
            else
            {
                onDefenseHit.Invoke();
            }
            Destroy(other.gameObject);
            if (flash) flash.Flash();
        }
    }
}