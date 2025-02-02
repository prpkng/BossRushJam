namespace BRJ.Systems
{
    using Cysharp.Threading.Tasks;
    using FMODUnity;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class TransitionManager : MonoBehaviour
    {
        public Animator animator;
        public float fadeInDuration = 1.5f;
        public EventReference transitionSound;

        public async UniTask TransitionToScene(string destination)
        {
            CallFadeIn();
            RuntimeManager.PlayOneShot(transitionSound);
            await UniTask.WaitForSeconds(fadeInDuration, true);
            await SceneManager.LoadSceneAsync(destination);
            await UniTask.WaitForSeconds(.05f, true);
            Time.timeScale = 1f;
            CallFadeOut();
        }

        public void CallFadeIn()
        {
            animator.Play("fadeIn");
        }
        public void CallFadeOut()
        {
            animator.Play("fadeOut");
        }
    }
}