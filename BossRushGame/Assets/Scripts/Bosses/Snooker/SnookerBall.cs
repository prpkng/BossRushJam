using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Bosses.Snooker
{
    public class SnookerBall : MonoBehaviour
    {
        [SerializeField] private float damageSpeedThreshold = 5f;
        [SerializeField] private int hazardousLayer;
        [SerializeField] private int safeLayer;
        [SerializeField] private SpriteRenderer ballSprite;
        
        private Rigidbody2D _rb;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            ballSprite.color = Random.ColorHSV(0, 1, 1, 1, 1, 1, 1, 1);
        }

        private void FixedUpdate()
        {
            transform.right = _rb.linearVelocity.normalized;

            gameObject.layer = _rb.linearVelocity.magnitude > damageSpeedThreshold ? hazardousLayer : safeLayer;
        }
    }
}