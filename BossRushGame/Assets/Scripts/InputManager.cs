using System;
using Game.Player;
using Pixelplacement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Game
{
    public class InputManager : Singleton<InputManager>
    {
        [SerializeField] private PlayerInput playerInputComponent;

        private static Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
            playerInputComponent.onActionTriggered += OnActionTriggered;
        }

        public static Vector2 MousePosition
        {
            get
            {
                Vector2 viewportPos = Mouse.current.position.ReadValue() /
                         new Vector2(Screen.width, Screen.height);
                float scale = GameManager.Instance.ScreenRenderTexture.localScale.x;
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

        public static bool isUsingGamepad;

        private void OnActionTriggered(InputAction.CallbackContext ctx)
        {
            isUsingGamepad = ctx.control.device is Gamepad;

            switch (ctx.action.name)
            {
                case "Move":
                    MoveVector = ctx.ReadValue<Vector2>();
                    break;
                case "Look":
                    LookVector = ctx.ReadValue<Vector2>();
                    break;
                case "Roll" when ctx.started:
                    RollPerformed?.Invoke();
                    break;
                case "Shoot" when ctx.started:
                    ShootPerformed?.Invoke(true);
                    break;
                case "Shoot" when ctx.canceled:
                    ShootPerformed?.Invoke(false);
                    break;
            }
        }
    }
}
