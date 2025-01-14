using System;
using UnityEngine;

namespace Game.Systems.Visual
{
    public class FixedRotation : MonoBehaviour
    {
        private void Update()
        {
            transform.rotation = Quaternion.identity;
        }
    }
}