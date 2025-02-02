namespace BRJ.Bosses.Snooker
{
    using System.Collections;
    using PrimeTween;
    using UnityEngine;

    public class HandsIdleSine : MonoBehaviour
    {

        public Transform rightHandTransform;
        public Transform leftHandTransform;


        [Space]
        public float sineXMagnitude = 1f;
        public float sineYMagnitude = 2f;
        public float sineXSpeed = 2f;
        public float sineYSpeed = 1f;
        public float lerpSpeed = 10f;
        public bool playOnAwake = false;

        private float counter;

        private bool isRunning = false;

        private void Awake()
        {
            if (playOnAwake) StartMovement();
        }

        public void StartMovement()
        {
            isRunning = true;
        }

        public void StopMovement()
        {
            isRunning = false;
            counter = 0f;
        }

        private void Update()
        {
            if (!isRunning) return;

            counter += Time.deltaTime;

            var localX = Mathf.Sin(counter * sineXSpeed) * sineXMagnitude;
            var localY = Mathf.Sin(counter * sineYSpeed) * sineYMagnitude;

            leftHandTransform.localPosition = Vector3.Lerp(
                leftHandTransform.localPosition,
                new Vector3(localX + 2, localY),
                Time.deltaTime * lerpSpeed
            );
            rightHandTransform.localPosition = Vector3.Lerp(
                rightHandTransform.localPosition,
                new Vector3(-localX - 2, localY),
                Time.deltaTime * lerpSpeed
            );
        }

    }
}