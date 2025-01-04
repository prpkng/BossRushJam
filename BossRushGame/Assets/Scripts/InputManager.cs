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

        public static event Action RollPerformed;
        public static event Action ShootPerformed;

        private void OnActionTriggered(InputAction.CallbackContext ctx)
        {
            switch (ctx.action.name)
            {
                case "Move":
                    MoveVector = ctx.ReadValue<Vector2>();
                    break;
                case "Roll" when ctx.started:
                    RollPerformed?.Invoke();
                    break;
                case "Shoot" when ctx.started:
                    ShootPerformed?.Invoke();
                    break;
            }
        }
    }
}
