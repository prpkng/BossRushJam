using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{

    public class PlayerGun : GunBehavior
    {
        private new Camera camera;
        private SpriteRenderer spriteRenderer;
        private void Awake()
        {
            camera = Camera.main;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            var lookDirection = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue())
                                - transform.position;
            lookDirection.Normalize();

            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            spriteRenderer.flipY = lookDirection.x < 0;
        }
    }
}