using FSM;
using Game.Player.States;
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
        public float rollDuration = .5f;
        public float rollSpeed = 12;
        public float rollCooldown = 0.5f;

        [Header("References")]
        public SpriteRenderer playerSprite;
        [System.NonSerialized] public Rigidbody2D rb;
        [System.NonSerialized] public StateMachine stateMachine;


        [System.NonSerialized] public bool canRoll = true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            stateMachine = GetComponent<StateMachine>();
            GameManager.Instance.Player = this;
        }

        private void OnEnable()
        {
            InputManager.RollPerformed += OnRollPerformed;
        }

        private void OnDisable()
        {
            InputManager.RollPerformed -= OnRollPerformed;
        }

        private void OnRollPerformed()
        {
            if (!canRoll)
                return;
            if (InputManager.MoveVector.sqrMagnitude <= Mathf.Epsilon)
                return;

            stateMachine.SetState(new RollState());
            canRoll = false;
        }

    }
}