using System;
using Pixelplacement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class InputManager : Singleton<InputManager>
    {
        [SerializeField] private PlayerInput playerInputComponent;

        private void Awake()
        {
            playerInputComponent.onActionTriggered += OnActionTriggered;
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
