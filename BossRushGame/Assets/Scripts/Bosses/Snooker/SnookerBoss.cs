using System.Diagnostics;
using System.Linq;
using System.Numerics;
using FMODUnity;
using BRJ.Systems;
using PrimeTween;
using UnityEngine.Serialization;

namespace BRJ.Bosses.Snooker
{
    using System.Collections;
    using Cysharp.Threading.Tasks;
    using FMOD.Studio;
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
        public HandsIdleSine handsIdleSine;

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

        public TweenSettings stompTween;

        public float stompHoverTime = .5f;
        public int stompCountMin = 3;
        public int stompCountMax = 6;

        [Header("Visual")] public float poolHandDistance = 3f;
        public float stickHandDistance = 8f;

        public ShakeSettings stompCameraShake;
        public ShakeSettings stompStuckCameraShake;


        [Header("Sound")]
        public EventReference moveToShot;
        public EventReference poolShotEvent;
        public EventReference returnHandsEvent;
        public EventReference shotBallSound;
        public EventReference stompHitSound;
        public float returnHandsMinSoundDistance = 4;
        public EventReference moveHandEvent;
        [Header("References")] public GameObject ballPrefab;
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

        private EventInstance shotBallInstance;
        private EventInstance stompHitInstance;

        private void Start()
        {
            // Create sound instances
            shotBallInstance = RuntimeManager.CreateInstance(shotBallSound);
            stompHitInstance = RuntimeManager.CreateInstance(stompHitSound);

            _currentBallCount = initialBallCount;
            fsm = new StateMachine();

            // IDLE COOLDOWN STATE
            fsm.AddState(
                IdleCooldownState,
                new State(
                    onEnter: _ =>
                    {
                        ReturnPoolStick();
                        Game.Instance.Sound.BossMusic.With(b => b.SetKidding());
                        Game.Instance.Camera.FocusUp();
                        health.Defense = vulnerableDefense;
                        handsIdleSine.StartMovement();
                    },
                    onExit: _ =>
                    {
                        Game.Instance.Camera.ResetFocus();
                        handsIdleSine.StopMovement();
                    }
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
            fsm.AddTransition(ShotBallState, ShotBallState);

            // FREAK OUT STATE
            fsm.AddState(
                FreakOutState,
                new CoState(
                    this,
                    FreakOutCoroutine,
                    loop: false,
                    onEnter: _ =>
                    {
                        Game.Instance.Camera.FocusUp();
                    },
                    onExit: _ =>
                    {
                        Game.Instance.Camera.ResetFocus();
                        handsIdleSine.StopMovement();
                    }
                )
            );
            fsm.AddTransition(
                new TransitionAfter(
                    FreakOutState,
                    StompPlayerState,
                    freakOutWaitTime)
            );

            // STOMP PLAYER STATE
            fsm.AddState(
                StompPlayerState,
                new CoState(
                    this,
                    StompPlayerCoroutine,
                    needsExitTime: true,
                    loop: false,
                    onExit: _ => poolStickShadow.gameObject.SetActive(false)
                )
            );
            fsm.AddTransition(StompPlayerState, PopulateBallsState);


            // POPULATE BALLS STATE
            fsm.AddState(PopulateBallsState,
                new CoState(this, PopulateBallsCoroutine, needsExitTime: true, loop: false)
                );
            fsm.AddTransition(PopulateBallsState, IdleCooldownState);

            fsm.SetStartState(PopulateBallsState);

            fsm.Init();
        }

        public void IncreaseBallCount()
        {
            _currentBallCount++;
            fsm.RequestStateChange(PopulateBallsState);
        }

        private void ReturnPoolStick()
        {
            poolStick.SetParent(leftHandTransform);
            Tween.LocalPosition(poolStick, new Vector3(1.5f, -2f), 1f, Ease.InOutQuad);
            Tween.Rotation(poolStick, Vector3.forward * -45, 1f, Ease.OutSine);
        }

        private IEnumerator ReturnHands(bool? left = null)
        {
            Tween.StopAll(poolStick);

            var leftHandDest = transform.position + Vector3.right * 2f;
            var rightHandDest = transform.position + Vector3.left * 2f;

            if (Vector2.Distance(leftHandTransform.position, leftHandDest) > returnHandsMinSoundDistance ||
                Vector2.Distance(rightHandTransform.position, rightHandDest) > returnHandsMinSoundDistance)
                RuntimeManager.CreateInstance(returnHandsEvent).start();

            bool doRight = !left.HasValue || !left.Value;
            bool doLeft = !left.HasValue || left.Value;
            if (doLeft)
            {
                Tween.StopAll(leftHand);
                leftHand.SetHand(HandType.Idle);
            }

            if (doRight)
            {
                Tween.StopAll(rightHand);
                rightHand.SetHand(HandType.Idle);
            }
            var sequence = Sequence.Create();
            if (doLeft)
            {
                sequence.Group(Tween.Position(leftHandTransform, leftHandDest, 1f, Ease.InOutQuad));
                sequence.Group(Tween.Rotation(leftHandTransform, Quaternion.identity, 1f, Ease.InOutQuad));
            }
            if (doRight)
            {
                sequence.Group(Tween.Position(rightHandTransform, rightHandDest, 1f, Ease.InOutQuad));
                sequence.Group(Tween.Rotation(rightHandTransform, Quaternion.identity, 1f, Ease.InOutQuad));
            }
            if (!left.HasValue)
            {
                ReturnPoolStick();
            }
            yield return sequence.ToYieldInstruction();
        }


        private IEnumerator FreakOutCoroutine()
        {
            yield return ReturnHands();
            handsIdleSine.StartMovement();
        }
        #region < == SHOT WHITE BALL STATE == >

        private IEnumerator ShotWhiteBallCoroutine(CoState<string, string> state)
        {
            // Play pool shot sound
            poolStick.parent = null;
            handsIdleSine.StopMovement();
            health.Defense = vulnerableDefense;
            float nextStep = 0;
            var player = Game.Instance.World.Player.transform;

            Vector2 dir = player.position - _currentBall.transform.position;
            dir.Normalize();

            // Move stick to ball
            Tween.Position(poolStick, _currentBall.position - dir * 1.6f, 1, Ease.OutCubic);
            Tween.Rotation(poolStick, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x) * Vector3.forward, 0.9f, Ease.OutCubic);
            RuntimeManager.CreateInstance(moveToShot).start();

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
            leftHand.SetOrder(3);
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

            RuntimeManager.CreateInstance(poolShotEvent).start();
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
            Game.Instance.Sound.BossMusic.With(b => b.SetAggressive());

            shotBallInstance.start();

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

            // poolShotSound.Play();

            // Apply the knockback tween to the pool stick and hands
            Tween.Position(poolStick, poolStick.position - (Vector3)dir, .5f, Ease.OutCubic);
            Tween.Position(rightHandTransform, rightHandTransform.position - (Vector3)dir, .5f, Ease.OutCubic);
            health.Defense = attackingDefense;

            yield return new WaitForSeconds(0.5f);
            rightHandTransform.SetParent(transform);

            // Return the stick and hands to the rest position

            Game.Instance.Sound.BossMusic.With(b => b.SetKidding());
            yield return ReturnHands();
            handsIdleSine.StartMovement();

            // Wait until all balls are still or the wait time elapsed
            nextStep += shotBallWaitTime;
            yield return new WaitWhile(() =>
                state.timer.Elapsed < nextStep);

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

        #region < == STOMP PLAYER STATE == >

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
                yield return new WaitForSeconds(.05f);
                poolStickShadow.position = poolStick.position + Vector3.down * poolStickStompDistance;
                var tween = Tween
                    .PositionY(poolStick, poolStick.position.y - poolStickStompDistance, stompTween);
                yield return tween.ToYieldInstruction();
                yield return new WaitForEndOfFrame();
                yield return new WaitForFixedUpdate();
                tween.Complete();
                poolStick.position = poolStickShadow.position;
                stompHitbox.transform.position = poolStick.position;
                stompHitInstance.start();

                Game.Instance.Camera.ShakeCamera(i <= 0 ? stompStuckCameraShake
                                                                       : stompCameraShake);

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
                    Game.Instance.Camera.ShakeCamera(stompCameraShake);
                    yield return Tween
                        .PositionY(poolStick, poolStick.position.y + poolStickStompDistance, .5f, Ease.OutCubic)
                        .ToYieldInstruction();
                }
            }
        }

        private IEnumerator StompPlayerCoroutine(CoState<string, string> state)
        {
            Game.Instance.Sound.BossMusic.With(b => b.SetAggressive());
            poolStick.parent = null;

            leftHand.SetHand(HandType.HoldingStomp);
            rightHand.SetHand(HandType.HoldingStomp);
            rightHand.SetOrder(5);
            leftHand.SetOrder(5);
            leftHandTransform.right = Vector3.left;
            rightHandTransform.right = Vector3.left;
            var playerTransform = Game.Instance.World.Player.transform;
            poolStickShadow.gameObject.SetActive(true);
            for (int i = 0; i < 3; i++)
            {
                yield return AttemptStompPlayer(playerTransform);
            }

            poolStickShadow.gameObject.SetActive(false);
            leftHandTransform.right = Vector3.right;
            rightHandTransform.right = Vector3.right;
            leftHand.SetHand(HandType.Idle);
            rightHand.SetHand(HandType.Idle);
            leftHandTransform.SetParent(transform);
            rightHandTransform.SetParent(transform);

            yield return ReturnHands();

            rightHand.SetHand(HandType.Idle);
            leftHand.SetHand(HandType.Idle);


            state.fsm.StateCanExit();
        }

        #endregion

        #region <== POPULATE BALLS STATE == >

        private async UniTask PopulateBall(Transform handTransform)
        {
            var pos = new Vector2(
                Random.Range(minBallPos.x, maxBallPos.x),
                Random.Range(minBallPos.y, maxBallPos.y));
            var ball = Instantiate(ballPrefab, handTransform);
            ball.transform.localPosition = Vector3.zero;
            var sb = ball.GetComponent<SnookerBall>();
            var rb = ball.GetComponent<Rigidbody2D>();
            rb.simulated = false;

            sb.SetShadowLocalPos(Vector2.down * 3);

            RuntimeManager.CreateInstance(moveHandEvent).start();
            await Tween.Position(handTransform, pos + Vector2.up * 3, 1, Ease.InOutSine);

            sb.DetachShadow();
            await Tween.Position(ball.transform, pos, .75f, Ease.InQuint);
            sb.AttachShadow();

            rb.simulated = true;
            ball.transform.SetParent(null);
            sb.SetShadowLocalPos(Vector3.zero);
            availableBalls = availableBalls.Append(rb).ToArray();
        }

        private async UniTask PopulateBalls(bool isLeft, int count)
        {
            for (var i = 0; i < count; i++)
            {
                await PopulateBall(isLeft ? leftHandTransform : rightHandTransform);
            }

            await ReturnHands(isLeft).ToUniTask();
        }

        private IEnumerator PopulateBallsCoroutine(CoState<string, string> state)
        {
            yield return ReturnHands();
            poolStick.parent = null;

            leftHand.SetHand(HandType.HoldingBall);
            rightHand.SetHand(HandType.HoldingBall);
            var availableBallCount = availableBalls.Count(b => b);
            float count = (_currentBallCount - availableBallCount) / 2f;

            int leftCount = Mathf.FloorToInt(count);
            int rightCount = Mathf.CeilToInt(count);

            yield return UniTask.WhenAll(
                PopulateBalls(true, leftCount),
                PopulateBalls(false, rightCount)
            ).ToCoroutine();

            leftHand.SetHand(HandType.Idle);
            rightHand.SetHand(HandType.Idle);
            state.fsm.StateCanExit();
        }

        #endregion

        private void FixedUpdate()
        {
            fsm.OnLogic();
        }
    }
}