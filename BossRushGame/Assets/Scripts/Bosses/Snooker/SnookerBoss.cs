namespace Game.Bosses.Snooker
{
    using System.Collections;
    using DG.Tweening;
    using Game.Player;
    using UnityEngine;
    using UnityHFSM;

    public class SnookerBoss : MonoBehaviour
    {
        [Header("Main Idle")]
        public float idleWaitTimeMin = 4;
        public float idleWaitTimeMax = 6;

        [Header("Shot White Ball")]
        public float shotForce = 25;
        public float shotAnticipationSecs = 2f;
        public float ballBouceTime = 5f;
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

        public IEnumerator ShotWhiteBall(CoState<string, string> state)
        {
            float _nextStep = 0;
            var player = GameManager.Instance.Player.transform;

            Vector2 dir = player.position - whiteBall.transform.position;
            dir.Normalize();

            poolStick.DOMove(whiteBall.position - dir * 1.6f, 1f);
            poolStick.DORotate(Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x) * Vector3.forward, 0.9f)
                .SetEase(Ease.OutCubic);
            poolStickHand.DOMove(whiteBall.position - dir * (1.6f + poolHandDistance), 1f);
            poolStickHand.DORotate(Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x) * Vector3.forward, 0.9f)
                .SetEase(Ease.OutCubic);

            _nextStep += 0.9f;
            yield return new WaitWhile(() => state.timer.Elapsed < _nextStep);


            _nextStep += shotAnticipationSecs;
            while (state.timer.Elapsed < _nextStep)
            {
                dir = player.transform.position - whiteBall.transform.position;
                dir.Normalize();
                poolStick.right = dir;
                poolStickHand.right = dir;

                poolStick.transform.position = whiteBall.position - dir * 1.6f;
                poolStickHand.transform.position = whiteBall.position - dir * (1.6f + poolHandDistance);

                yield return null;
            }

            _nextStep += 0.55f;
            poolStick.DOMove(whiteBall.position - dir * 3, 0.5f).SetEase(Ease.OutCubic);
            yield return new WaitWhile(() => state.timer.Elapsed < _nextStep);

            _nextStep += 0.1f;
            poolStick.DOMove(whiteBall.position - dir * 1.45f, 0.1f).SetEase(Ease.Linear);
            yield return new WaitWhile(() => state.timer.Elapsed < _nextStep);

            whiteBall.linearDamping = 0;
            foreach (var ball in redBalls)
                ball.linearDamping = 0;
            whiteBall.AddForce(dir * shotForce, ForceMode2D.Impulse);

            poolStick.DORotate(Vector3.forward * -90, 1.5f).SetEase(Ease.OutSine);
            poolStick.DOMove(transform.position + Vector3.right, 1.5f).SetEase(Ease.OutSine);

            poolStickHand.DOMove(transform.position, 1.5f).SetEase(Ease.OutSine);
            poolStickHand.DORotate(Vector3.forward * -90, 1.5f).SetEase(Ease.OutSine);


            _nextStep += ballBouceTime;
            yield return new WaitWhile(() => state.timer.Elapsed < _nextStep);

            _nextStep += 2f;
            while (state.timer.Elapsed < _nextStep)
            {
                whiteBall.linearDamping += ballStopLinearDrag * Time.fixedDeltaTime;
                foreach (var ball in redBalls)
                    ball.linearDamping += redBallStopLinearDrag * Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            // whiteBall.linearDamping = 30;
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