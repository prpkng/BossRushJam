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
        public Rigidbody2D whiteBall;
        public Rigidbody2D[] redBalls;
        public Transform poolStick;
        public Transform poolStickHand;

        private StateMachine fsm;

        private const string IdleCooldownState = "IdleCooldown";
        private const string AttackCooldownState = "AttackCooldown";
        private const string ShotWhiteBallState = "ShotWhiteBall";

        private IEnumerator ShotWhiteBall(CoState<string, string> state)
        {
            float nextStep = 0;
            var player = GameManager.Instance.Player.transform;

            Vector2 dir = player.position - whiteBall.transform.position;
            dir.Normalize();

            Tween.Position(poolStick, whiteBall.position - dir * 1.6f, 1, Ease.OutCubic);
            Tween.Rotation(poolStick, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x) * Vector3.forward, 0.9f, Ease.OutCubic);
            
            Tween.Position(poolStickHand, whiteBall.position - dir * (1.6f + poolHandDistance), 1f);
            Tween.Rotation(poolStickHand, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x) * Vector3.forward, 0.9f, Ease.OutCubic);

            nextStep += 0.9f;
            yield return new WaitWhile(() => state.timer.Elapsed < nextStep);


            nextStep += shotAnticipationSecs;
            while (state.timer.Elapsed < nextStep)
            {
                dir = player.transform.position - whiteBall.transform.position;
                dir.Normalize();
                poolStick.right = dir;
                poolStickHand.right = dir;

                poolStick.transform.position = whiteBall.position - dir * 1.6f;
                poolStickHand.transform.position = whiteBall.position - dir * (1.6f + poolHandDistance);

                yield return null;
            }

            nextStep += 0.55f;
            Tween.Position(poolStick, whiteBall.position - dir * 3, 0.5f, Ease.OutCubic);
            yield return new WaitWhile(() => state.timer.Elapsed < nextStep);

            nextStep += 0.1f;
            Tween.Position(poolStick, whiteBall.position - dir * 1.45f, 0.1f, Ease.Linear);
            yield return new WaitWhile(() => state.timer.Elapsed < nextStep);

            whiteBall.linearDamping = 0;
            foreach (var ball in redBalls)
                ball.linearDamping = 0;
            whiteBall.linearVelocity = dir * shotForce;

            Tween.Rotation(poolStick, Vector3.forward * -90, 1.5f, Ease.OutSine);
            Tween.Position(poolStick, transform.position + Vector3.right, 1.5f, Ease.OutSine);

            Tween.Position(poolStickHand, transform.position, 1.5f, Ease.OutSine);
            Tween.Rotation(poolStickHand, Vector3.forward * -90, 1.5f, Ease.OutSine);


            nextStep += shotBallWaitTime;
            yield return new WaitWhile(() => state.timer.Elapsed < nextStep && whiteBall.linearVelocity.magnitude > whiteBallStopThreshold);

            nextStep += 2f;
            while (state.timer.Elapsed < nextStep)
            {
                whiteBall.linearDamping += ballStopLinearDrag * Time.fixedDeltaTime;
                foreach (var ball in redBalls)
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