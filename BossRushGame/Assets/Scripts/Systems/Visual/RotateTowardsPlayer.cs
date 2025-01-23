namespace BRJ.Systems.Visual
{
    using UnityEngine;

    public class RotateTowardsPlayer : MonoBehaviour
    {
        public float lerpSpeed = 10f;
        [Range(0, 1)]
        public float rotateFactor = 0.5f;

        public float displacementX = 0f;
        public float displacementY = 0f;

        private void Update()
        {
            var player = Game.Instance.World.Player;
            if (player == null) return;

            Vector2 direction = player.transform.position - transform.position;
            direction.Normalize();
            var up = Vector3.Lerp(Vector3.up, -direction, rotateFactor);


            if (displacementX > 0 || displacementY > 0)
            {
                transform.localPosition = Vector3.Lerp(
                    transform.localPosition,
                    Vector3.Lerp(Vector3.zero, direction * new Vector2(displacementX, displacementY), rotateFactor),
                    Time.deltaTime * lerpSpeed
                );
            }
            transform.up = Vector3.Lerp(transform.up, up, Time.deltaTime * lerpSpeed);
        }
    }
}