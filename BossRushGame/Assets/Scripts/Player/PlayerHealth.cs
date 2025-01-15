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
            base.OnDeath();
            GameManager.Instance.PlayerDeath();
        }
    }
}