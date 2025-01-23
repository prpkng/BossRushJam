namespace BRJ.Systems.Common
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class TransitionToScene : MonoBehaviour
    {
        public void Transition(string destination)
        {
            SceneManager.LoadScene(destination);
        }
    }
}