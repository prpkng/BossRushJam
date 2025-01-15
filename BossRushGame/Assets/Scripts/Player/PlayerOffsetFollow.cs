using System;
using UnityEngine;

namespace Game.Player
{
    public class PlayerOffsetFollow : MonoBehaviour
    {
        public Vector2 offsetMultiplier = Vector2.one;


        private void FixedUpdate()
        {
            transform.position = GameManager.Instance.PlayerPosition * offsetMultiplier;
        }
    }
}