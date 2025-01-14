using System;
using Game.Systems;
using UnityEngine;

namespace Game.Bosses.Poker
{
    public class SpadesBullet : MonoBehaviour
    {
        public ParabolaMovement movement;
        private void Start()
        {
            movement.StartTrajectory(GameManager.Instance.PlayerPosition).GetAwaiter().OnCompleted(() =>
            {
                Destroy(gameObject);
            });
        }
    }
}