using System;
using System.Threading.Tasks;
using BRJ.Player;
using BRJ.Systems;
using Cysharp.Threading.Tasks;
using Pixelplacement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace BRJ
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInputComponent;

        private static Camera mainCamera;

        private void OnEnable()
        {
            mainCamera = Camera.main;
            playerInputComponent.onActionTriggered += OnActionTriggered;
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

        private void Update()
        {
            if (Keyboard.current.rKey.wasPressedThisFrame)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            if (Keyboard.current.f6Key.wasPressedThisFrame)
                SceneManager.LoadScene("TheHand");
            if (Keyboard.current.f7Key.wasPressedThisFrame)
                SceneManager.LoadScene("Joker");
            if (Keyboard.current.f8Key.wasPressedThisFrame)
                SceneManager.LoadScene("Lobby");
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
        public static bool isHoldingShoot;

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
                    isHoldingShoot = true;
                    break;
                case "Shoot" when ctx.canceled:
                    ShootPerformed?.Invoke(false);
                    isHoldingShoot = false;
                    break;
            }
        }
    }
}
