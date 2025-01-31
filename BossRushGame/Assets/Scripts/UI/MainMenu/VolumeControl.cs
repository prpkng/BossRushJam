namespace BRJ.UI
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;
    using UnityEngine.UI;

    public class VolumeControl : MonoBehaviour, IMoveHandler, IPointerEnterHandler, IPointerExitHandler
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

        private void Awake()
        {
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

        private bool isDragging;

        private void Update()
        {
            if (InputManager.isUsingGamepad) return;
            if (!isMouseInside && !isDragging) return;
            if (!Mouse.current.leftButton.isPressed)
            {
                isDragging = false;
                return;
            }

            isDragging = true;

            var viewport = Mouse.current.position.ReadValue() / new Vector2(Screen.width, Screen.height);

            var mp = canvas.referenceResolution * viewport;
            var pos = (Vector2)transform.position - canvas.referenceResolution / 2f;
            print(mp.x - pos.x);

            var lastValue = value;
            value = (mp.x - pos.x) / width;
            if (value != lastValue)
                UpdatePosition();

            print(value);

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isMouseInside = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isMouseInside = true;
            print("MouseEntered");
        }
    }
}