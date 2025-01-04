namespace Game
{
    using Game.Player;
    using UnityEngine;

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public PlayerManager Player;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
}