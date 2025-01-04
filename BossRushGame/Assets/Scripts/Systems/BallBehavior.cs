namespace Game.Systems
{
    using UnityEngine;

    public class BallBehavior : MonoBehaviour
    {
        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            transform.right = rb.linearVelocity.normalized;
        }
    }
}