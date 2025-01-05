using System;
using UnityEngine;

namespace Game.Player
{
    public class PlayerHitbox : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;

        private void OnTriggerEnter2D(Collider2D other)
        {
            print($"Trigger entered: {other}");
            playerHealth.ApplyDamage(1);
        }
    }
}