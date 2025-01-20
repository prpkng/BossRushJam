namespace BRJ.Bosses
{
    using UnityEngine;

    public class BossBarController : MonoBehaviour
    {
        public RectTransform rectTransform;

        private float startWidth;
        private void Start() {
            startWidth = rectTransform.rect.width;
        }

        public void SetHealthPercentage(float percentage)
        {
            float value = percentage / 100f;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, startWidth * value);
        }
    }
}