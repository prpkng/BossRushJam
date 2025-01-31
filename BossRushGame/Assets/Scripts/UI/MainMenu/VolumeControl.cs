namespace BRJ.UI
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class VolumeControl : MonoBehaviour, IMoveHandler
    {
        public float value = 1f;
        public RectTransform handle;
        public float width;
        public string key;
        public string vcaPath = "vca:/";
        public float increment = 0.05f;

        private FMOD.Studio.VCA vca;

        private void Awake()
        {
            vca = FMODUnity.RuntimeManager.GetVCA(vcaPath);
            if (PlayerPrefs.HasKey(key))
                value = PlayerPrefs.GetFloat(key);
            UpdatePosition();
        }

        public void UpdatePosition()
        {
            value = Mathf.Clamp(value, 0, 1);
            PlayerPrefs.SetFloat(key, value);
            vca.setVolume(value);
            handle.anchoredPosition = new Vector2(value * width, 0);
            PlayerPrefs.Save();
        }

        public void OnMove(AxisEventData eventData)
        {
            switch (eventData.moveDir)
            {
                case MoveDirection.Left:
                    eventData.Use();
                    value -= increment;
                    UpdatePosition();
                    break;
                case MoveDirection.Right:
                    eventData.Use();
                    value += increment;
                    UpdatePosition();
                    break;
            }
        }
    }
}