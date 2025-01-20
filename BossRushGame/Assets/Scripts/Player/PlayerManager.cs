using Game.Systems;
using UnityEngine;
using UnityHFSM;

namespace Game.Player
{
    public partial class PlayerManager : MonoBehaviour
    {
        [Header("Player Properties")] public float movementSpeed;

        [Range(0, 1)] public float acceleration;
        [Range(0, 1)] public float deceleration;

        public float rollDuration = .5f;
        public float rollInvulnerabilityDuration = .35f;
        public float rollSpeed = 12;
        public float rollCooldown = 0.5f;
        [Space] public float damageInvulnerabilityDuration = .4f;

        public float idleWaitTime = 1f;

        [Space] public float knockbackDuration = .25f;
        [Header("References")] public SpriteRenderer playerSprite;
        public Animator playerAnimations;
        public PlayerHitbox playerHitbox;
        public PlayerGun activeGun;


        private Vector2 currentKnockbackVector;
        private StateMachine fsm;

        [System.NonSerialized] public Rigidbody2D Rb;
        [System.NonSerialized] public bool CanRoll = true;
        [System.NonSerialized] public bool CanRollOverride = true;
        [System.NonSerialized] public float SpeedMultiplier = 1f;


        private void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            WorldManager.Instance.Player = this;
        }

        // TODO: FIX PLAYER INPUT NOT WORKING WHEN TRANSITIONING FROM LOBBY SCENE

        private void Start()
        {
            fsm = new StateMachine();

            fsm.AddState(
                "Idle",
                onEnter: _ => playerAnimations.Play("Idle"),
                onLogic: _ => Rb.linearVelocity -= Rb.linearVelocity * deceleration * SpeedMultiplier
            );
            fsm.AddState("Move",
                onEnter: _ => playerAnimations.Play("Run"),
                onLogic: state =>
                {
                    var moveInput = InputManager.MoveVector;
                    print("Move state running");
                    print($"Move input: {moveInput}");
                    var targetSpeed = moveInput * movementSpeed;
                    var speedDiff = targetSpeed - Rb.linearVelocity;

                    Rb.linearVelocity += speedDiff * acceleration * SpeedMultiplier;
                }
            );

            fsm.AddState("Roll", new RollState(this));
            fsm.AddTransition("Roll", "Move", _ => InputManager.MoveVector.sqrMagnitude > Mathf.Epsilon);
            fsm.AddTransition("Roll", "Idle", _ => InputManager.MoveVector.sqrMagnitude <= Mathf.Epsilon);

            fsm.AddState(
                "Hit",
                onLogic: _ => Rb.linearVelocity = currentKnockbackVector,
                canExit: state => state.timer.Elapsed > knockbackDuration,
                needsExitTime: true
            );
            fsm.AddTransition("Hit", "Idle", _ => InputManager.MoveVector.sqrMagnitude <= Mathf.Epsilon);
            fsm.AddTransition("Hit", "Move", _ => InputManager.MoveVector.sqrMagnitude > Mathf.Epsilon);

            fsm.AddTwoWayTransition("Idle", "Move", _ => InputManager.MoveVector.sqrMagnitude > Mathf.Epsilon);

            fsm.AddTriggerTransitionFromAny("Roll", new TransitionBase("", "Roll", forceInstantly: true));
            fsm.AddTriggerTransitionFromAny("Hit", new TransitionBase("", "Hit"));
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
            if (!CanRoll || !CanRollOverride)
                return;
            if (InputManager.MoveVector.sqrMagnitude <= Mathf.Epsilon)
                return;

            playerAnimations.Play("Roll");
            fsm.Trigger("Roll");
            CanRoll = false;
        }

        public void OnDamage(Vector2 knockback)
        {
            playerHitbox.SetInvulnerable(damageInvulnerabilityDuration);
            fsm.Trigger("Hit");
            currentKnockbackVector = knockback;
        }

        private void FixedUpdate()
        {
            fsm.OnLogic();
            print(InputManager.MoveVector);
        }
    }
}