namespace BRJ
{
    using System;
    using BRJ.Player;
    using BRJ.Systems;
    using BRJ.Systems.Saving;
    using BRJ.Systems.Slots.Modifiers;
    using LDtkUnity;
    using UnityEngine;
    using UnityEngine.SceneManagement;

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
        private void Start()
        {
            // #if UNITY_EDITOR
            var currentModifierType = SaveManager.GetCurrentModifierType();
            if (currentModifierType != null)
            {
                World.CurrentActiveModifier = (Modifier)Activator.CreateInstance(currentModifierType);
                print("Current modifier type: " + World.CurrentActiveModifier.GetType());
            }
            // #endif
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }


        /// <summary>
        /// Called when a new level is loaded and data should be initialized in it
        /// </summary>
        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Menu") { Destroy(gameObject); return; }

            World.ApplyModifier();
        }
    }
}