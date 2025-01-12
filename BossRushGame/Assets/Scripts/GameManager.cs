using System;
using Game.Systems.Slots.Modifiers;
using UnityEngine.SceneManagement;

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
        public static Modifier CurrentActiveModifier;
        public static int CurrentLevelId { get; set; } = 1;
        public Transform ScreenRenderTexture;
        public PlayerManager Player;
        public Maybe<BossBarController> BossBarController;

        [SerializeField] private GameObject bossUIControllerPrefab;
        
        public float RenderTextureZoom
        {
            get => ScreenRenderTexture.transform.localScale.x;
            set => ScreenRenderTexture.transform.localScale = Vector2.one * value;
        }

        private void Awake()
        {
            CurrentLevelId = SceneManager.GetActiveScene().buildIndex;
        }

        public void CreateBossBar()
        {
            var obj = Instantiate(bossUIControllerPrefab);
            BossBarController = new(obj.GetComponent<BossBarController>());
        }
    }
}