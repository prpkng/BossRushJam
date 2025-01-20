using System;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

namespace BRJ.Systems
{
    public class DeathScreenController : MonoBehaviour
    {
        public static Vector3 LastCameraPosition;
        public static Vector3 LastPlayerPosition;
        public Transform cameraTransform;
        public Transform deathCapTransform;
        public TweenSettings lerpCameraTween;
        private async void Start()
        {
            cameraTransform.position = LastCameraPosition;
            deathCapTransform.position = LastPlayerPosition;

            await Tween.Position(cameraTransform, LastCameraPosition, LastPlayerPosition + Vector3.up * 1.5f - Vector3.forward*100f, lerpCameraTween);
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            Tween.Position(cameraTransform, cameraTransform.position + Vector3.down * 12f, lerpCameraTween);
        }
    }
}