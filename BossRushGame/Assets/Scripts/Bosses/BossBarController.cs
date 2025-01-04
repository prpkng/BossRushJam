namespace Game.Bosses
{
    using UnityEngine;
    using UnityEngine.UIElements;

    public class BossBarController : MonoBehaviour
    {
        public UIDocument uiDocument;

        public void SetHealthPercentage(float percentage)
        {
            print($"HealthPercentage: {percentage}");
        }
    }
}