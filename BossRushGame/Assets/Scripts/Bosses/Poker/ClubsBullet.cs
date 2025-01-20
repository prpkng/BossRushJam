using System;
using UnityEngine;

namespace Game.Bosses.Poker
{
    public class ClubsBullet : MonoBehaviour
    {
        public new Collider2D collider;
        public float moveSpeed = 30f;

        private void FixedUpdate()
        {
            transform.position += transform.right * (moveSpeed * Time.deltaTime);
        }

        private void OnEnable() {
            collider.enabled = true;
        }
    }
}