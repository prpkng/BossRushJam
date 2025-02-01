using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
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

        private const string ClubWallsAttackSfxPath = "event:/BOSSES/Joker/SFX_JokerWallAttack";

        private const float xPos = 16;
        private const float yMin = -5.65f;
        private const float yMax = 10.15f;
        private const float yAddIfLow = .5f;

        private const int WallShootingCount = 6;
        private const float EachWallDelay = 1f;
        private const float TweenMoveDuration = 1.5f;
        private const float FinishDelay = .25f;
        private const int BulletCount = 7;

        private void Awake()
        {
            var op = Addressables.LoadAssetAsync<GameObject>("Prefabs/ClubsBullet.prefab");
            op.Completed += handle =>
            {
                bulletPrefab = handle.Result.transform;
            };
        }

        private async void Burst()
        {
            for (int i = 0; i < WallShootingCount; i++)
            {
                Shoot();
                await UniTask.WaitForSeconds(EachWallDelay);
            }
        }

        private async void Shoot()
        {
            bullets = new List<Transform>();

            var index = Random.Range(1, BulletCount);

            var sequence = Sequence.Create();

            float startX = transform.position.x > 0 ? xPos : -xPos;

            for (int i = 0; i < BulletCount + 1; i++)
            {
                if (i == index) continue;
                var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

                if (startX > 0) bullet.right = Vector3.left;

                float y = Mathf.Lerp(yMin, yMax, i / (float)BulletCount);
                if (i < index)
                    y -= yAddIfLow;
                else if (i > index)
                    y += yAddIfLow;

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

        private EventInstance sfxEvent;

        public float StartAttack()
        {
            sfxEvent = RuntimeManager.CreateInstance(ClubWallsAttackSfxPath);
            sfxEvent.start();
            RuntimeManager.AttachInstanceToGameObject(sfxEvent, gameObject);
            Burst();
            return EachWallDelay * WallShootingCount + TweenMoveDuration + FinishDelay;
        }

        public void StopAttack()
        {
            sfxEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            sfxEvent.clearHandle();
        }

        private void Update()
        {
            sfxEvent.setPitch(Time.timeScale);
        }
    }
}