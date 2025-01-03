using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInputComponent;

        private void Awake()
        {
            playerInputComponent.onActionTriggered += OnActionTriggered;
        }

        public static Vector2 MoveVector;

        public static event Action RollPerformed;

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
            }
        }
    }
}
