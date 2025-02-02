namespace BRJ.UI
{
    using FMODUnity;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UISounds : MonoBehaviour, ISelectHandler, IPointerClickHandler, ISubmitHandler
    {
        public EventReference selectSound;
        public EventReference clickSound;

        public void OnSelect(BaseEventData eventData)
        {
            InputManager.Shake(.25f, 0f, .05f);
            RuntimeManager.PlayOneShot(selectSound);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            InputManager.Shake(0f, 0.75f);
            RuntimeManager.PlayOneShot(clickSound);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            InputManager.Shake(0f, 0.75f);
            RuntimeManager.PlayOneShot(clickSound);
        }
    }
}