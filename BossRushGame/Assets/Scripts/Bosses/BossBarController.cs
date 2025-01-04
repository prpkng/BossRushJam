namespace Game.Bosses
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
            print($"HealthPercentage: {percentage}");
            bar.style.right = new StyleLength(Length.Percent(100f - percentage));
        }
    }
}