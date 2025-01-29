namespace BRJ.UI
{
    using PrimeTween;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class Crosshair : MonoBehaviour
    {
        public float gamepadLookLerpSpeed = 12f;
        public float gamepadLookSpacing;
        public Vector2 playerOffset;
        public new SpriteRenderer renderer;

        public TweenSettings rotationTweenSettings;

        private Tween rotationTween;

        private void OnEnable()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }

        private void Start()
        {
            Game.Instance.World.Player.activeGun.ShootTriggered += ShootPerformed;
        }

        private void OnDisable()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Game.Instance.World.Player.activeGun.ShootTriggered -= ShootPerformed;
        }

        public void ShootPerformed()
        {
            rotationTween.Complete();

            var dir = Random.value > 0.5f ? 1 : -1;

            rotationTween = Tween.Rotation(
                transform,
                transform.eulerAngles,
                transform.eulerAngles + Vector3.forward * (90f * dir),
                rotationTweenSettings
            );
        }

        // TODO Switch this to magnitude lerp


        private float magnitude;
        private Vector2 lastNorm;
        private void Update()
        {
            if (InputManager.isUsingGamepad)
            {
                if (InputManager.LookVector.normalized != Vector2.zero)
                    lastNorm = InputManager.LookVector.normalized;

                magnitude = Mathf.Lerp(
                    magnitude,
                    InputManager.LookVector.magnitude,
                     Time.deltaTime * gamepadLookLerpSpeed
                );
                renderer.color = new Color(1, 1, 1, magnitude);
                transform.localPosition =
                    (Vector2)WorldManager.PlayerPosition + playerOffset
                            + gamepadLookSpacing * (lastNorm * magnitude);
            }
            else
            {
                renderer.color = Color.white;
                transform.localPosition = InputManager.MousePosition;
            }
        }
    }
}