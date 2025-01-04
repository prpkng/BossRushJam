using System.Collections;
using Game.Systems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{

    public class PlayerGun : GunBehavior
    {
        [Header("Properties")]
        public float bulletForce;

        [Tooltip("Bullet per Second")]
        public float fireRate = 3;
        [Header("References")]
        [SerializeField] private GameObject bulletPrefab;

        private new Camera camera;
        private SpriteRenderer spriteRenderer;

        private bool _isHoldingFire;
        private float _fireRateCounter;

        private void Awake()
        {
            camera = Camera.main;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private Vector2 _lastPointVector;
        private Vector2 GetPointVector()
        {
            if (InputManager.isUsingGamepad)
            {
                if (InputManager.LookVector.sqrMagnitude > Mathf.Epsilon)
                    _lastPointVector = InputManager.LookVector;
            }
            else
            {
                _lastPointVector =
                camera.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position;
            }
            return _lastPointVector.normalized;
        }

        private void Update()
        {
            var lookDirection = GetPointVector();

            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            spriteRenderer.flipY = lookDirection.x < 0;


            _fireRateCounter -= Time.deltaTime;
            if (_isHoldingFire && _fireRateCounter < 0)
                TriggerShoot();

        }

        private void OnEnable()
        {
            InputManager.ShootPerformed += OnPlayerFire;
        }
        private void OnDestroy()
        {
            InputManager.ShootPerformed -= OnPlayerFire;
        }

        private void OnPlayerFire(bool pressed)
        {
            _isHoldingFire = pressed;
            if (pressed && _fireRateCounter < 0)
                TriggerShoot();
        }

        private void TriggerShoot()
        {
            FireBullet(bulletPrefab, bulletForce);
            _fireRateCounter = 1f / fireRate;
        }

    }
}