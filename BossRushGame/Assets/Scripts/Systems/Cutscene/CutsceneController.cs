namespace BRJ.Systems.Cutscene
{
    using BRJ.Systems.Common;
    using UnityEngine;

    public class CutsceneController : MonoBehaviour
    {
        public TransitionToScene transition;
        public string destination = "Poker";
        private void OnEnable()
        {
            InputManager.RollPerformed += Transition;
        }

        private void OnDisable()
        {
            InputManager.RollPerformed -= Transition;
        }

        private void Transition()
        {
            transition.Transition(destination);
        }
    }
}