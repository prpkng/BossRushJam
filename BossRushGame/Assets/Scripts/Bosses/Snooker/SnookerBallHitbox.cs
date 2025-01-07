using Game.Systems;
using PrimeTween;
using UnityEngine;

namespace Game.Bosses.Snooker
{
    public class SnookerBallHitbox : MonoBehaviour
    {
        public Transform ballSprite;
        public ShakeSettings shotShakeSettings;
        public Rigidbody2D rb;
        public float knockbackForce = 2f;
        public Transform healthBarSprite;
        public HealthBehavior health;

        private Tween _shotShake;
        private void OnTriggerEnter2D(Collider2D other)
        {
            health.ApplyDamage(GameManager.Instance.Player.activeGun.bulletDamage);
            healthBarSprite.localScale = new Vector3(health.currentHealth / health.totalHealth, 1f);
            rb.AddForce(other.attachedRigidbody.linearVelocity.normalized * knockbackForce, ForceMode2D.Impulse);
            
            _shotShake.Complete();
            _shotShake = Tween.ShakeLocalPosition(ballSprite, shotShakeSettings);
            
            Destroy(other.gameObject);
        }
    }
}