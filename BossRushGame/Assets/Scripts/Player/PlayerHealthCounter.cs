namespace BRJ.Player
{
    using BRJ.Systems;
    using UnityEngine;
    using UnityEngine.UI;

    public class PlayerHealthCounter : MonoBehaviour
    {
        public Image[] hearts;

        private void Start()
        {
            Game.Instance.World.Player.health.OnHealthChanged += OnHealthChanged;
            hearts[5].WithComponent((Animator anim) => anim.Play("Spin"));
        }

        private void OnDisable()
        {
            Game.Instance.World.Player.health.OnHealthChanged -= OnHealthChanged;
        }

        public void OnHealthChanged(float health)
        {
            for (int i = 0; i < 6; i++)
            {
                hearts[i].enabled = i < health;
                if (i == health - 1)
                {
                    hearts[i].WithComponent((Animator anim) => anim.Play("Spin"));
                }
            }
        }
    }
}