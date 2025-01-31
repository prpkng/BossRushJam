namespace BRJ.UI.MainMenu
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class SettingsManager : MonoBehaviour
    {
        public GameObject obj;
        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(obj);
        }
    }
}