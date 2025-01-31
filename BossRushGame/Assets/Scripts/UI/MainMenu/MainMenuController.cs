namespace BRJ.UI.MainMenu
{
    using BRJ.Systems.Cutscene;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class MainMenuController : MonoBehaviour
    {
        private void Start()
        {
            SceneManager.LoadSceneAsync("IntroCutscene", LoadSceneMode.Additive);
        }
    }
}