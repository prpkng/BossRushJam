using System;
using UnityEngine;

namespace Game.Bosses.Poker
{
    public class DiamondBullet : MonoBehaviour
    {
        public SpriteRenderer renderer;
        public float bulletSpeed = 20f;
		public float aimSpeed = .75f;

        private void FixedUpdate()
        {
            transform.position += bulletSpeed * Time.fixedDeltaTime * transform.right;

            Vector2 dir = GameManager.PlayerPosition - transform.position;
            dir.Normalize();
            transform.right = Vector3.Lerp(
                transform.right,
                dir,
                aimSpeed * Time.fixedDeltaTime
            );
            
            if (!renderer.isVisible) Destroy(gameObject);
        }
    }
}