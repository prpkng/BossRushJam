using System.Collections;
using Game.Systems;
using PrimeTween;
using UnityEngine;

namespace Game.Player
{

    public class PlayerGun : GunBehavior
    {
        [Header("Properties")]
        public float bulletDamage;
        public float bulletForce;
        
        [Tooltip("Bullet per Second")] public float fireRate = 3;

        [Header("Visual")] 
        
        [SerializeField] private TweenSettings<float> gunRecoilSettings;
        
        [Header("References")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator gunAnimator;
        
        private new Camera camera;

        private bool _isHoldingFire;
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
            var lookDirection = GetPointVector();

            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            spriteRenderer.flipY = lookDirection.x < 0;
            
            _temp = GameManager.Instance.Player.playerSprite.transform.localScale;
            _temp.x = lookDirection.x < 0 ? -1 : 1;
            GameManager.Instance.Player.playerSprite.transform.localScale = _temp;


            _fireRateCounter -= Time.deltaTime;
            if (_isHoldingFire && _fireRateCounter < 0)
                TriggerShoot();

        }

        private void OnEnable()
        {
            OnPlayerFire(false);
            InputManager.ShootPerformed += OnPlayerFire;
        }
        private void OnDisable()
        {
            OnPlayerFire(false);
            InputManager.ShootPerformed -= OnPlayerFire;
        }

        private void OnPlayerFire(bool pressed)
        {
            _isHoldingFire = pressed;
            if (pressed && _fireRateCounter < 0)
                TriggerShoot();
        }

        private Tween _recoilTween;
        private void TriggerShoot()
        {
            FireBullet(bulletPrefab, bulletForce);
            _fireRateCounter = 1f / fireRate;
            
            gunAnimator.SetTrigger("Shot");
            
            _recoilTween.Complete();
            _recoilTween = Tween.LocalPositionX(gunAnimator.transform, gunRecoilSettings);
        }

    }
}