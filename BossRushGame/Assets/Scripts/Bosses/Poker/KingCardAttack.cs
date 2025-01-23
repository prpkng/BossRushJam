namespace BRJ.Bosses.Poker
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class KingCardAttack : MonoBehaviour
    {

        public float attackWaitTime = .5f;
        public float attackColliderWaitTime = .3f;
        public float swordsAnimationDuration = .5f;
        public GameObject swordsPrefab;
        public new Collider2D collider;

        private async void Start()
        {
            await UniTask.WaitForSeconds(attackWaitTime);
            var swords = Instantiate(swordsPrefab, transform);
            await UniTask.WaitForSeconds(attackColliderWaitTime);

            collider.enabled = true;
            await UniTask.WaitForSeconds(swordsAnimationDuration - attackColliderWaitTime);

            collider.enabled = false;
            Destroy(swords);
        }
    }
}