using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BRJ.Bosses.Poker
{
    public class CardAttackDiamonds : CardAttack
    {
        public override bool FaceDirection => true;
        public override void GetBulletPrefab()
        {
            Addressables.LoadAssetAsync<GameObject>("Prefabs/DiamondBullet.prefab").Completed +=
                handle => bulletPrefab = handle.Result.transform;
        }
    }
}