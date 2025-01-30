namespace BRJ.Systems.Common
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class TransitionToScene : MonoBehaviour
    {
        public bool animateTransition = false;
        public void Transition(string destination)
        {
            if (animateTransition)
                Game.Instance.Transition.TransitionToScene(destination);
            else
                SceneManager.LoadScene(destination);
        }
    }
}