namespace Game.Bosses.Snooker
{
    using System.Collections;
    using UnityEngine;
    using UnityHFSM;

    public class SnookerBoss : MonoBehaviour
    {
        public float idleWaitTimeMin = 4;
        public float idleWaitTimeMax = 6;

        public Rigidbody2D whiteBall;
        public Transform poolStick;

        public float shotForce = 25;


        private StateMachine fsm;

        private const string IdleCooldownState = "IdleCooldown";
        private const string AttackCooldownState = "AttackCooldown";
        private const string ShotWhiteBallState = "ShotWhiteBall";

        public IEnumerator ShotWhiteBall(CoState<string, string> state)
        {
            print("TriggerShoot");
            Vector2 dir = Vector2.zero;

            while (state.timer.Elapsed < 1f)
            {
                dir = GameManager.Instance.Player.transform.position - whiteBall.transform.position;
                dir.Normalize();
                Debug.DrawLine(
                    whiteBall.transform.position,
                    whiteBall.transform.position + (Vector3)dir * 5,
                    Color.magenta
                );
                yield return null;
            }
            yield return new WaitWhile(() => state.timer.Elapsed < 1.5f);

            whiteBall.AddForce(dir * shotForce, ForceMode2D.Impulse);

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