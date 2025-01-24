using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BRJ.Bosses.Poker
{
    public class CardAttackDiamonds : ShootingAttack, ICardAttack
    {
        public override bool FaceDirection => false;

        public override float AttackDuration => 4f;

        public override void GetBulletPrefab()
        {
            Addressables.LoadAssetAsync<GameObject>("Prefabs/DiamondBullet.prefab").Completed +=
                handle => bulletPrefab = handle.Result.transform;
        }

    }
}