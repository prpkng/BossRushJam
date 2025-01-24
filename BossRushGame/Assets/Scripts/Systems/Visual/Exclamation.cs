namespace BRJ.Systems.Visual
{
    using BRJ.UI;
    using UnityEngine;

    public class Exclamation : MonoBehaviour
    {
        private void OnDisable()
        {
            ExclamationUIManager.RemoveExclamation();
        }

        private void OnEnable()
        {
            ExclamationUIManager.SetExclamation(transform.position);
        }
    }
}