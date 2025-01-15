using System;
using System.Collections.Generic;
using System.Linq;
using Game.Systems;
using UnityEngine;

namespace Game.Bosses.Snooker
{
    public class SnookerBall : MonoBehaviour
    {
        private static readonly List<float> PickedHuesFlat = new();
        private static readonly List<float> PickedHuesLine = new();
        
        [SerializeField] private bool faceDirection = false;
        [SerializeField] private float damageSpeedThreshold = 5f;
        [SerializeField] private int hazardousLayer;
        [SerializeField] private int safeLayer;
        [SerializeField] private SpriteRenderer ballSprite;
        [SerializeField] private Animator ballAnimator;
        [SerializeField] private RuntimeAnimatorController[] possibleBallAnimations;
        [SerializeField] private float[] possibleHues;
        [SerializeField] private float ballSpeedMulti = 0.25f;
        [SerializeField] private float ballSpeedPow = 1.1f;
        private Rigidbody2D _rb;

        public float CurrentHue { get; private set; }
        private bool isFlat;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            ballAnimator.runtimeAnimatorController = possibleBallAnimations.ChooseRandom();
            isFlat = Array.IndexOf(possibleBallAnimations, ballAnimator.runtimeAnimatorController) == 0;
            CurrentHue = possibleHues.Except(isFlat ? PickedHuesFlat : PickedHuesLine).ToArray().ChooseRandom();
            ballSprite.material.SetFloat("_Shift", CurrentHue);
            
        }

        private void OnEnable()
        {
            (isFlat ? PickedHuesFlat : PickedHuesLine).Add(CurrentHue);
        }

        private void OnDisable()
        {
            (isFlat ? PickedHuesFlat : PickedHuesLine).Remove(CurrentHue);
        }

        private void FixedUpdate()
        {
            ballAnimator.speed = Mathf.Pow(_rb.linearVelocity.magnitude * ballSpeedMulti, ballSpeedPow);
            if (faceDirection) transform.right = _rb.linearVelocity.normalized;

            gameObject.layer = _rb.linearVelocity.magnitude > damageSpeedThreshold ? hazardousLayer : safeLayer;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("PocketTrigger"))
            {
                Destroy(gameObject);
            }
        }
    }
}