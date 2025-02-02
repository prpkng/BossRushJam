namespace BRJ.Systems.Common
{
    using UnityEngine;
    using UnityEngine.Events;

    public class OnCreate : MonoBehaviour
    {
        public UnityEvent @event;

        private void Start()
        {
            @event.Invoke();
        }
    }
}