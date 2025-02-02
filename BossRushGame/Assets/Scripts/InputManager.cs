using System;
using System.Threading.Tasks;
using BRJ.Player;
using BRJ.Systems;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace BRJ
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInputComponent;

        private static Camera mainCamera;
        private InputAction lookAction;
        private InputAction moveAction;

        private void OnEnable()
        {
            mainCamera = Camera.main;
            playerInputComponent.onActionTriggered += OnActionTriggered;
            lookAction = playerInputComponent.actions["Look"];
            moveAction = playerInputComponent.actions["Move"];
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            playerInputComponent.onActionTriggered -= OnActionTriggered;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private async void OnSceneLoaded(Scene _, LoadSceneMode mode)
        {
            if (mode != LoadSceneMode.Single)
                return;
            mainCamera = Camera.main;


            // Reload player input component
            playerInputComponent.enabled = false;
            await UniTask.NextFrame();
            playerInputComponent.enabled = true;
        }

        public static void ShakeWeak()
        {
            Shake(0f, 0.75f, .025f);
        }

        public static void ShakeWeakLeft()
        {
            Shake(0.45f, 0f, .025f);
        }

        public static void ShakeStrong()
        {
            Shake(1f, 0.5f);
        }

        private static (float l, float r) motorSpeeds = (0, 0);

        public static void StartShake(float lowForce, float highForce)
        {
            if (!isUsingGamepad) return;
            if (lowForce != 0) motorSpeeds.l = lowForce;
            if (highForce != 0) motorSpeeds.r = highForce;
            Gamepad.current.SetMotorSpeeds(motorSpeeds.l, motorSpeeds.r);
        }

        public static void StopShake()
        {
            Gamepad.current.SetMotorSpeeds(0, 0);
        }

        public static async void Shake(float lowForce, float highForce, float duration = .1f)
        {
            if (!isUsingGamepad) return;
            if (lowForce != 0) motorSpeeds.l = lowForce;
            if (highForce != 0) motorSpeeds.r = highForce;
            Gamepad.current.SetMotorSpeeds(motorSpeeds.l, motorSpeeds.r);
            await UniTask.WaitForSeconds(duration);
            if (lowForce != 0) motorSpeeds.l = 0;
            if (highForce != 0) motorSpeeds.r = 0;
            Gamepad.current.SetMotorSpeeds(motorSpeeds.l, motorSpeeds.r);
        }

        public static void ShakeFadeout(float duration = 1f)
        {
            if (!isUsingGamepad) return;
            Tween.Custom(
                Gamepad.current,
                1,
                0,
                duration,
                (pad, f) =>
                {
                    pad.SetMotorSpeeds(1f * f, 0.5f * f);
                },
                Ease.OutSine
            ).OnComplete(Gamepad.current, pad => pad.SetMotorSpeeds(0, 0));
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Keyboard.current.rKey.wasPressedThisFrame)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
#endif

            var look = lookAction.ReadValue<Vector2>();
            var move = moveAction.ReadValue<Vector2>();
            LookVector = look.sqrMagnitude <= 0 && move.sqrMagnitude > 0 ? move / 2.5f : look;
        }

        public static Vector2 MousePosition
        {
            get
            {
                Vector2 viewportPos = Mouse.current.position.ReadValue() /
                         new Vector2(Screen.width, Screen.height);
                float scale = Game.Instance.World.ScreenRenderTexture.localScale.x;
                var range = new Vector2(0.5f - 0.5f / scale, 0.5f + 0.5f / scale);
                viewportPos = new Vector2(
                    Mathf.Lerp(range.x, range.y, viewportPos.x),
                    Mathf.Lerp(range.x, range.y, viewportPos.y)
                );
                return mainCamera.ViewportToWorldPoint(viewportPos); ;
            }
        }

        public static Vector2 MoveVector;
        public static Vector2 LookVector;

        public static event Action RollPerformed;
        public static event Action<bool> ShootPerformed;
        public static event Action PausePressed;
        public static bool isHoldingShoot;

        public static bool isUsingGamepad;
        public static bool isPsGamepad;
        public static event Action InputChanged;


        private void OnActionTriggered(InputAction.CallbackContext ctx)
        {
            var lastUsingGamepad = isUsingGamepad;
            var lastPsGamepad = isPsGamepad;
            isUsingGamepad = ctx.control.device is Gamepad;
            isPsGamepad = ctx.control.device is DualShockGamepad;
            if (isUsingGamepad != lastUsingGamepad || isPsGamepad != lastPsGamepad)
                InputChanged?.Invoke();

            switch (ctx.action.name)
            {
                case "Move":
                    MoveVector = ctx.ReadValue<Vector2>();
                    break;
                case "Roll" when ctx.started:
                    RollPerformed?.Invoke();
                    break;
                case "Shoot" when ctx.started:
                    ShootPerformed?.Invoke(true);
                    isHoldingShoot = true;
                    break;
                case "Shoot" when ctx.canceled:
                    ShootPerformed?.Invoke(false);
                    isHoldingShoot = false;
                    break;
                case "Pause" when ctx.started:
                    PausePressed?.Invoke();
                    break;
            }
        }
    }
}
