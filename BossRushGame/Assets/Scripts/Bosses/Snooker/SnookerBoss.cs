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
        [Header("Main Idle")] public float idleWaitTimeMin = 4;
        public float idleWaitTimeMax = 6;

        [Header("Shot White Ball")] public float shotBallWaitTime = 5f;

        public float whiteBallStopThreshold = 1f;
        [Space] public float shotForce = 25;
        public float shotAnticipationSecs = 2f;
        [FormerlySerializedAs("ballStopLinearDrag")] public float currentBallStopLinearDrag = 0.25f;
        [FormerlySerializedAs("redBallStopLinearDrag")] public float otherBallsStopLinearDrag = 0.1f;

        [Header("Visual")] public float poolHandDistance = 3f;

        [Header("References")] public Rigidbody2D[] availableBalls;
        public Transform poolStick;
        public Transform poolStickHand;

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
                onEnter: _ => { CameraManager.Instance.FocusUp(); },
                onExit: _ => { CameraManager.Instance.ResetFocus(); }
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
            Tween.Position(poolStickHand, _currentBall.position - dir * (1.6f + poolHandDistance), 1f);
            Tween.Rotation(poolStickHand, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x) * Vector3.forward, 0.9f,
                Ease.OutCubic);

            nextStep += 0.9f;
            yield return new WaitWhile(() => state.timer.Elapsed < nextStep);
            
            nextStep += shotAnticipationSecs;
            while (state.timer.Elapsed < nextStep)
            {
                dir = player.transform.position - _currentBall.transform.position;
                dir.Normalize();
                poolStick.right = dir;
                poolStickHand.right = dir;

                poolStick.transform.position = _currentBall.position - dir * 1.6f;
                poolStickHand.transform.position = _currentBall.position - dir * (1.6f + poolHandDistance);

                yield return null;
            }

            nextStep += 0.55f;
            Tween.Position(poolStick, _currentBall.position - dir * 3, 0.5f, Ease.OutCubic);
            yield return new WaitWhile(() => state.timer.Elapsed < nextStep);

            nextStep += 0.1f;
            Tween.Position(poolStick, _currentBall.position - dir * 1.45f, 0.1f, Ease.Linear);
            yield return new WaitWhile(() => state.timer.Elapsed < nextStep);

            foreach (var ball in availableBalls)
                if (ball) ball.linearDamping = 0;
            
            _currentBall.linearVelocity = dir * shotForce;

            Tween.Rotation(poolStick, Vector3.forward * -90, 1.5f, Ease.OutSine);
            Tween.Position(poolStick, transform.position + Vector3.right, 1.5f, Ease.OutSine);

            Tween.Position(poolStickHand, transform.position, 1.5f, Ease.OutSine);
            Tween.Rotation(poolStickHand, Vector3.forward * -90, 1.5f, Ease.OutSine);


            nextStep += shotBallWaitTime;
            yield return new WaitWhile(() =>
                state.timer.Elapsed < nextStep && _currentBall.linearVelocity.magnitude > whiteBallStopThreshold);

            nextStep += 2f;

            var otherBalls = availableBalls.Where(b => b != _currentBall).ToArray();
            while (state.timer.Elapsed < nextStep)
            {
                _currentBall.linearDamping += currentBallStopLinearDrag * Time.fixedDeltaTime;
                foreach (var ball in otherBalls)
                    if (ball) ball.linearDamping += otherBallsStopLinearDrag * Time.fixedDeltaTime;
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