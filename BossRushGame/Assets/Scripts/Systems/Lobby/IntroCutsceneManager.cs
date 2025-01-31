namespace BRJ.Systems.Lobby
{
    using BRJ.Systems.Cutscene;
    using UnityEngine;

    public class IntroCutsceneManager : MonoBehaviour
    {
        public static string CutsceneDestinationScene = "Tutorial";

        public CutsceneController cutsceneController;

        private void Start()
        {
            cutsceneController.destination = CutsceneDestinationScene;
        }
    }
}