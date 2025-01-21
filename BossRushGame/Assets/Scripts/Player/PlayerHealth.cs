using BRJ.Systems;
using BRJ.Systems.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BRJ.Player
{
    public class PlayerHealth : HealthBehavior
    {
        protected override void OnDeath()
        {
            base.OnDeath();
            Game.Instance.World.PlayerDeath();
        }
    }
}