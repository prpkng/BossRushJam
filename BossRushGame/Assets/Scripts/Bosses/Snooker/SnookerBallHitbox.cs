using Game.Systems;
using Game.Systems.Common;
using Game.Systems.Visual;
using PrimeTween;
using UnityEngine;

namespace Game.Bosses.Snooker
{
    public class SnookerBallHitbox : MonoBehaviour
    {
        public Transform ballSprite;
        public FlashSprite flash;
        public ShakeSettings shotShakeSettings;
        public Rigidbody2D rb;
        public float knockbackForce = 2f;
        public HealthBehavior health;

        private Tween _shotShake;
        private void OnTriggerEnter2D(Collider2D other)
        {
            health.ApplyDamage(GameManager.Instance.Player.activeGun.bulletDamage);
            rb.AddForceAtPosition(other.attachedRigidbody.linearVelocity.normalized * knockbackForce, other.transform.position, ForceMode2D.Impulse);
            
            _shotShake.Complete();
            _shotShake = Tween.ShakeLocalPosition(ballSprite, shotShakeSettings);
            flash.Flash();
            
            Destroy(other.gameObject);
        }
    }
}