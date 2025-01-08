using Pixelplacement;
using PrimeTween;
using Unity.Cinemachine;
using UnityEngine;
using Tween = PrimeTween.Tween;

namespace Game.Player
{

    public class CameraManager : Singleton<CameraManager>
    {
        public static Vector2 currentScreenOffset;

        public CinemachinePositionComposer positionComposer;
        public CinemachineRecomposer recomposer;

        [Header("Focus Settings")]
        
        public TweenSettings<float> focusUpTween;
        public TweenSettings<float> focusDownTween;
        
        public float zoomDuration = 1f;
        public float defaultZoom = 1.5f;
        public float zoomOutZoom = 1.35f;

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
                GameManager.Instance.RenderTextureZoom,
                zoomOutZoom,
                zoomDuration,
                f => GameManager.Instance.RenderTextureZoom = f
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
                GameManager.Instance.RenderTextureZoom,
                defaultZoom,
                zoomDuration,
                f => GameManager.Instance.RenderTextureZoom = f
            );
        }

        private Tween _shakePosTween;
        public void ShakeCamera(ShakeSettings shakeSettings)
        {
            _shakePosTween.Complete();
            _shakePosTween = Tween.ShakeCustom(this, Vector3.zero, shakeSettings, (self, vector3) =>
            {
                self.recomposer.Pan = vector3.x;
                self.recomposer.Tilt = vector3.y;
                self.recomposer.Dutch = vector3.z;
            });
        }
        
        private void Update()
        {
            currentScreenOffset = positionComposer.Composition.ScreenPosition;
        }
    }
}