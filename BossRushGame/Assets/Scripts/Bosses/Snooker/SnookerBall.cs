using System;
using UnityEngine;

namespace Game.Bosses.Snooker
{
    public class SnookerBall : MonoBehaviour
    {
        [SerializeField] private float damageSpeedThreshold = 5f;
        [SerializeField] private int hazardousLayer;
        [SerializeField] private int safeLayer;
        
        private Rigidbody2D _rb;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            transform.right = _rb.linearVelocity.normalized;

            gameObject.layer = _rb.linearVelocity.magnitude > damageSpeedThreshold ? hazardousLayer : safeLayer;
        }
    }
}