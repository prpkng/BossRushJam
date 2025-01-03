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

        [Header("References")]
        [System.NonSerialized] public Rigidbody2D rb;
        [System.NonSerialized] public StateMachine stateMachine;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            stateMachine = GetComponent<StateMachine>();
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
            print("Roll button pressed");
            stateMachine.SetState(new RollState());
        }

    }
}