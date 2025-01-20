using Pixelplacement;
using PrimeTween;
using Unity.Cinemachine;
using UnityEngine;
using Tween = PrimeTween.Tween;

namespace BRJ.Player
{

    public class CameraManager : Singleton<CameraManager>
    {
        public static Vector2 currentScreenOffset;

        public CinemachineTargetGroup targetGroup;
        public CinemachinePositionComposer positionComposer;
        public CinemachineCameraOffset cameraOffset;
        public CinemachineRecomposer recomposer;

        [Header("Focus Settings")]
        
        public TweenSettings<float> focusUpTween;
        public TweenSettings<float> focusDownTween;
        
        public float zoomDuration = 1f;
        public float defaultZoom = 1.5f;
        public float zoomOutZoom = 1.35f;

        [Header("Default Shakes")] public ShakeSettings defaultWeakShake;
        
        private Tween _scaleTween;
        public void FocusUp()
        {
            Tween.Custom(
                focusUpTween,
                f => positionComposer.Composition.ScreenPosition =
                    new Vector2(positionComposer.Composition.ScreenPosition.x, f)
            );

            _scaleTween.Stop();
            _scaleTween = Tween.Custom(
                WorldManager.Instance.RenderTextureZoom,
                zoomOutZoom,
                zoomDuration,
                f => WorldManager.Instance.RenderTextureZoom = f
            );
        }
        public void ResetFocus()
        {
            Tween.Custom(
                focusDownTween,
                f => positionComposer.Composition.ScreenPosition =
                    new Vector2(positionComposer.Composition.ScreenPosition.x, f)
            );
            
            _scaleTween.Stop();
            _scaleTween = Tween.Custom(
                WorldManager.Instance.RenderTextureZoom,
                defaultZoom,
                zoomDuration,
                f => WorldManager.Instance.RenderTextureZoom = f
            );
        }

        private Tween _shakePosTween;
        public void ShakeCamera(ShakeSettings shakeSettings)
        {
            _shakePosTween.Complete();
            _shakePosTween = Tween.ShakeCustom(this, Vector3.zero, shakeSettings, (self, vector3) =>
            {
                self.cameraOffset.Offset = (Vector2)vector3;
                self.recomposer.Dutch = vector3.z;
            });
        }

        public void AddTarget(Transform target, float weight = .5f, float radius = 6f)
        {
            targetGroup.AddMember(target, weight, radius);    
        } 
        
        public void RemoveTarget(Transform target)
        {
            targetGroup.RemoveMember(target);
        }
        
        private void Update()
        {
            currentScreenOffset = positionComposer.Composition.ScreenPosition;
        }
    }
}