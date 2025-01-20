using Game.Systems;
using Game.Systems.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Player
{
    public class PlayerHealth : HealthBehavior
    {
        protected override void OnDeath()
        {
            base.OnDeath();
            WorldManager.Instance.PlayerDeath();
        }
    }
}