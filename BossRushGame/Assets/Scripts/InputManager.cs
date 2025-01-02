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

        public static Vector2 moveVector;

        private void OnActionTriggered(InputAction.CallbackContext ctx)
        {
            switch (ctx.action.name)
            {
                case "Move":
                    print(ctx.ReadValue<Vector2>());
                    moveVector = ctx.ReadValue<Vector2>();
                    break;
            }
        }
    }
}
