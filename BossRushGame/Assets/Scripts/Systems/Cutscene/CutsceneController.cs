namespace BRJ.Systems.Cutscene
{
    using BRJ.Systems.Common;
    using Cysharp.Threading.Tasks;
    using FMODUnity;
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.SceneManagement;

    public class CutsceneController : MonoBehaviour
    {
        private static CutsceneController _instance;
        private void Awake()
        {
            _instance = this;
        }

        public static void StartCutscene() => startCutsceneEvent?.Invoke();
        private static System.Action startCutsceneEvent;

        public static void SetDestination(string destination)
        {
            if (_instance)
            {
                _instance.destination = destination;
            }
        }

        public StudioEventEmitter bgmEvent;
        public PlayableDirector director;
        public TransitionToScene transition;
        public string destination = "Poker";
        public bool startDisabled = false;

        private void OnEnable()
        {
            if (startDisabled)
                startCutsceneEvent += Enable;
            else
                Enable();
        }

        public void Enable()
        {
            print("StartObject fired!");
            InputManager.RollPerformed += Transition;
            startCutsceneEvent = null;
            director.Play();
        }

        private void OnDisable()
        {
            InputManager.RollPerformed -= Transition;
        }

        public async void Transition()
        {
            var scene = SceneManager.GetActiveScene();
            if (bgmEvent)
            {
                Destroy(bgmEvent.gameObject);
                UniTask.Void(async () =>
                {
                    await UniTask.WaitForEndOfFrame();
                    bgmEvent.Stop();
                });
            }
            InputManager.RollPerformed -= Transition;
            await transition.TransitionAsync(destination);
            Game.Instance.World.RenderTextureZoom = 1.5f;
            if (scene.isSubScene)
                await SceneManager.UnloadSceneAsync(scene);
        }
    }
}