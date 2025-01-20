using Game.Systems.Common;
using Game.Systems.Visual;
using PrimeTween;

namespace Game.Bosses
{
    using Game.Systems;
    using UnityEngine;

    public class BossHitbox : MonoBehaviour
    {
        public HealthBehavior health;
        [SerializeField] private float flashDuration = .25f;
        public FlashSprite flash;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            health.ApplyDamage(WorldManager.Instance.Player.activeGun.bulletDamage);
            Destroy(other.gameObject);
            flash.Flash();
        }
    }
}