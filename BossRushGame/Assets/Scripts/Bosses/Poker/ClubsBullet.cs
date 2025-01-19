using System;
using UnityEngine;

namespace Game.Bosses.Poker
{
    public class ClubsBullet : MonoBehaviour
    {
        public float moveSpeed = 30f;

        private void FixedUpdate()
        {
            transform.position += transform.right * (moveSpeed * Time.deltaTime);
        }
    }
}