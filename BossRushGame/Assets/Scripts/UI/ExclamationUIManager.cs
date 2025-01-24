namespace BRJ.UI
{
    using UnityEngine;

    public class ExclamationUIManager : MonoBehaviour
    {
        private static Vector3? exclamationPos = null;

        public static void SetExclamation(Vector3 pos) => exclamationPos = pos;
        public static void RemoveExclamation() => exclamationPos = null;

        // ======================

        public float multiplier = 0.9f;

        public float clampX = 320 / 2 - 16f;
        public float clampY = 180 / 2 - 16f;

        public RectTransform exclamationTransform;

        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }


        private void LateUpdate()
        {
            if (!exclamationPos.HasValue)
            {
                exclamationTransform.gameObject.SetActive(false);
                return;
            }
            exclamationTransform.gameObject.SetActive(true);

            var viewportPos = mainCamera.WorldToScreenPoint(exclamationPos.Value);

            var pos = (Vector2)viewportPos - new Vector2(320, 180);

            pos *= multiplier;

            exclamationTransform.anchoredPosition = new Vector2(
                Mathf.Clamp(pos.x, -clampX, clampX),
                Mathf.Clamp(pos.y, -clampY, clampY)
            );
        }
    }
}