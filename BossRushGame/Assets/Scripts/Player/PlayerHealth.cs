using Game.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Player
{
    public class PlayerHealth : HealthBehavior
    {
        public override void OnDeath()
        {
            print("PLAYER DEAD!!");
            base.OnDeath();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}