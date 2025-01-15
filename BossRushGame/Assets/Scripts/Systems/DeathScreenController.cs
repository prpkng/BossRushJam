using System;
using UnityEngine;

namespace Game.Systems
{
    public class DeathScreenController : MonoBehaviour
    {
        public static Vector3 LastCameraPosition;
        public static Vector3 LastPlayerPosition;
        public Transform cameraTransform;
        public Transform deathCapTransform;

        private void Start()
        {
            cameraTransform.position = LastCameraPosition;
            deathCapTransform.position = LastPlayerPosition;
        }
    }
}