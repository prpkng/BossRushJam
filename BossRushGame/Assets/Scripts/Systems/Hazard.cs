using System;
using Game.Player;
using UnityEngine;

namespace Game.Systems
{
    public class Hazard : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerHitbox player))
            {
                player.HitPlayer(1);
            }
        }
    }
}