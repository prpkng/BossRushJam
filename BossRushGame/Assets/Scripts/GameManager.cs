namespace Game
{
    using Game.Bosses;
    using Game.Player;
    using Game.Systems;
    using Pixelplacement;
    using UnityEngine;
    using UnityEngine.UI;

    public class GameManager : Singleton<GameManager>
    {
        public Transform ScreenRenderTexture;
        public PlayerManager Player;
        public Maybe<BossBarController> BossBarController;

        [SerializeField] private GameObject bossUIControllerPrefab;


        public float RenderTextureZoom
        {
            get => ScreenRenderTexture.transform.localScale.x;
            set => ScreenRenderTexture.transform.localScale = Vector2.one * value;
        }

        public void CreateBossBar()
        {
            var obj = Instantiate(bossUIControllerPrefab);
            BossBarController = new(obj.GetComponent<BossBarController>());
        }
    }
}