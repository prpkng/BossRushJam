namespace BRJ.Bosses
{
    using UnityEngine;
    using UnityEngine.UIElements;

    public class BossBarController : MonoBehaviour
    {
        public UIDocument uiDocument;

        private VisualElement bar;

        private void Start()
        {
            bar = uiDocument.rootVisualElement.Q<VisualElement>("Bar");
        }

        public void SetHealthPercentage(float percentage)
        {
            float value = (100f - percentage) / 2;
            bar.style.right = new StyleLength(Length.Percent(value));
            bar.style.left = new StyleLength(Length.Percent(value));
        }
    }
}