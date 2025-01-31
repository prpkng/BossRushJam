namespace BRJ.Systems.Lobby
{
    using System.Linq;
    using UnityEngine;

    public class TutorialDoor : MonoBehaviour
    {
        public Animator animator;
        public Collider2D[] targets;
        public Collider2D doorCollider;

        private void Start()
        {
            InvokeRepeating(nameof(CheckTargets), 0, .1f);
        }

        private void CheckTargets()
        {
            if (targets.Any(t => t.enabled)) return;
            animator.Play("Open");
            doorCollider.enabled = false;
        }
    }
}