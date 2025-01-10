using System.Diagnostics;
using System.Linq;
using Game.Systems;
using PrimeTween;
using UnityEngine.Serialization;

namespace Game.Bosses.Snooker
{
    using System.Collections;
    using Player;
    using UnityEngine;
    using UnityHFSM;

    public class SnookerBoss : MonoBehaviour
    {
        [Header("Common")] public float vulnerableDefense;
        public float attackingDefense;
        public int initialBallCount = 3;
        public Vector2 minBallPos;
        public Vector2 maxBallPos;
        
        [Header("Idle Cooldown")] public float idleWaitTime = 4;

        public TweenSettings<float> idleSineTweenY;
        public TweenSettings<float> rightIdleSineTweenX;
        public TweenSettings<float> leftIdleSineTweenX;

        [Header("Shot White Ball")] public float shotBallWaitTime = 5f;
        public float whiteBallStopThreshold = 1f;
        [Space] public float shotForce = 25;
        public float shotAnticipationSecs = 2f;
        public float currentBallStopLinearDrag = 1.75f;
        public float otherBallsStopLinearDrag = 0.1f;

        [Header("Freak Out")] public float freakOutWaitTime = 5f;

        [Header("Stomp State")] public Transform poolStickShadow;
        public GameObject stompHitbox;
        public float poolStickStompDistance = 5f;
        public float poolStickLeftHandDistance = 4f;
        public float poolStickRightHandDistance = 2f;
        public float stickFollowDelay = .75f;

        public ShakeSettings preStompShake;
        public ShakeSettings stickStuckShake;
        public ShakeSettings stompCameraShake;

        public float stompHoverTime = .5f;
        public int stompCountMin = 3;
        public int stompCountMax = 6;

        [Header("Visual")] public float poolHandDistance = 3f;
        public float stickHandDistance = 8f;

        [Header("References")] 
        public GameObject ballPrefab;
        public BossHealth health;
        public Rigidbody2D[] availableBalls;
        public Transform poolStick;
        public Transform leftHandTransform;
        public PoolHand leftHand;
        public Transform rightHandTransform;
        public PoolHand rightHand;

        private StateMachine fsm;
        private Rigidbody2D _currentBall;
        
        private int _currentBallCount;

        // State names
        private const string IdleCooldownState = "IdleCooldown";
        private const string FreakOutState = "FreakOut";
        private const string ShotBallState = "ShotBall";
        private const string SearchForBallsState = "SearchForBalls";
        private const string StompPlayerState = "StompPlayer";
        private const string PopulateBallsState = "PopulateBallsState";

        private void Start()
        {
            _currentBallCount = initialBallCount;
            fsm = new StateMachine();

            // IDLE COOLDOWN STATE
            fsm.AddState(
                IdleCooldownState,
                new ParallelStates(
                    new CoState(this, IdleStateCoroutineX),
                    new CoState(this, IdleStateCoroutineY),
                    new State(
                        onEnter: _ =>
                        {
                            CameraManager.Instance.FocusUp();
                            health.Defense = vulnerableDefense;
                        },
                        onExit: _ =>
                        {
                            CameraManager.Instance.ResetFocus();
                            _idleSineSequenceX.Stop();
                            _idleSineSequenceY.Stop();
                        })
                )
            );
            fsm.AddTransition(
                new TransitionAfter(
                    IdleCooldownState,
                    SearchForBallsState,
                    idleWaitTime
                )
            );

            // SEARCH FOR BALLS STATE
            fsm.AddState(SearchForBallsState, onEnter: _ =>
            {
                availableBalls = availableBalls.Where(b => b != null).ToArray();

                if (!availableBalls.Any()) return;

                _currentBall = availableBalls.ChooseRandom();
                print($"Chosen a random ball of all available: {_currentBall}");
            }, isGhostState: true);
            fsm.AddTransition(SearchForBallsState, ShotBallState, _ => availableBalls.Any());
            fsm.AddTransition(SearchForBallsState, FreakOutState, _ => !availableBalls.Any());

            // SHOT BALL STATE
            fsm.AddState(
                ShotBallState,
                new ParallelStates(
                    needsExitTime: true,
                    new CoState(
                        this,
                        ShotWhiteBallCoroutine,
                        needsExitTime: true,
                        loop: false
                    ),
                    new State(
                        onLogic: state =>
                        {
                            if (_currentBall) return;
                            print("The main ball was destroyed, trying again");
                            fsm.RequestStateChange(SearchForBallsState, true);
                        },
                        onExit: _ =>
                        {
                            rightHandTransform.SetParent(transform);
                            leftHand.SetHand(HandType.Idle);
                            rightHand.SetHand(HandType.Idle);
                        })
                )
            );
            fsm.AddTransition(ShotBallState, IdleCooldownState);

            // FREAK OUT STATE
            fsm.AddState(
                FreakOutState,
                new ParallelStates(
                    new CoState(this, IdleStateCoroutineX),
                    new CoState(this, IdleStateCoroutineY),
                    new State(
                        onEnter: _ => { CameraManager.Instance.FocusUp(); },
                        onExit: _ =>
                        {
                            CameraManager.Instance.ResetFocus();
                            _idleSineSequenceX.Stop();
                            _idleSineSequenceY.Stop();
                        })
                )
            );
            fsm.AddTransition(
                new TransitionAfter(
                    FreakOutState,
                    StompPlayerState,
                    freakOutWaitTime)
            );

            // STOMP PLAYER STATE
            fsm.AddState(StompPlayerState, new CoState(this, StompPlayerCoroutine, needsExitTime: true));
            fsm.AddTransition(StompPlayerState, PopulateBallsState);

            
            // POPULATE BALLS STATE
            // TODO: Improve random ball position select (It can sometimes spawn over other balls or even the player!!)
            // TODO: Animate hands bringing the balls (This gonna take some hard work)
            // TODO: Make it so it only keeps the existing balls, and add the needed ones
            fsm.AddState(PopulateBallsState, onEnter: _ =>
            {
                var availableBallCount = availableBalls.Count(b => b);
                print($"There are {availableBallCount} available balls");
                print($"And the ball count is {_currentBallCount}");
                print($"So, we're adding {_currentBallCount - availableBallCount} balls");
                for (var i = 0; i < _currentBallCount - availableBallCount; i++)
                {
                    var pos = new Vector2(
                        Random.Range(minBallPos.x, maxBallPos.x),
                        Random.Range(minBallPos.y, maxBallPos.y));
                    var ball = Instantiate(ballPrefab, pos, Quaternion.identity);
                    availableBalls = availableBalls.Append(ball.GetComponent<Rigidbody2D>()).ToArray();
                }
            });
            fsm.AddTransition(PopulateBallsState, IdleCooldownState);

            fsm.SetStartState(PopulateBallsState);
            
            fsm.Init();
        }

        public void IncreaseBallCount()
        {
            _currentBallCount++;
            fsm.RequestStateChange(PopulateBallsState, true);
        }
        
        
        #region < == IDLE STATE == >

        private Sequence _idleSineSequenceY;

        private IEnumerator IdleStateCoroutineY()
        {
            _idleSineSequenceY = Sequence.Create(Tween.LocalPositionY(rightHandTransform, idleSineTweenY));
            _idleSineSequenceY.Group(Tween.LocalPositionY(leftHandTransform, idleSineTweenY));
            yield return _idleSineSequenceY.ToYieldInstruction();
            _idleSineSequenceY =
                Sequence.Create(Tween.LocalPositionY(rightHandTransform, idleSineTweenY.WithDirection(false)));
            _idleSineSequenceY.Group(Tween.LocalPositionY(leftHandTransform, idleSineTweenY.WithDirection(false)));
            yield return _idleSineSequenceY.ToYieldInstruction();
        }

        private Sequence _idleSineSequenceX;

        private IEnumerator IdleStateCoroutineX()
        {
            _idleSineSequenceX = Sequence.Create(Tween.LocalPositionX(rightHandTransform, leftIdleSineTweenX));
            _idleSineSequenceX.Group(Tween.LocalPositionX(leftHandTransform, rightIdleSineTweenX));
            yield return _idleSineSequenceX.ToYieldInstruction();
            _idleSineSequenceX =
                Sequence.Create(Tween.LocalPositionX(rightHandTransform, leftIdleSineTweenX.WithDirection(false)));
            _idleSineSequenceX.Group(Tween.LocalPositionX(leftHandTransform,
                rightIdleSineTweenX.WithDirection(false)));
            yield return _idleSineSequenceX.ToYieldInstruction();
        }

        #endregion

        #region < == SHOT WHITE BALL STATE == >

        private IEnumerator ShotWhiteBallCoroutine(CoState<string, string> state)
        {
            health.Defense = vulnerableDefense;
            float nextStep = 0;
            var player = GameManager.Instance.Player.transform;

            Vector2 dir = player.position - _currentBall.transform.position;
            dir.Normalize();

            // Move stick to ball
            Tween.Position(poolStick, _currentBall.position - dir * 1.6f, 1, Ease.OutCubic);
            Tween.Rotation(poolStick, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x) * Vector3.forward, 0.9f, Ease.OutCubic);

            // Move left hand to under the stick
            Tween.Position(leftHandTransform, _currentBall.position - dir * (1.6f + poolHandDistance), 1f);
            Tween.Rotation(leftHandTransform, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x) * -Vector3.forward, 0.9f,
                Ease.OutCubic);
            // Move right hand to hold the stick
            Tween.Position(rightHandTransform, _currentBall.position - dir * (1.6f + stickHandDistance), 1f);
            Tween.Rotation(rightHandTransform, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x) * Vector3.forward, 0.9f,
                Ease.OutCubic);

            // Set hand sprites
            leftHand.SetHand(HandType.PoolHand);
            leftHand.SetOrder(0);
            rightHand.SetHand(HandType.HoldingStick);
            rightHand.SetOrder(5);

            nextStep += 0.9f;
            yield return new WaitWhile(() => state.timer.Elapsed < nextStep);

            //Aim pool stick to selected ball
            nextStep += shotAnticipationSecs;
            while (state.timer.Elapsed < nextStep)
            {
                dir = player.position - _currentBall.transform.position;
                dir.Normalize();
                poolStick.right = dir;
                leftHandTransform.right = dir;
                rightHandTransform.right = -dir;

                poolStick.transform.position = _currentBall.position - dir * 1.6f;
                leftHandTransform.transform.position = _currentBall.position - dir * (1.6f + poolHandDistance);
                rightHandTransform.transform.position = _currentBall.position - dir * (1.6f + stickHandDistance);

                yield return null;
            }

            // Start pulling stick back
            nextStep += 0.25f;
            rightHandTransform.SetParent(poolStick);
            Tween.Position(poolStick, _currentBall.position - dir * 3, 0.5f, Ease.OutCubic);

            yield return new WaitWhile(() => state.timer.Elapsed < nextStep);

            // Shake the stick and hands during the pulling
            nextStep += 0.35f;
            Tween.ShakeLocalRotation(poolStick, Vector3.forward * 2.5f, .35f, 15f);

            Tween.ShakeLocalRotation(leftHandTransform, Vector3.forward * 5f, .35f, 15f);
            Tween.ShakeLocalPosition(leftHandTransform, Vector3.one * .15f, .35f, 25f);

            yield return new WaitWhile(() => state.timer.Elapsed < nextStep);

            // Push the stick
            nextStep += 0.1f;
            Tween.Position(poolStick, _currentBall.position - dir * 1.45f, 0.1f, Ease.Linear);
            yield return new WaitWhile(() => state.timer.Elapsed < nextStep);


            // Reset all balls' linear damping (were set during the last loop)
            foreach (var ball in availableBalls)
                if (ball)
                    ball.linearDamping = 0;

            // Apply velocity to the selected ball
            _currentBall.linearVelocity = dir * shotForce;

            // Apply the knockback tween to the pool stick and hands
            Tween.Position(poolStick, poolStick.position - (Vector3)dir, .5f, Ease.OutCubic);
            Tween.Position(rightHandTransform, rightHandTransform.position - (Vector3)dir, .5f, Ease.OutCubic);
            health.Defense = attackingDefense;

            yield return new WaitForSeconds(0.5f);
            rightHandTransform.SetParent(transform);

            // Return the stick and hands to the rest position
            Tween.Rotation(poolStick, Vector3.forward * -90, 1.5f, Ease.OutSine);
            Tween.Position(poolStick, transform.position + Vector3.right, 1.5f, Ease.OutSine);

            Tween.Position(rightHandTransform, transform.position + Vector3.left * 2f, 1.5f, Ease.InOutQuad);
            Tween.Rotation(rightHandTransform, Quaternion.identity, 1.5f, Ease.InOutSine);
            Tween.Position(leftHandTransform, transform.position + Vector3.right * 3, 1.5f, Ease.InOutQuad);
            Tween.Rotation(leftHandTransform, Quaternion.identity, 1.5f, Ease.InOutSine);

            rightHand.SetHand(HandType.Idle);
            leftHand.SetHand(HandType.Idle);

            // Wait until all balls are still or the wait time elapsed
            nextStep += shotBallWaitTime;
            yield return new WaitWhile(() =>
                state.timer.Elapsed < nextStep && _currentBall?.linearVelocity.magnitude > whiteBallStopThreshold);


            // During two seconds, start reducing all balls' linear damping
            nextStep += 2f;
            var otherBalls = availableBalls.Where(b => b != _currentBall).ToArray();
            while (state.timer.Elapsed < nextStep)
            {
                _currentBall.linearDamping += currentBallStopLinearDrag * Time.fixedDeltaTime;
                foreach (var ball in otherBalls)
                    if (ball)
                        ball.linearDamping += otherBallsStopLinearDrag * Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            fsm.StateCanExit();
        }

        #endregion

        #region <== STOMP PLAYER STATE == >

        private IEnumerator AttemptStompPlayer(Transform playerTransform)
        {
            int stompCount = Random.Range(stompCountMin, stompCountMax);
                for (int i = stompCount; i >= 0; i--)
                {
                    float time = Time.time;
                    poolStick.right = Vector3.down;
                    leftHandTransform.SetParent(transform);
                    rightHandTransform.SetParent(transform);
                    while (Time.time < time + stompHoverTime)
                    {
                        poolStick.position = Vector3.Lerp(poolStick.position,
                            playerTransform.position + Vector3.up * poolStickStompDistance, stickFollowDelay);
                        poolStickShadow.position = Vector3.Lerp(poolStickShadow.position, playerTransform.position,
                            stickFollowDelay);
                        leftHandTransform.position = Vector3.Lerp(leftHandTransform.position,
                            playerTransform.position + Vector3.up * poolStickLeftHandDistance, stickFollowDelay);
                        rightHandTransform.position = Vector3.Lerp(rightHandTransform.position,
                            playerTransform.position + Vector3.up * poolStickRightHandDistance, stickFollowDelay);
                        yield return null;
                    }

                    leftHandTransform.SetParent(poolStick);
                    rightHandTransform.SetParent(poolStick);

                    yield return new WaitForSeconds(.05f);
                    Tween.ShakeLocalPosition(poolStick, preStompShake);
                    yield return new WaitForSeconds(.1f);
                    poolStickShadow.position = poolStick.position + Vector3.down * poolStickStompDistance;
                    yield return Tween
                        .PositionY(poolStick, poolStick.position.y - poolStickStompDistance, .1f, Ease.InSine)
                        .ToYieldInstruction();
                    stompHitbox.transform.position = poolStick.position;
                    CameraManager.Instance.ShakeCamera(stompCameraShake);
                    stompHitbox.SetActive(true);
                    // Yield 3 frames
                    for (int j = 0; j < 3; j++) yield return null;
                    stompHitbox.SetActive(false);

                    yield return new WaitForSeconds(.3f);

                    if (i > 0)
                    {
                        Tween.PositionY(poolStick, poolStick.position.y + poolStickStompDistance, .35f, Ease.OutCubic);
                    }
                    else
                    {
                        yield return Tween.ShakeLocalRotation(poolStick, stickStuckShake).ToYieldInstruction();
                        CameraManager.Instance.ShakeCamera(stompCameraShake);
                        yield return Tween
                            .PositionY(poolStick, poolStick.position.y + poolStickStompDistance, .5f, Ease.OutCubic)
                            .ToYieldInstruction();
                    }
                }
        }
        private IEnumerator StompPlayerCoroutine(CoState<string, string> state)
        {
            leftHand.SetHand(HandType.HoldingStomp);
            rightHand.SetHand(HandType.HoldingStomp);
            rightHand.SetOrder(5);
            leftHand.SetOrder(5);
            leftHandTransform.right = Vector3.left;
            rightHandTransform.right = Vector3.left;
            var playerTransform = GameManager.Instance.Player.transform;
            poolStickShadow.gameObject.SetActive(true);
            for (int i = 0; i < 3; i++)
            {
                yield return AttemptStompPlayer(playerTransform);
            }
            poolStickShadow.gameObject.SetActive(false);
            leftHand.SetHand(HandType.Idle);
            rightHand.SetHand(HandType.Idle);
            leftHandTransform.SetParent(transform);
            rightHandTransform.SetParent(transform);
            
            var sequence = Sequence.Create(Tween.Rotation(poolStick, Vector3.forward * -90, 1.5f, Ease.OutSine));
            // Return the stick and hands to the rest position
            sequence.Group(Tween.Position(poolStick, transform.position + Vector3.right, 1.5f, Ease.OutSine));

            sequence.Group(Tween.Position(rightHandTransform, transform.position + Vector3.left * 2f, 1.5f, Ease.InOutQuad));
            sequence.Group(Tween.Rotation(rightHandTransform, Quaternion.identity, 1.5f, Ease.InOutSine));
            sequence.Group(Tween.Position(leftHandTransform, transform.position + Vector3.right * 3, 1.5f, Ease.InOutQuad));
            sequence.Group(Tween.Rotation(leftHandTransform, Quaternion.identity, 1.5f, Ease.InOutSine));

            rightHand.SetHand(HandType.Idle);
            leftHand.SetHand(HandType.Idle);

            yield return sequence.ToYieldInstruction();
            
            state.fsm.StateCanExit();
        }

        #endregion

        private void FixedUpdate()
        {
            fsm.OnLogic();
        }
    }
}