using Game.Player.States;
using Game.Systems;
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
        public float rollInvulnerabilityDuration = .35f;
        public float rollSpeed = 12;
        public float rollCooldown = 0.5f;
        [Space]
        public float damageInvulnerabilityDuration = .4f;

        [Space] public float knockbackDuration = .25f;
        [Header("References")]
        public SpriteRenderer playerSprite;
        public Animator playerAnimations;
        public PlayerHitbox playerHitbox;
        public PlayerGun activeGun;


        private Vector2 _currentKnockbackVector;
        private StateMachine fsm;

        [System.NonSerialized] public Rigidbody2D rb;
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
                onEnter: _ => playerAnimations.Play("Idle"),
                onLogic: state => rb.linearVelocity -= rb.linearVelocity * deceleration
            );
            fsm.AddState("Move", 
                onEnter: _ => playerAnimations.Play("Run"), 
                onLogic: state =>
                {
                    var moveInput = InputManager.MoveVector;
                    var targetSpeed = moveInput * movementSpeed;
                    var speedDiff = targetSpeed - rb.linearVelocity;
    
                    rb.linearVelocity += speedDiff * acceleration;
                }
            );

            fsm.AddState("Roll", new RollState(this));
            fsm.AddTransition("Roll", "Move", _ => InputManager.MoveVector.sqrMagnitude > Mathf.Epsilon);
            fsm.AddTransition("Roll", "Idle", _ => InputManager.MoveVector.sqrMagnitude <= Mathf.Epsilon);

            fsm.AddState(
                "Hit",
                onLogic: _ => rb.linearVelocity = _currentKnockbackVector,
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
            if (!canRoll)
                return;
            if (InputManager.MoveVector.sqrMagnitude <= Mathf.Epsilon)
                return;

            // fsm.SetState(new RollState());
            playerAnimations.Play("Roll");
            fsm.Trigger("Roll");
            canRoll = false;
        }

        public void OnDamage(Vector2 knockback)
        {
            playerHitbox.SetInvulnerable(damageInvulnerabilityDuration);
            fsm.Trigger("Hit");
            _currentKnockbackVector = knockback;
        }

        private void FixedUpdate()
        {
            fsm.OnLogic();
        }

    }
}