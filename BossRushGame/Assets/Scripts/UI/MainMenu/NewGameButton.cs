namespace BRJ.UI
{
    using BRJ.Systems.Cutscene;
    using BRJ.Systems.Saving;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public class NewGameButton : MonoBehaviour
    {
        public Image textImage;

        public Sprite newGameText;
        public Sprite continueText;

        private void OnEnable()
        {
            textImage.sprite = SaveManager.HasSave() ? continueText : newGameText;
        }

        public void OnClick()
        {
            if (SaveManager.HasSave())
                SaveManager.LoadData();
            else
                SaveManager.SaveData();

            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            CutsceneController.StartCutscene();
        }
    }
}