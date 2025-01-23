using System;
using BRJ.Systems;
using UnityEngine;

namespace BRJ.Bosses.Poker
{
    public class SpadesBullet : MonoBehaviour
    {
        public ParabolaMovement movement;
        public float moveDisplacementForce = 1.2f;
        private void Start()
        {
            Vector2 dest = WorldManager.PlayerPosition;
            if (Game.Instance.World.Player.IsMoving)
            {
                var add = Game.Instance.World.Player.movementSpeed * moveDisplacementForce
                * InputManager.MoveVector;
                print($"Player is moving, adding: {add}");
                dest += add;
            }
            dest += Vector2.up * .5f;

            movement.StartTrajectory(dest).GetAwaiter().OnCompleted(() =>
            {
                if (!this) return;
                Destroy(gameObject);
            });
        }
    }
}