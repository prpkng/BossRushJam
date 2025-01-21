namespace BRJ.UI {
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class PauseManager : MonoBehaviour {

        public GameObject resumeButton;

        public void ResetPause() {
            EventSystem.current.SetSelectedGameObject(resumeButton);
        }
    }
}