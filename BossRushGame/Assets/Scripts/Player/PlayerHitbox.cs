using System;
using PrimeTween;
using UnityEngine;

namespace Game.Player
{
    public class PlayerHitbox : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private ShakeSettings weakHitShake;
        [SerializeField] private ShakeSettings strongHitShake;
        public void HitPlayer(int damage, bool strong = true) 
        {
            playerHealth.ApplyDamage(damage);
            
            CameraManager.Instance.ShakeCamera(strong ? strongHitShake : weakHitShake);
        }
    }
}