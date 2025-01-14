using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Bosses.Poker
{
    public class CardAttackSpades : CardAttack
    {
        public override bool FaceDirection => false;
        public override void GetBulletPrefab()
        {
            Addressables.LoadAssetAsync<GameObject>("Prefabs/SpadesBullet.prefab").Completed +=
                handle => bulletPrefab = handle.Result.transform;
        }
    }
}