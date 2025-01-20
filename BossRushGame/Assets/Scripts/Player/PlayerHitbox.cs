using System;
using BRJ.Systems;
using FMODUnity;
using PrimeTween;
using TMPro;
using UnityEngine;

namespace BRJ.Player
{
    public class PlayerHitbox : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D hitboxCollider;
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private TweenSettings<float> soundAttenuationTween;
        [SerializeField] private StudioEventEmitter hitSound;
        [Header("Shakes")]
        [SerializeField] private ShakeSettings weakHitShake;
        [SerializeField] private ShakeSettings strongHitShake;
        public void HitPlayer(int damage, Vector2? knockbackVector = null, bool strong = true)
        {
            playerHealth.ApplyDamage(damage);
            if (playerHealth.currentHealth <= 0) return;
            print($"kbv: {knockbackVector}");
            hitSound.Play();
            Game.Instance.World.Player.OnDamage(knockbackVector ?? Vector2.zero);
            CameraManager.Instance.ShakeCamera(strong ? strongHitShake : weakHitShake);

            Tween.Custom(soundAttenuationTween, f =>
            {
                RuntimeManager.StudioSystem.setParameterByName("DamageAttenuation", f);
            });
        }


        private Tween _invulnerableTween;
        public void SetInvulnerable(float duration)
        {
            if (_invulnerableTween.isAlive)
                duration += _invulnerableTween.duration - _invulnerableTween.elapsedTime;
            _invulnerableTween.Stop();

            hitboxCollider.enabled = false;
            _invulnerableTween = Tween.Delay(hitboxCollider, duration, col => 
                { 
                    if (col) col.enabled = true; 
                }
            );
        }
    }
}