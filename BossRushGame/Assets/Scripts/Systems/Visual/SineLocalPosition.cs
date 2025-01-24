namespace BRJ.Systems.Visual
{
    using UnityEngine;

    public class SineLocalPosition : MonoBehaviour
    {
        public float ySpeed;
        public float yMagnitude;
        public float xSpeed;
        public float xMagnitude;

        public float lerpSpeed = 10f;
        public bool startRunning = true;


        private Vector3 startPosition;
        private float counter;
        private bool isRunning = false;

        private void Start()
        {
            if (startRunning) StartMovement();
        }

        public void StartMovement()
        {
            isRunning = true;
            startPosition = transform.localPosition;
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

            var localX = Mathf.Sin(counter * xSpeed) * xMagnitude;
            var localY = Mathf.Sin(counter * ySpeed) * yMagnitude;

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                new Vector3(localX, localY) + startPosition,
                Time.deltaTime * lerpSpeed
            );
        }
    }
}