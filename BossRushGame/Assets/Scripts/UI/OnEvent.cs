namespace BRJ.UI {
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class OnEvent : MonoBehaviour, IPointerEnterHandler, ISelectHandler
    {
        public UnityEvent hover;
        public void OnPointerEnter(PointerEventData eventData)
        {
            hover.Invoke();
        }
        public UnityEvent selected;
        public void OnSelect(BaseEventData eventData)
        {
            selected.Invoke();
        }
    }
}