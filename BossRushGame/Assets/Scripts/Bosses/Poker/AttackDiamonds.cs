using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Bosses.Poker
{
    public class AttackDiamonds : MonoBehaviour
    {
        private static Transform _diamondBulletPrefab;
        

        public float rotatingSpeed = 25f;

        public float fireRate = 2;


        private void Awake()
        {
            if (!_diamondBulletPrefab)
            {
                Addressables.LoadAssetAsync<GameObject>("Prefabs/DiamondBullet.prefab").Completed +=
                    handle => _diamondBulletPrefab = handle.Result.transform;
            }
        }

        private void Update()
        {
            Vector2 dir = GameManager.Instance.Player.transform.position - transform.position;
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
                if (!_diamondBulletPrefab) continue;
                Fire();
            }
        }

        private void Fire()
        {
            Transform bullet = Instantiate(_diamondBulletPrefab, transform.position + transform.up * 1.5f, Quaternion.identity);
            bullet.right = transform.up;
        }
    }
}