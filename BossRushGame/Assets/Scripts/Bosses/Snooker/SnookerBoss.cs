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

        [Header("Visual")] public float poolHandDistance = 3f;

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
        private const string AttackCooldownState = "AttackCooldown";
        private const string ShotBallState = "ShotBall";
        private const string SearchForBallsState = "SearchForBalls";

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
                _currentBall = availableBalls.ChooseRandom();
                print($"Chosen a random ball of all available: {_currentBall}");
            }, isGhostState: true);
            fsm.AddTransition(SearchForBallsState, ShotBallState);

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
                    })
                )
            );
            fsm.AddTransition(ShotBallState, IdleCooldownState);

            fsm.Init();
        }

        private Sequence _idleSineSequenceY;

        private IEnumerator IdleStateCoroutineY()
        {
            _idleSineSequenceY = Sequence.Create(Tween.LocalPositionY(leftHandTransform, idleSineTweenY));
            _idleSineSequenceY.Group(Tween.LocalPositionY(rightHandTransform, idleSineTweenY));
            yield return _idleSineSequenceY.ToYieldInstruction();
            _idleSineSequenceY = Sequence.Create(Tween.LocalPositionY(leftHandTransform, idleSineTweenY.WithDirection(false)));
            _idleSineSequenceY.Group(Tween.LocalPositionY(rightHandTransform, idleSineTweenY.WithDirection(false)));
            yield return _idleSineSequenceY.ToYieldInstruction();
        }
        private Sequence _idleSineSequenceX;
        private IEnumerator IdleStateCoroutineX()
        {
            _idleSineSequenceX = Sequence.Create(Tween.LocalPositionX(leftHandTransform, leftIdleSineTweenX));
            _idleSineSequenceX.Group(Tween.LocalPositionX(rightHandTransform, rightIdleSineTweenX));
            yield return _idleSineSequenceX.ToYieldInstruction();
            _idleSineSequenceX = Sequence.Create(Tween.LocalPositionX(leftHandTransform, leftIdleSineTweenX.WithDirection(false)));
            _idleSineSequenceX.Group(Tween.LocalPositionX(rightHandTransform, rightIdleSineTweenX.WithDirection(false)));
            yield return _idleSineSequenceX.ToYieldInstruction();
        }

        private IEnumerator ShotWhiteBallCoroutine(CoState<string, string> state)
        {
            float nextStep = 0;
            var player = GameManager.Instance.Player.transform;

            Vector2 dir = player.position - _currentBall.transform.position;
            dir.Normalize();

            // Move stick to ball
            Tween.Position(poolStick, _currentBall.position - dir * 1.6f, 1, Ease.OutCubic);
            Tween.Rotation(poolStick, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x) * Vector3.forward, 0.9f, Ease.OutCubic);

            // Move pool hand too
            Tween.Position(leftHandTransform, _currentBall.position - dir * (1.6f + poolHandDistance), 1f);
            Tween.Rotation(leftHandTransform, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x) * -Vector3.forward, 0.9f,
                Ease.OutCubic);
            
            leftHand.SetHand(HandType.PoolHand);

            nextStep += 0.9f;
            yield return new WaitWhile(() => state.timer.Elapsed < nextStep);

            nextStep += shotAnticipationSecs;
            while (state.timer.Elapsed < nextStep)
            {
                dir = player.transform.position - _currentBall.transform.position;
                dir.Normalize();
                poolStick.right = dir;
                leftHandTransform.right = -dir;

                poolStick.transform.position = _currentBall.position - dir * 1.6f;
                leftHandTransform.transform.position = _currentBall.position - dir * (1.6f + poolHandDistance);

                yield return null;
            }

            nextStep += 0.25f;
            Tween.Position(poolStick, _currentBall.position - dir * 3, 0.5f, Ease.OutCubic);
            yield return new WaitWhile(() => state.timer.Elapsed < nextStep);
            nextStep += 0.35f;
            Tween.ShakeLocalRotation(poolStick, Vector3.forward * 2.5f, .35f, 15f);
            Tween.ShakeLocalRotation(leftHandTransform, Vector3.forward * 5f, .35f, 15f);
            Tween.ShakeLocalPosition(leftHandTransform, Vector3.one * .15f, .35f, 25f);
            yield return new WaitWhile(() => state.timer.Elapsed < nextStep);

            nextStep += 0.1f;
            Tween.Position(poolStick, _currentBall.position - dir * 1.45f, 0.1f, Ease.Linear);
            yield return new WaitWhile(() => state.timer.Elapsed < nextStep);

            foreach (var ball in availableBalls)
                if (ball)
                    ball.linearDamping = 0;

            _currentBall.linearVelocity = dir * shotForce;
            
            Tween.Position(poolStick, poolStick.position - (Vector3)dir, .5f, Ease.OutCubic);
            Tween.Position(leftHandTransform, leftHandTransform.position - (Vector3)dir, .5f, Ease.OutCubic);

            yield return new WaitForSeconds(0.5f);

            Tween.Rotation(poolStick, Vector3.forward * -90, 1.5f, Ease.OutSine);
            Tween.Position(poolStick, transform.position + Vector3.right, 1.5f, Ease.OutSine);

            Tween.Position(leftHandTransform, transform.position, 1.5f, Ease.InOutQuad);
            Tween.Rotation(leftHandTransform, Quaternion.identity, 1.5f, Ease.InOutSine);

            leftHand.SetHand(HandType.Idle);
            nextStep += shotBallWaitTime;
            yield return new WaitWhile(() =>
                state.timer.Elapsed < nextStep && _currentBall.linearVelocity.magnitude > whiteBallStopThreshold);

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

        private void FixedUpdate()
        {
            fsm.OnLogic();
        }
    }
}