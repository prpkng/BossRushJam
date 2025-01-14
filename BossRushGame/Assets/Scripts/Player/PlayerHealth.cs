using Game.Systems;
using Game.Systems.Common;
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
            SceneManager.LoadScene("Spin");
        }
    }
}