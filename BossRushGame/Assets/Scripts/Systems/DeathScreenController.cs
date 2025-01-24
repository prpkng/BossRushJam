using System;
using Cysharp.Threading.Tasks;
using FMODUnity;
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

        public EventReference deathSnapshotEvent;

        private async void Start()
        {
            cameraTransform.position = LastCameraPosition;
            deathCapTransform.position = LastPlayerPosition;

            var ev = RuntimeManager.CreateInstance(deathSnapshotEvent);
            ev.start();

            await Tween.Position(cameraTransform, LastCameraPosition, LastPlayerPosition + Vector3.up * 1.5f - Vector3.forward * 100f, lerpCameraTween);
            await UniTask.Delay(TimeSpan.FromSeconds(1));

            await Tween.Position(cameraTransform, cameraTransform.position + Vector3.down * 12f, lerpCameraTween);

            Game.Instance.Sound.CurrentBGM?.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            ev.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }
}