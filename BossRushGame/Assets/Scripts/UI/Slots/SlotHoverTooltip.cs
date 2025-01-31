using System;
using BRJ.Systems.Slots.Modifiers;
using Pixelplacement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace BRJ.UI.Slots
{
    public class SlotHoverTooltip : Singleton<SlotHoverTooltip>
    {
        public UIDocument document;
        public int panelHeight = 225;

        private Label titleLabel;
        private Label descriptionLabel;
        private Camera cam;

        private void Start()
        {
            cam = Camera.main;
            document.enabled = false;
        }


        private void Update()
        {
            if (!document.enabled) return;
            var mousePos = RuntimePanelUtils.CameraTransformWorldToPanel(document.runtimePanel, EventSystem.current.currentSelectedGameObject.transform.position, cam);
            document.rootVisualElement.transform.position = new Vector2(mousePos.x, panelHeight - mousePos.y);
        }

        public void SetVisible(bool visible)
        {
            document.enabled = visible;
        }

        public void UpdateText(Modifier mod)
        {
            print($"Updating tooltip with {mod.Name}");
            document.rootVisualElement.Q<Label>("Title").text = mod.Name;
            document.rootVisualElement.Q<Label>("Description").text = mod.Description;
            document.rootVisualElement.Q<VisualElement>("Star1").style.display =
                mod.Tier > 1 ? DisplayStyle.Flex : DisplayStyle.None;
            document.rootVisualElement.Q<VisualElement>("Star2").style.display =
                mod.Tier > 2 ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}