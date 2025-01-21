namespace BRJ.UI {
    using Pixelplacement;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class PauseManager : MonoBehaviour {
        public GameObject resumeButton;

        public void ResetPause() {
            EventSystem.current.SetSelectedGameObject(resumeButton);
        }


        public void OpenSettings() {

        }

        public void GiveUp() {
            Game.Instance.SetPaused(false);
            Game.Instance.World.PlayerDeath();
        }
    }
}