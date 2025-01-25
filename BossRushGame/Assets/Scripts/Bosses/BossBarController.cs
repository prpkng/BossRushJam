namespace BRJ.Bosses
{
    using UnityEngine;

    public class BossBarController : MonoBehaviour
    {
        public RectTransform rectTransform;

        private float startWidth;
        private void Start()
        {
            startWidth = rectTransform.rect.width;
            Game.Instance.World.BossBarController.Set(this);
        }

        public void SetHealthPercentage(float percentage)
        {
            float value = percentage / 100f;
            if (value <= 0)
                gameObject.SetActive(false);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, startWidth * value);
        }
    }
}