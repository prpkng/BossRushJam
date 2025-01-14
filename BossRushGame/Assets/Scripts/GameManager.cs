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

        private PlayerManager _player;
        public PlayerManager Player
        {
            get => _player;
            set
            {
                PlayerTransform = value.transform;
                _player = value;
            }
        }

        public Transform PlayerTransform { get; private set;}
        public Vector3 PlayerPosition => PlayerTransform.position;
        
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

        private void Start()
        {
            CurrentActiveModifier?.ApplyAdvantage();
            CurrentActiveModifier?.ApplyDownside();
        }

        public void CreateBossBar()
        {
            var obj = Instantiate(bossUIControllerPrefab);
            BossBarController = new(obj.GetComponent<BossBarController>());
        }
    }
}