using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

namespace Game.Bosses.Poker
{
    public class ClubsWallAttack : MonoBehaviour
    {
        private Transform bulletPrefab;
        private List<Transform> bullets;
        
        private const float xPos = 8;
        private const float yMin = -2;
        private const float yMax = 8;

        private void Awake()
        {
            var op = Addressables.LoadAssetAsync<GameObject>("Prefabs/ClubsBullet.prefab");
            op.Completed += handle =>
            {
                bulletPrefab = handle.Result.transform;
                Burst();
            };
        }

        private async void Burst() {
            for (int i = 0; i < 3; i++)
            {
                Shoot();
                await UniTask.WaitForSeconds(1f);
            }
        }

        private async void Shoot()
        {
            bullets = new List<Transform>();

            var index = Random.Range(1, 5);

            var sequence = Sequence.Create();

            float startX = transform.position.x > 0 ? xPos : -xPos;
            
            for (int i = 0; i < 6; i++)
            {
                if (i == index) continue;
                var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

                if (startX > 0) bullet.right = Vector3.left;
                
                float y = Mathf.Lerp(yMin, yMax, i / 6f);

                bullets.Add(bullet);
                
                sequence = sequence.Group(Tween.Position(
                    bullet,
                    new Vector3(
                        startX,
                        y),
                    2f,
                    Ease.InOutExpo
                ));
            }

            await sequence;

            bullets.ForEach(b => b.GetComponent<ClubsBullet>().enabled = true);
            
        }
        
    }
}