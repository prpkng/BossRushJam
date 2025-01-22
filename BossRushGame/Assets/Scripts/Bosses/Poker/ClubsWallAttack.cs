using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

namespace BRJ.Bosses.Poker
{
    public class ClubsWallAttack : MonoBehaviour, ICardAttack
    {
        private Transform bulletPrefab;
        private List<Transform> bullets;
        
        private const float xPos = 8;
        private const float yMin = -2;
        private const float yMax = 10;

        private const int WallShootingCount = 5;
        private const float EachWallDelay = .75f;
        private const float TweenMoveDuration = 1.5f;
        private const float FinishDelay = .25f;


        private void Awake()
        {
            var op = Addressables.LoadAssetAsync<GameObject>("Prefabs/ClubsBullet.prefab");
            op.Completed += handle =>
            {
                bulletPrefab = handle.Result.transform;
            };
        }

        private async void Burst() {
            for (int i = 0; i < WallShootingCount; i++)
            {
                Shoot();
                await UniTask.WaitForSeconds(EachWallDelay);
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
                    TweenMoveDuration,
                    Ease.OutCubic
                ));
            }

            await sequence;

            bullets.ForEach(b => b.GetComponent<ClubsBullet>().enabled = true);
            
        }

        public float StartAttack()
        {
            Burst();
            return EachWallDelay * WallShootingCount + TweenMoveDuration + FinishDelay;
        }

        public void StopAttack()
        { }
    }
}