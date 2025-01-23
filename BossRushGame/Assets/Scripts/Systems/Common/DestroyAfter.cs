namespace BRJ.Systems.Common
{
    using System.Collections;
    using PrimeTween;
    using UnityEngine;

    public class DestroyAfter : MonoBehaviour
    {
        public float seconds;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(seconds);
            Destroy(gameObject);
        }
    }
}