using System.Collections;
using BRJ.Systems;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BRJ.Bosses.Poker
{
    public abstract class CardAttack : MonoBehaviour
    {
        protected Transform bulletPrefab;

        public float rotatingSpeed = 15f;
        public float fireRate = 2;
        public abstract bool FaceDirection { get; }
        public abstract void GetBulletPrefab();

        private void Awake()
        {
            GetBulletPrefab();
        }

        private void Update()
        {
            if (!FaceDirection) return;
            Vector2 dir = Game.Instance.World.Player.transform.position - transform.position;
            dir.Normalize();
            float a = Mathf.Atan2(-dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.Euler(0, 180, a + 90f),
                Time.deltaTime * rotatingSpeed
            );
        }

        private IEnumerator Start()
        {
            while (true)
            {
                if (!this) yield break;
                yield return new WaitForSeconds(1f / fireRate);
                if (!bulletPrefab) continue;
                Fire();
            }
        }

        private void Fire()
        {
            var bullet = Instantiate(bulletPrefab, transform.position + transform.up * 1.5f, Quaternion.identity);
            if (FaceDirection) bullet.right = transform.up;
        }
    }
}