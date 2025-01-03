namespace Game.Bosses.Snooker
{
    using System.Collections;
    using DG.Tweening;
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

        [Header("References")]
        public Rigidbody2D whiteBall;
        public Transform poolStick;

        private StateMachine fsm;

        private const string IdleCooldownState = "IdleCooldown";
        private const string AttackCooldownState = "AttackCooldown";
        private const string ShotWhiteBallState = "ShotWhiteBall";

        public IEnumerator ShotWhiteBall(CoState<string, string> state)
        {
            float nextStep = 0;
            var player = GameManager.Instance.Player.transform;

            Vector2 dir = player.position - whiteBall.transform.position;
            dir.Normalize();

            poolStick.DOMove(whiteBall.position - dir * 1.5f, 1f).SetEase(Ease.OutCubic);
            poolStick.DORotate(Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x) * Vector3.forward, 1f)
                        .SetEase(Ease.OutCubic);
            nextStep += 0.9f;
            yield return new WaitWhile(() => state.timer.Elapsed < nextStep);

            nextStep += shotAnticipationSecs;
            while (state.timer.Elapsed < nextStep)
            {
                dir = player.transform.position - whiteBall.transform.position;
                dir.Normalize();
                poolStick.right = dir;

                poolStick.transform.position = whiteBall.position - dir * 1.6f;

                Debug.DrawLine(
                    whiteBall.transform.position,
                    whiteBall.transform.position + (Vector3)dir * 5,
                    Color.magenta
                );
                yield return null;
            }

            nextStep += 0.5f;
            poolStick.DOMove(whiteBall.position - dir * 3, 0.5f).SetEase(Ease.OutCubic);
            yield return new WaitWhile(() => state.timer.Elapsed < nextStep);

            nextStep += 0.1f;
            poolStick.DOMove(whiteBall.position - dir * 1.45f, 0.1f).SetEase(Ease.Linear);
            yield return new WaitWhile(() => state.timer.Elapsed < nextStep);

            whiteBall.AddForce(dir * shotForce, ForceMode2D.Impulse);
            poolStick.DOKill(true);
            // poolStick.DORotate(Vector3.forward * -90, 1.5f).SetEase(Ease.InOutCubic);
            poolStick.DOMove(transform.position + Vector3.right * 1, 1.5f).SetEase(Ease.InOutCubic);


            fsm.StateCanExit();
        }

        private void Start()
        {
            fsm = new StateMachine();
            fsm.AddState(IdleCooldownState, onEnter: _ => whiteBall.linearVelocity = Vector2.zero);
            fsm.AddState(AttackCooldownState);

            fsm.AddState(ShotWhiteBallState, new CoState(
                this,
                ShotWhiteBall,
                needsExitTime: true
            ));

            fsm.AddTransition(
                new TransitionAfterDynamic(
                    IdleCooldownState,
                    ShotWhiteBallState,
                    s => Random.Range(idleWaitTimeMin, idleWaitTimeMax)
                )
            );

            fsm.AddTransition(
                new TransitionAfter(
                    AttackCooldownState,
                    IdleCooldownState,
                    4
                )
            );

            fsm.AddTransition(ShotWhiteBallState, AttackCooldownState);

            fsm.Init();
        }

        private void FixedUpdate()
        {
            fsm.OnLogic();
        }
    }
}