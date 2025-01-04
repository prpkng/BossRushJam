namespace Game.Player
{
    using DG.Tweening;
    using Pixelplacement;
    using Unity.Cinemachine;
    using UnityEngine;

    public class CameraManager : Singleton<CameraManager>
    {
        public static Vector2 currentScreenOffset;

        public CinemachinePositionComposer positionComposer;

        public float focusUpScreenY = 0.1f;
        public float focusDuration = 1f;

        public void FocusUp()
        {
            this.DOKill();
            DOTween.To(
                () => positionComposer.Composition.ScreenPosition.y,
                f => positionComposer.Composition.ScreenPosition = new Vector2(positionComposer.Composition.ScreenPosition.x, f),
                focusUpScreenY,
                focusDuration
            ).SetTarget(this);
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
        }

        private void Update()
        {
            currentScreenOffset = positionComposer.Composition.ScreenPosition;
        }
    }
}