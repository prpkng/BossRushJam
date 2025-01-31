namespace BRJ.UI.MainMenu
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class MenuManager : MonoBehaviour
    {
        public GameObject defaultSelected;

        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(defaultSelected);
        }
    }
}