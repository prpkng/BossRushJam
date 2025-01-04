using Game.Player.States;
using UnityEngine;
using UnityHFSM;

namespace Game.Player
{
    public partial class PlayerManager : MonoBehaviour
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

        private StateMachine fsm;

        [System.NonSerialized] public bool canRoll = true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            GameManager.Instance.Player = this;
        }

        private void Start()
        {
            fsm = new StateMachine();
            fsm.AddState(
                "Idle",
                onLogic: state => rb.linearVelocity -= rb.linearVelocity * deceleration
            );
            fsm.AddState("Move", onLogic: state =>
            {
                var moveInput = InputManager.MoveVector;
                var targetSpeed = moveInput * movementSpeed;
                var speedDiff = targetSpeed - rb.linearVelocity;

                rb.linearVelocity += speedDiff * acceleration;
            });

            fsm.AddState("Roll", new RollState(this));
            fsm.AddTransition("Roll", "Move", _ => InputManager.MoveVector.sqrMagnitude > Mathf.Epsilon);
            fsm.AddTransition("Roll", "Idle", _ => InputManager.MoveVector.sqrMagnitude <= Mathf.Epsilon);

            fsm.AddTwoWayTransition("Idle", "Move", _ => InputManager.MoveVector.sqrMagnitude > Mathf.Epsilon);

            fsm.AddTriggerTransitionFromAny("Roll", new TransitionBase("", "Roll"));

            fsm.Init();
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

            // fsm.SetState(new RollState());
            fsm.Trigger("Roll");
            canRoll = false;
        }

        private void FixedUpdate()
        {
            fsm.OnLogic();
        }

    }
}