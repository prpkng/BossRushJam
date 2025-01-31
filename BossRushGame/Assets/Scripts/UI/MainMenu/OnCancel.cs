namespace BRJ.UI.MainMenu
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class OnCancel : MonoBehaviour, ICancelHandler
    {
        public UnityEvent @event;

        void ICancelHandler.OnCancel(BaseEventData eventData)
        {
            @event.Invoke();
        }
    }
}