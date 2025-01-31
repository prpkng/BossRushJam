namespace BRJ.UI.MainMenu
{
    using BRJ.Systems.Saving;
    using UnityEngine;

    public class ResetButton : MonoBehaviour
    {
        public void OnClick()
        {
            SaveManager.DeleteSave();
        }
    }
}