using System.Linq;
using Game.Systems;
using PrimeTween;

namespace Game.Bosses.Snooker
{
    using System.Collections;
    using Player;
    using UnityEngine;
    using UnityHFSM;

    public class SnookerBoss : MonoBehaviour
    {
        [Header("Main Idle")]
        public float idleWaitTimeMin = 4;
        public float idleWaitTimeMax = 6;

        [Header("Shot White Ball")]
        public float shotBallWaitTime = 5f;

        public float whiteBallStopThreshold = 1f;
        [Space]
        public float shotForce = 25;
        public float shotAnticipationSecs = 2f;
        public float ballStopLinearDrag = 0.25f;
        public float redBallStopLinearDrag = 0.1f;

        [Header("Visual")]
        public float poolHandDistance = 3f;
        
        [Header("References")]
        public Rigidbody2D[] availableBalls;
        public Transform poolStick;
        public Transform poolStickHand;

        private StateMachine fsm;

        private const string IdleCooldownState = "IdleCooldown";
        private const string AttackCooldownState = "AttackCooldown";
        private const string ShotWhiteBallState = "ShotWhiteBall";

        private IEnumerator ShotWhiteBall(CoState<string, string> state)
        {
            availableBalls = availableBalls.Where(b => b != null).ToArray();
            var currentBall = availableBalls.ChooseRandom();
            float nextStep = 0;
            var player = GameManager.Instance.Player.transform;

            Vector2 dir = player.position - currentBall.transform.position;
            dir.Normalize();

            Tween.Position(poolStick, currentBall.position - dir * 1.6f, 1, Ease.OutCubic);
            Tween.Rotation(poolStick, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x) * Vector3.forward, 0.9f, Ease.OutCubic);
            
            Tween.Position(poolStickHand, currentBall.position - dir * (1.6f + poolHandDistance), 1f);
            Tween.Rotation(poolStickHand, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x) * Vector3.forward, 0.9f, Ease.OutCubic);

            nextStep += 0.9f;
            yield return new WaitWhile(() => state.timer.Elapsed < nextStep);


            nextStep += shotAnticipationSecs;
            while (state.timer.Elapsed < nextStep)
            {
                dir = player.transform.position - currentBall.transform.position;
                dir.Normalize();
                poolStick.right = dir;
                poolStickHand.right = dir;

                poolStick.transform.position = currentBall.position - dir * 1.6f;
                poolStickHand.transform.position = currentBall.position - dir * (1.6f + poolHandDistance);

                yield return null;
            }

            nextStep += 0.55f;
            Tween.Position(poolStick, currentBall.position - dir * 3, 0.5f, Ease.OutCubic);
            yield return new WaitWhile(() => state.timer.Elapsed < nextStep);

            nextStep += 0.1f;
            Tween.Position(poolStick, currentBall.position - dir * 1.45f, 0.1f, Ease.Linear);
            yield return new WaitWhile(() => state.timer.Elapsed < nextStep);

            foreach (var ball in availableBalls)
                ball.linearDamping = 0;
            currentBall.linearVelocity = dir * shotForce;

            Tween.Rotation(poolStick, Vector3.forward * -90, 1.5f, Ease.OutSine);
            Tween.Position(poolStick, transform.position + Vector3.right, 1.5f, Ease.OutSine);

            Tween.Position(poolStickHand, transform.position, 1.5f, Ease.OutSine);
            Tween.Rotation(poolStickHand, Vector3.forward * -90, 1.5f, Ease.OutSine);


            nextStep += shotBallWaitTime;
            yield return new WaitWhile(() => state.timer.Elapsed < nextStep && currentBall.linearVelocity.magnitude > whiteBallStopThreshold);

            nextStep += 2f;
            
            var otherBalls = availableBalls.Where(b => b != currentBall);
            while (state.timer.Elapsed < nextStep)
            {
                currentBall.linearDamping += ballStopLinearDrag * Time.fixedDeltaTime;
                foreach (var ball in otherBalls)
                    ball.linearDamping += redBallStopLinearDrag * Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            fsm.StateCanExit();
        }

        private void Start()
        {
            fsm = new StateMachine();
            fsm.AddState(IdleCooldownState, onEnter: _ =>
            {
                CameraManager.Instance.FocusUp();
            }, onExit: _ =>
            {
                CameraManager.Instance.ResetFocus();
            });

            fsm.AddState(ShotWhiteBallState, new CoState(
                this,
                ShotWhiteBall,
                needsExitTime: true,
                loop: false
            ));

            fsm.AddTransition(
                new TransitionAfterDynamic(
                    IdleCooldownState,
                    ShotWhiteBallState,
                    s => Random.Range(idleWaitTimeMin, idleWaitTimeMax)
                )
            );

            fsm.AddTransition(ShotWhiteBallState, IdleCooldownState);

            fsm.Init();
        }

        private void FixedUpdate()
        {
            fsm.OnLogic();
        }
    }
}