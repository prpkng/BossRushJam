using System.Collections;
using BRJ.Systems;
using BRJ.Systems.Common;
using PrimeTween;
using UnityEngine;

namespace BRJ.Player
{
    public class PlayerGun : GunBehavior
    {
        [Header("Properties")] public float bulletDamage;
        public float bulletForce;

        [Tooltip("Bullet per Second")] public float fireRate = 3;
        public float bulletSpreadMin = 1f;
        public float bulletSpreadMax = 1f;

        public float bulletRecoil;

        [Header("Visual")][SerializeField] private TweenSettings<float> gunRecoilSettings;

        [Header("References")]
        [SerializeField]
        private FMODUnity.StudioEventEmitter fireEventEmitter;

        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator gunAnimator;

        private new Camera camera;

        public bool IsHoldingFire { get; private set; }
        private float _fireRateCounter;

        private void Awake()
        {
            camera = Camera.main;
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
                _lastPointVector = (Vector3)InputManager.MousePosition - transform.position;
            }

            return _lastPointVector.normalized;
        }

        private Vector3 _temp;

        private void Update()
        {
            if (Game.Instance.Paused) return;
            var lookDirection = GetPointVector();

            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            spriteRenderer.flipY = lookDirection.x < 0;

            _temp = Game.Instance.World.Player.playerSprite.transform.localScale;
            _temp.x = lookDirection.x < 0 ? -1 : 1;
            Game.Instance.World.Player.playerSprite.transform.localScale = _temp;


            _fireRateCounter -= Time.deltaTime;
            if (IsHoldingFire && _fireRateCounter < 0)
                TriggerShoot();
        }

        private void OnEnable()
        {
            OnPlayerFire(InputManager.isHoldingShoot);
            InputManager.ShootPerformed += OnPlayerFire;
        }

        private void OnDisable()
        {
            OnPlayerFire(false);
            InputManager.ShootPerformed -= OnPlayerFire;
        }

        public void OnPlayerFire(bool pressed)
        {
            if (pressed & Game.Instance.Paused) return;
            IsHoldingFire = pressed;
            if (pressed && _fireRateCounter < 0)
                TriggerShoot();
        }

        private Tween recoilTween;
        private Tween playerRecoilTween;

        public event System.Action ShootTriggered;

        private void TriggerShoot()
        {
            ShootTriggered?.Invoke();
            var direction = transform.right;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle += Random.Range(bulletSpreadMin, bulletSpreadMax) * (Random.value > .5f ? -1 : 1);
            FireBullet(bulletPrefab, Utilities.FromDegrees(angle), bulletForce);
            _fireRateCounter = 1f / fireRate;
            if (bulletRecoil > .1f)
            {
                Game.Instance.World.Player.Rb.linearVelocity = -direction * bulletRecoil;
                playerRecoilTween.Complete();
                playerRecoilTween = Tween.Custom(
                    0f,
                    1f,
                    .25f,
                    f => Game.Instance.World.Player.SpeedMultiplier = f,
                    Ease.OutCubic
                );
            }

            gunAnimator.SetTrigger("Shot");
            fireEventEmitter.Play();

            recoilTween.Complete();
            recoilTween = Tween.LocalPositionX(gunAnimator.transform, gunRecoilSettings);
        }
    }
}