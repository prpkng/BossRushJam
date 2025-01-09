using System.Collections.Generic;
using System.Linq;
using Game.Systems;
using UnityEditor.Animations;
using UnityEngine;

namespace Game.Bosses.Snooker
{
    public class SnookerBall : MonoBehaviour
    {
        private static readonly List<float> PickedHues = new();
        
        [SerializeField] private float damageSpeedThreshold = 5f;
        [SerializeField] private int hazardousLayer;
        [SerializeField] private int safeLayer;
        [SerializeField] private SpriteRenderer ballSprite;
        [SerializeField] private Animator ballAnimator;
        [SerializeField] private AnimatorController[] possibleBallAnimations;
        [SerializeField] private float[] possibleHues;
        [SerializeField] private float ballSpeedMulti = 0.25f;
        [SerializeField] private float ballSpeedPow = 1.1f;
        private Rigidbody2D _rb;

        private float currentHue;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            currentHue = possibleHues.Except(PickedHues).ToArray().ChooseRandom();
            ballSprite.material.SetFloat("_Shift", currentHue);
            ballAnimator.runtimeAnimatorController = possibleBallAnimations.ChooseRandom();
        }

        private void OnEnable()
        {
            PickedHues.Add(currentHue);
        }

        private void OnDisable()
        {
            PickedHues.Remove(currentHue);
        }

        private void FixedUpdate()
        {
            ballAnimator.speed = Mathf.Pow(_rb.linearVelocity.magnitude * ballSpeedMulti, ballSpeedPow);
            transform.right = _rb.linearVelocity.normalized;

            gameObject.layer = _rb.linearVelocity.magnitude > damageSpeedThreshold ? hazardousLayer : safeLayer;
        }
    }
}