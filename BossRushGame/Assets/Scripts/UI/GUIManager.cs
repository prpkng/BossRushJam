namespace BRJ.UI {
    using Pixelplacement;
    using UnityEngine;
    
    public class GUIManager : MonoBehaviour {
        public DisplayObject playerHud;
        public DisplayObject pauseMenu;
        public PauseManager pauseManager;

        private void Start() {
            playerHud.SetActive(true);
            pauseMenu.SetActive(false);
        }

        private void OnEnable() {
            InputManager.PausePressed += PausePressed;
        }

        private void OnDisable() {
            InputManager.PausePressed -= PausePressed;
        }

        public void PausePressed() {
            if (pauseMenu.ActiveSelf) {
                pauseMenu.SetActive(false);
                Game.Instance.SetPaused(false);
            } else {
                pauseManager.ResetPause();
                pauseMenu.SetActive(true);
                Game.Instance.SetPaused(true);
            }
        }
    }
}