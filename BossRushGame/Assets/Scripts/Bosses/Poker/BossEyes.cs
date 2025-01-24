namespace BRJ.Bosses.Joker
{
    using UnityEngine;

    public class BossEyes : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        public void SetNormal()
        {
            animator.Play("Normal");
        }

        public void SetAngry()
        {
            animator.Play("Angry");
        }
    }
}