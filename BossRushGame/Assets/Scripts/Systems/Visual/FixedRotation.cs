using System;
using UnityEngine;

namespace BRJ.Systems.Visual
{
    public class FixedRotation : MonoBehaviour
    {
        private void Update()
        {
            transform.rotation = Quaternion.identity;
        }
    }
}