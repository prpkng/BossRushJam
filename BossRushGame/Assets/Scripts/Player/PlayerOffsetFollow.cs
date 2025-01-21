using System;
using UnityEngine;

namespace BRJ.Player
{
    public class PlayerOffsetFollow : MonoBehaviour
    {
        public Vector2 offsetMultiplier = Vector2.one;


        private void FixedUpdate()
        {
            transform.position = WorldManager.PlayerPosition * offsetMultiplier;
        }
    }
}