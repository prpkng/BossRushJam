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
        [SerializeField] public FlashSprite flash;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            health.ApplyDamage(GameManager.Instance.Player.activeGun.bulletDamage);
            Destroy(other.gameObject);
            flash.Flash();
        }
    }
}