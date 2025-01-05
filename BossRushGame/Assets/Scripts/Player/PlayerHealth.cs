using Game.Systems;
using UnityEngine;

namespace Game.Player
{
    public class PlayerHealth : HealthBehavior
    {
        public override void OnDeath()
        {
            print("PLAYER DEAD!!");
            base.OnDeath();
        }
    }
}