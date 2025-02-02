namespace BRJ.UI.MainMenu
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class FullscreenToggle : MonoBehaviour, ISubmitHandler, IPointerClickHandler
    {
        public Image checkmark;

        private void Start()
        {
            UpdateCheckbox();
        }

        public bool IsFullscreen
        {
            get
            {
                return Screen.fullScreen;
            }

            set
            {
                Screen.fullScreen = value;
            }
        }

        public void UpdateCheckbox()
        {
            checkmark.enabled = IsFullscreen;
        }

        public async void ToggleFullscreen()
        {
            IsFullscreen = !IsFullscreen;
            print($"Fullscreen toggled to {IsFullscreen}");
            await UniTask.NextFrame();
            UpdateCheckbox();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            ToggleFullscreen();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ToggleFullscreen();
        }
    }
}