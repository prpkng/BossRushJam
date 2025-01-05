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

        public TweenSettings<float> focusUpTween;
        public TweenSettings<float> focusDownTween;
        
        public float focusUpScreenY = 0.1f;
        public float focusDuration = 1f;

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
                focusDuration,
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
                focusDuration,
                f => GameManager.Instance.RenderTextureZoom = f
            );
        }

        private void Update()
        {
            currentScreenOffset = positionComposer.Composition.ScreenPosition;
        }
    }
}