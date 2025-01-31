namespace BRJ.Systems.Common
{
    using UnityEngine;
    using UnityEngine.Events;

    public class PlayerTrigger : MonoBehaviour
    {
        public UnityEvent unityEvent;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                unityEvent.Invoke();
            }
        }
    }
}