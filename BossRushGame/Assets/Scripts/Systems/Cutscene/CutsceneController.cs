namespace BRJ.Systems.Cutscene
{
    using BRJ.Systems.Common;
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.SceneManagement;

    public class CutsceneController : MonoBehaviour
    {
        public static void StartCutscene() => startCutsceneEvent?.Invoke();
        private static System.Action startCutsceneEvent;

        public PlayableDirector director;
        public TransitionToScene transition;
        public string destination = "Poker";
        public bool startDisabled = false;
        private void OnEnable()
        {
            if (startDisabled)
                startCutsceneEvent += Enable;
            else
                Enable();
        }

        public void Enable()
        {
            print("StartObject fired!");
            InputManager.RollPerformed += Transition;
            startCutsceneEvent = null;
            director.Play();
        }

        private void OnDisable()
        {
            InputManager.RollPerformed -= Transition;
        }

        private void Transition()
        {
            var scene = SceneManager.GetActiveScene();
            InputManager.RollPerformed -= Transition;
            transition.Transition(destination);
            if (scene.isSubScene)
                SceneManager.UnloadSceneAsync(scene);
        }
    }
}