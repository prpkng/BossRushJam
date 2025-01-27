namespace BRJ
{
    using BRJ.Player;
    using BRJ.Systems;
    using LDtkUnity;
    using UnityEngine;

    public class Game : MonoBehaviour
    {
        public bool Paused { get; private set; }
        public static Game Instance { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetInstance()
        {
            Instance = null;
        }

        public WorldManager World { get; private set; }
        public InputManager Input { get; private set; }
        public SoundManager Sound { get; private set; }
        public CameraManager Camera { get; private set; }
        public TransitionManager Transition { get; private set; }

        public void SetPaused(bool paused)
        {
            Time.timeScale = paused ? 0 : 1;
            Paused = paused;
            Sound.SetGlobalParameter(SoundManager.PauseAttenuationParam, paused ? 1 : 0);
        }

        public void SetCamera(CameraManager camera)
        {
            Camera = camera;
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            World = GetComponentInChildren<WorldManager>();
            Input = GetComponentInChildren<InputManager>();
            Sound = GetComponentInChildren<SoundManager>();
            Transition = GetComponentInChildren<TransitionManager>();
        }
    }
}