namespace BRJ.Systems.Common
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class TransitionToScene : MonoBehaviour
    {
        public bool animateTransition = false;
        public void Transition(string destination)
        {
            if (animateTransition)
                Game.Instance.Transition.TransitionToScene(destination).Forget();
            else
                SceneManager.LoadScene(destination);
        }
        public async UniTask TransitionAsync(string destination)
        {
            if (animateTransition)
                await Game.Instance.Transition.TransitionToScene(destination);
            else
                await SceneManager.LoadSceneAsync(destination);
        }
    }
}