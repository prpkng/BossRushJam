using System;
using Cysharp.Threading.Tasks;
using BRJ.Systems.Slots.Modifiers;
using UnityEngine.SceneManagement;

namespace BRJ
{
    using BRJ.Bosses;
    using BRJ.Player;
    using BRJ.Systems;
    using Pixelplacement;
    using UnityEngine;
    using UnityEngine.UI;

    public class WorldManager : MonoBehaviour
    {
        public Modifier CurrentActiveModifier;
        public static int CurrentLevelId { get; set; } = 1;
        public Transform ScreenRenderTexture;
        public Material RenderTextureMaterial;
        public static Transform PlayerTransform { get; private set; }
        public static Vector3 PlayerPosition => PlayerTransform.position;

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


        public Maybe<BossBarController> BossBarController;

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

        public async void PlayerDeath()
        {
            DeathScreenController.LastCameraPosition = Game.Instance.Camera.transform.position;
            DeathScreenController.LastPlayerPosition = PlayerPosition;

            await SceneManager.LoadSceneAsync("DeathScreen");

            await UniTask.WaitForSeconds(5);

            SceneManager.LoadScene("Spin");
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
        }

        private void SceneLoaded(Scene scene, LoadSceneMode _)
        {
            ScreenRenderTexture.gameObject.SetActive(scene.name != "Spin");
        }
    }
}