namespace BRJ.Systems.Common
{
    using UnityEngine;
    using UnityEngine.Events;

    public class OnEnableEvent : MonoBehaviour
    {
        public UnityEvent @event;

        private void OnEnable()
        {
            @event.Invoke();
        }
    }
}