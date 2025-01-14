using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Systems
{
    public class ParabolaMovement : MonoBehaviour
    {
        public float speed;
        public float arcHeight;

        private Vector3 target;
        private Vector3 startPosition;
        private float stepScale;
        private float progress;

        private void Awake()
        {
            enabled = false;
        }

        public async UniTask StartTrajectory(Vector3 target)
        {
            this.target = target;
            enabled = true;
            startPosition = transform.position;

            float distance = Vector3.Distance(startPosition, target);

            // This is one divided by the total flight duration, to help convert it to 0-1 progress.
            stepScale = speed / 60f;

            await UniTask.WaitUntil(this, self => !self.enabled);
        }

        private void FixedUpdate() {
            // Increment our progress from 0 at the start, to 1 when we arrive.
            progress = Mathf.Min(progress + Time.fixedDeltaTime * stepScale, 1.0f);

            // Turn this 0-1 value into a parabola that goes from 0 to 1, then back to 0.
            float parabola = 1.0f - 4.0f * (progress - 0.5f) * (progress - 0.5f);

            // Travel in a straight line from our start position to the target.        
            Vector3 nextPos = Vector3.Lerp(startPosition, target, progress);

            // Then add a vertical arc in excess of this.
            nextPos.y += parabola * arcHeight;

            // Continue as before.
            transform.right = (nextPos - transform.position).normalized;
            transform.position = nextPos;

            if (progress >= 1.0f)
                enabled = false;
        }
    }
}