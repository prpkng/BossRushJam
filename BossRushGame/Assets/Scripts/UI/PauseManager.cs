namespace BRJ.UI
{
    using Pixelplacement;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class PauseManager : MonoBehaviour
    {
        public GameObject resumeButton;

        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(resumeButton);
        }

        public void ResetPause()
        {
        }


        public void OpenSettings()
        {

        }

        public void GiveUp()
        {
            Game.Instance.SetPaused(false);
            Game.Instance.World.PlayerDeath();
        }
    }
}