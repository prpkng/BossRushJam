namespace Game.Player
{
    using DG.Tweening;
    using Tween = Pixelplacement.Tween;
    using Pixelplacement;
    using Unity.Cinemachine;
    using UnityEngine;
    using Pixelplacement.TweenSystem;

    public class CameraManager : Singleton<CameraManager>
    {
        public static Vector2 currentScreenOffset;

        public CinemachinePositionComposer positionComposer;

        public float focusUpScreenY = 0.1f;
        public float focusDuration = 1f;

        public float defaultZoom = 1.5f;
        public float zoomOutZoom = 1.35f;


        private void Awake()
        {

        }

        private TweenBase _scaleTween;
        public void FocusUp()
        {
            this.DOKill();
            DOTween.To(
                () => positionComposer.Composition.ScreenPosition.y,
                f => positionComposer.Composition.ScreenPosition = new Vector2(positionComposer.Composition.ScreenPosition.x, f),
                focusUpScreenY,
                focusDuration
            ).SetTarget(this);

            _scaleTween?.Stop();
            _scaleTween = Tween.Value(
                GameManager.Instance.RenderTextureZoom,
                zoomOutZoom,
                f => GameManager.Instance.RenderTextureZoom = f,
                focusDuration,
                0f,
                Tween.EaseOut
            );
        }

        public void ResetFocus()
        {
            this.DOKill();
            DOTween.To(
                () => positionComposer.Composition.ScreenPosition.y,
                f => positionComposer.Composition.ScreenPosition = new Vector2(positionComposer.Composition.ScreenPosition.x, f),
                0,
                focusDuration
            ).SetTarget(this);

            _scaleTween?.Stop();
            _scaleTween = Tween.Value(
                GameManager.Instance.RenderTextureZoom,
                defaultZoom,
                f => GameManager.Instance.RenderTextureZoom = f,
                focusDuration,
                0f,
                Tween.EaseOut
            );
        }

        private void Update()
        {
            currentScreenOffset = positionComposer.Composition.ScreenPosition;
        }
    }
}