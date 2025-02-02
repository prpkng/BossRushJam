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

            FMODUnity.RuntimeManager.GetVCA("MUS").setVolume(PlayerPrefs.GetFloat("Music", 1f));
            FMODUnity.RuntimeManager.GetVCA("SFX").setVolume(PlayerPrefs.GetFloat("SFX", 1f));
        }
    }
}