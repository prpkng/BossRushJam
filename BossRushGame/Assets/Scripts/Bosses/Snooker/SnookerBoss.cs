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
        [Header("Idle Cooldown")] public float idleWaitTimeMin = 4;
        public float idleWaitTimeMax = 6;

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

        [Header("Stomp State")] public float poolStickStompDistance = 5f;

        [Header("Visual")] public float poolHandDistance = 3f;
        public float stickHandDistance = 8f;

        [Header("References")] public Rigidbody2D[] availableBalls;
        public Transform poolStick;
        public Transform rightHandTransform;
        public PoolHand rightHand;
        public Transform leftHandTransform;
        public PoolHand leftHand;

        private StateMachine fsm;
        private Rigidbody2D _currentBall;

        // State names
        private const string IdleCooldownState = "IdleCooldown";
        private const string FreakOutState = "FreakOut";
        private const string ShotBallState = "ShotBall";
        private const string SearchForBallsState = "SearchForBalls";
        private const string StompPlayerState = "StompPlayer";

        private void Start()
        {
            fsm = new StateMachine();

            // IDLE COOLDOWN STATE
            fsm.AddState(
                IdleCooldownState,
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
                new TransitionAfterDynamic(
                    IdleCooldownState,
                    SearchForBallsState,
                    s => Random.Range(idleWaitTimeMin, idleWaitTimeMax)
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
                    new State(onLogic: state =>
                        {
                            if (_currentBall) return;
                            print("The main ball was destroyed, trying again");
                            fsm.RequestStateChange(SearchForBallsState, true);
                        },
                        onExit: _ => rightHandTransform.SetParent(transform)
                    )
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

            fsm.AddState(StompPlayerState, new CoState(this, StompPlayerCoroutine));

            fsm.Init();
        }


        #region < == IDLE STATE == >

        private Sequence _idleSineSequenceY;

        private IEnumerator IdleStateCoroutineY()
        {
            _idleSineSequenceY = Sequence.Create(Tween.LocalPositionY(leftHandTransform, idleSineTweenY));
            _idleSineSequenceY.Group(Tween.LocalPositionY(rightHandTransform, idleSineTweenY));
            yield return _idleSineSequenceY.ToYieldInstruction();
            _idleSineSequenceY =
                Sequence.Create(Tween.LocalPositionY(leftHandTransform, idleSineTweenY.WithDirection(false)));
            _idleSineSequenceY.Group(Tween.LocalPositionY(rightHandTransform, idleSineTweenY.WithDirection(false)));
            yield return _idleSineSequenceY.ToYieldInstruction();
        }

        private Sequence _idleSineSequenceX;

        private IEnumerator IdleStateCoroutineX()
        {
            _idleSineSequenceX = Sequence.Create(Tween.LocalPositionX(leftHandTransform, leftIdleSineTweenX));
            _idleSineSequenceX.Group(Tween.LocalPositionX(rightHandTransform, rightIdleSineTweenX));
            yield return _idleSineSequenceX.ToYieldInstruction();
            _idleSineSequenceX =
                Sequence.Create(Tween.LocalPositionX(leftHandTransform, leftIdleSineTweenX.WithDirection(false)));
            _idleSineSequenceX.Group(Tween.LocalPositionX(rightHandTransform,
                rightIdleSineTweenX.WithDirection(false)));
            yield return _idleSineSequenceX.ToYieldInstruction();
        }

        #endregion

        #region < == SHOT WHITE BALL STATE == >

        private IEnumerator ShotWhiteBallCoroutine(CoState<string, string> state)
        {
            float nextStep = 0;
            var player = GameManager.Instance.Player.transform;

            Vector2 dir = player.position - _currentBall.transform.position;
            dir.Normalize();

            // Move stick to ball
            Tween.Position(poolStick, _currentBall.position - dir * 1.6f, 1, Ease.OutCubic);
            Tween.Rotation(poolStick, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x) * Vector3.forward, 0.9f, Ease.OutCubic);

            // Move pool hands too
            Tween.Position(leftHandTransform, _currentBall.position - dir * (1.6f + poolHandDistance), 1f);
            Tween.Rotation(leftHandTransform, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x) * -Vector3.forward, 0.9f,
                Ease.OutCubic);
            Tween.Position(rightHandTransform, _currentBall.position - dir * (1.6f + stickHandDistance), 1f);
            Tween.Rotation(rightHandTransform, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x) * Vector3.forward, 0.9f,
                Ease.OutCubic);

            // Set hand sprites
            leftHand.SetHand(HandType.PoolHand);
            rightHand.SetHand(HandType.HoldingStick);

            nextStep += 0.9f;
            yield return new WaitWhile(() => state.timer.Elapsed < nextStep);

            //Aim poolstick to selected ball
            nextStep += shotAnticipationSecs;
            while (state.timer.Elapsed < nextStep)
            {
                dir = player.transform.position - _currentBall.transform.position;
                dir.Normalize();
                poolStick.right = dir;
                leftHandTransform.right = -dir;
                rightHandTransform.right = dir;

                poolStick.transform.position = _currentBall.position - dir * 1.6f;
                leftHandTransform.transform.position = _currentBall.position - dir * (1.6f + poolHandDistance);
                rightHandTransform.transform.position = _currentBall.position - dir * (1.6f + stickHandDistance);

                yield return null;
            }

            // Start pulling stick back
            nextStep += 0.25f;
            Tween.Position(poolStick, _currentBall.position - dir * 3, 0.5f, Ease.OutCubic);
            Tween.Position(rightHandTransform, rightHandTransform.position - (Vector3)dir * 3, 0.5f, Ease.OutCubic);

            yield return new WaitWhile(() => state.timer.Elapsed < nextStep);
            rightHandTransform.SetParent(poolStick);

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
            Tween.Position(leftHandTransform, leftHandTransform.position - (Vector3)dir, .5f, Ease.OutCubic);

            yield return new WaitForSeconds(0.5f);
            rightHandTransform.SetParent(transform);

            // Return the stick and hands to the rest position
            Tween.Rotation(poolStick, Vector3.forward * -90, 1.5f, Ease.OutSine);
            Tween.Position(poolStick, transform.position + Vector3.right, 1.5f, Ease.OutSine);

            Tween.Position(leftHandTransform, transform.position + Vector3.left * 2f, 1.5f, Ease.InOutQuad);
            Tween.Rotation(leftHandTransform, Quaternion.identity, 1.5f, Ease.InOutSine);
            Tween.Position(rightHandTransform, transform.position + Vector3.right * 3, 1.5f, Ease.InOutQuad);
            Tween.Rotation(rightHandTransform, Quaternion.identity, 1.5f, Ease.InOutSine);

            leftHand.SetHand(HandType.Idle);

            // Wait until all balls are still or the wait time elapsed
            nextStep += shotBallWaitTime;
            yield return new WaitWhile(() =>
                state.timer.Elapsed < nextStep && _currentBall.linearVelocity.magnitude > whiteBallStopThreshold);


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

        private IEnumerator StompPlayerCoroutine(CoState<string, string> state)
        {
            var playerTransform = GameManager.Instance.Player.transform;

            float nextStep = 0f;
            nextStep += 2f;

            while (state.timer.Elapsed < nextStep)
            {
                poolStick.position = playerTransform.position + Vector3.up * poolStickStompDistance;
                poolStick.right = Vector3.down;
                yield return null;
            }
        }

        #endregion

        private void FixedUpdate()
        {
            fsm.OnLogic();
        }
    }
}