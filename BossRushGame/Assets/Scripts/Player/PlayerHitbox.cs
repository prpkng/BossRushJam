using System;
using PrimeTween;
using TMPro;
using UnityEngine;

namespace Game.Player
{
    public class PlayerHitbox : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D hitboxCollider;
        [SerializeField] private PlayerHealth playerHealth;
        
        [Header("Shakes")]
        [SerializeField] private ShakeSettings weakHitShake;
        [SerializeField] private ShakeSettings strongHitShake;
        public void HitPlayer(int damage, Vector2? knockbackVector = null, bool strong = true) 
        {
            playerHealth.ApplyDamage(damage);
            GameManager.Instance.Player.OnDamage(knockbackVector ?? Vector2.zero);
            
            CameraManager.Instance.ShakeCamera(strong ? strongHitShake : weakHitShake);
        }


        private Tween _invulnerableTween;
        public void SetInvulnerable(float duration)
        {

            if (_invulnerableTween.isAlive)
                duration += _invulnerableTween.duration - _invulnerableTween.elapsedTime;
            _invulnerableTween.Stop();
            
            hitboxCollider.enabled = false;
            _invulnerableTween = Tween.Delay(duration, () => hitboxCollider.enabled = true);
        }
    }
}