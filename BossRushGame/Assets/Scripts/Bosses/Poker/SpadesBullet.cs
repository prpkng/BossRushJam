using System;
using BRJ.Systems;
using UnityEngine;

namespace BRJ.Bosses.Poker
{
    public class SpadesBullet : MonoBehaviour
    {
        public ParabolaMovement movement;
        private void Start()
        {
            movement.StartTrajectory(WorldManager.PlayerPosition).GetAwaiter().OnCompleted(() =>
            {
                if (!this) return;
                Destroy(gameObject);
            });
        }
    }
}