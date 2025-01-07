using System;
using System.Collections.Generic;
using System.Linq;
using Game.Systems;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Bosses.Snooker
{
    public class SnookerBall : MonoBehaviour
    {
        private static readonly List<Color> PickedColors = new();
        
        [SerializeField] private float damageSpeedThreshold = 5f;
        [SerializeField] private int hazardousLayer;
        [SerializeField] private int safeLayer;
        [SerializeField] private SpriteRenderer ballSprite;
        [SerializeField] private Color[] possibleColors;
        private Rigidbody2D _rb;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            ballSprite.color = possibleColors.Except(PickedColors).ToArray().ChooseRandom();
        }

        private void OnEnable()
        {
            PickedColors.Add(ballSprite.color);
        }

        private void OnDisable()
        {
            PickedColors.Remove(ballSprite.color);
        }

        private void FixedUpdate()
        {
            transform.right = _rb.linearVelocity.normalized;

            gameObject.layer = _rb.linearVelocity.magnitude > damageSpeedThreshold ? hazardousLayer : safeLayer;
        }
    }
}