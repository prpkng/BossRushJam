namespace BRJ.UI
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;
    using UnityEngine.UI;

    public class VolumeControl : MonoBehaviour, IMoveHandler
    {
        public float value = 1f;
        public RectTransform handle;
        public float width;
        public string key;
        public string vcaPath = "vca:/";
        public float increment = 0.05f;

        private bool isMouseInside;
        private FMOD.Studio.VCA vca;
        private CanvasScaler canvas;
        private Camera cam;

        private Vector2 pointerPos;

        private void Awake()
        {
            cam = Camera.main;
            canvas = GetComponentInParent<CanvasScaler>();
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

        public void MoveLeft()
        {
            value -= increment;
            UpdatePosition();
        }
        public void MoveRight()
        {
            value += increment;
            UpdatePosition();
        }
        public void OnMove(AxisEventData eventData)
        {
            switch (eventData.moveDir)
            {
                case MoveDirection.Left:
                    eventData.Use();
                    MoveLeft();
                    break;
                case MoveDirection.Right:
                    eventData.Use();
                    MoveRight();
                    break;
            }
        }
    }
}