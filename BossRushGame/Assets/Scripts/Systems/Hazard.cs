using System;
using Game.Player;
using UnityEngine;

namespace Game.Systems
{
    public class Hazard : MonoBehaviour
    {
        public float knockbackForce = 5;
        protected virtual Vector2 CalculateKnockback(PlayerHitbox player) => 
            (player.transform.position - transform.position).normalized;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out PlayerHitbox player)) return;

            var knockbackDir = CalculateKnockback(player);
            player.HitPlayer(1, knockbackDir * knockbackForce);
        }
    }
}