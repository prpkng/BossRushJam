using BRJ.Systems.Common;
using BRJ.Systems.Visual;
using PrimeTween;

namespace BRJ.Bosses
{
    using BRJ.Systems;
    using UnityEngine;

    public class BossHitbox : MonoBehaviour
    {
        public HealthBehavior health;
        [SerializeField] private float flashDuration = .25f;
        public FlashSprite flash;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            health.ApplyDamage(Game.Instance.World.Player.activeGun.bulletDamage);
            Destroy(other.gameObject);
            flash.Flash();
        }
    }
}