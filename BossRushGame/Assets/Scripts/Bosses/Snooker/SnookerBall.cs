using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using BRJ.Systems;
using BRJ.Systems.Common;
using UnityEngine;
using System;

namespace BRJ.Bosses.Snooker
{
    public class SnookerBall : MonoBehaviour
    {
        private static readonly List<float> PickedHuesFlat = new();
        private static readonly List<float> PickedHuesLine = new();

        [SerializeField] private bool faceDirection = true;
        [SerializeField] private float[] possibleHues;
        [SerializeField] private float impactForceThreshold = 3f;
        [SerializeField] private SpriteRenderer ballSprite;
        [SerializeField] private Transform ballShadow;
        [SerializeField] private EventReference hitBallSound;
        [SerializeField] private EventReference hitWallSound;
        [SerializeField] private HealthBehavior ballHealth;
        [Header("Ball Animation")]
        [SerializeField] private float ballSpeedMulti = 0.25f;
        [SerializeField] private float ballSpeedPow = 1.1f;
        [SerializeField] private Animator ballAnimator;
        [SerializeField] private RuntimeAnimatorController[] possibleBallAnimations;

        private Rigidbody2D _rb;

        public float CurrentHue { get; private set; }
        private bool isFlat;

        public void DetachShadow()
        {
            ballShadow.parent = null;
        }
        public void AttachShadow()
        {
            ballShadow.parent = transform;
        }

        public void SetShadowLocalPos(Vector3 localPos)
        {
            ballShadow.localPosition = localPos;
        }

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
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("PocketTrigger"))
            {
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.relativeVelocity.magnitude <= impactForceThreshold) return;
            RuntimeManager.CreateInstance(
                other.gameObject.CompareTag("SnookerBall") ? hitBallSound : hitWallSound
            ).start();
            ballHealth.ApplyDamage(1);
        }
    }
}