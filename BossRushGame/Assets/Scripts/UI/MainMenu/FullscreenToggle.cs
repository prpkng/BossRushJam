namespace BRJ.UI.MainMenu
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class FullscreenToggle : MonoBehaviour, ISubmitHandler
    {
        public Image checkmark;

        private void Start()
        {
            UpdateCheckbox();
        }

        public void UpdateCheckbox()
        {
            checkmark.enabled = Screen.fullScreen;
        }

        public void ToggleFullscreen()
        {
            Screen.fullScreen = !Screen.fullScreen;
            UpdateCheckbox();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            ToggleFullscreen();
        }
    }
}