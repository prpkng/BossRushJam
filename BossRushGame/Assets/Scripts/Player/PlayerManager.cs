using UnityEngine;

namespace Game.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("Player Properties")]
        public float movementSpeed;

        [Range(0, 1)]
        public float acceleration;
        [Range(0, 1)]
        public float deceleration;
        [System.NonSerialized] public Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }
}