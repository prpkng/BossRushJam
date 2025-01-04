namespace Game
{
    using Game.Bosses;
    using Game.Player;
    using Game.Systems;
    using Pixelplacement;
    using UnityEngine;

    public class GameManager : Singleton<GameManager>
    {
        public PlayerManager Player;
        public Maybe<BossBarController> BossBarController;

        [SerializeField] private GameObject bossUIControllerPrefab;

        public void CreateBossBar()
        {
            var obj = Instantiate(bossUIControllerPrefab);
            BossBarController = new(obj.GetComponent<BossBarController>());
        }
    }
}