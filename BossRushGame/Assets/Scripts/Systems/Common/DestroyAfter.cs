namespace BRJ.Systems.Common
{
    using System.Collections;
    using PrimeTween;
    using UnityEngine;

    public class DestroyAfter : MonoBehaviour
    {
        public float seconds;
        public bool runOnAwake;

        private void Start()
        {
            if (runOnAwake)
                Destroy(gameObject, seconds);
        }

        public void Destroy() => Destroy(seconds);
        public void Destroy(float seconds)
        {
            Destroy(gameObject, seconds);
        }
    }
}