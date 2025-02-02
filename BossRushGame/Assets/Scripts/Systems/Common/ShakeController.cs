namespace BRJ.Systems.Common
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class ShakeController : MonoBehaviour
    {
        public float startDelay = 0f;
        public float lowForce = 0f;
        public float highForce = 0f;
        public float duration = .1f;

        public async void StartShake()
        {
            await UniTask.WaitForSeconds(startDelay);
            InputManager.StartShake(lowForce, highForce);
        }

        public void StopShake()
        {
            InputManager.StopShake();
        }

        public async void Shake()
        {
            await UniTask.WaitForSeconds(startDelay);
            InputManager.Shake(lowForce, highForce, duration);
        }
    }
}