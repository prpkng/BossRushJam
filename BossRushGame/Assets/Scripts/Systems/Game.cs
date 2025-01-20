namespace BRJ.Systems
{
    using LDtkUnity;
    using UnityEngine;

    public class Game : MonoBehaviour
    {
        public static Game Instance { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetInstance() {
            Instance = null;
        }

        public WorldManager World { get; private set; }
        public InputManager Input { get; private set; }

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
        }
    }
}