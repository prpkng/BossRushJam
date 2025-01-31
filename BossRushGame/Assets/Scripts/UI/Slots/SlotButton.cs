using System;
using System.Collections.Generic;
using System.Linq;
using BRJ.Systems.Slots.Modifiers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BRJ.UI.Slots
{
    public class SlotButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        public static SlotButton CurrentSelectedSlot { get; private set; }

        private static List<SlotButton> buttonList = new();
        private static readonly int Enabled = Shader.PropertyToID("_Enabled");
        private List<Image> icons;

        private List<SpriteRenderer> attachedBGs = new();
        [SerializeField] private SpriteRenderer bgSprite;

        public Modifier CurrentModifier;

        private void Awake()
        {
            icons = new List<Image> { GetComponent<Image>() };
        }

        private static void SetSelected(SlotButton btn)
        {
            CurrentSelectedSlot = btn;
            buttonList.ForEach(b =>
            {
                b.bgSprite.enabled = b == CurrentSelectedSlot;
                b.attachedBGs.ForEach(bg => bg.enabled = b == CurrentSelectedSlot);
            });
        }

        public void SetupFromModifier(Modifier mod)
        {

            icons.ForEach(icon => icon.material = Instantiate(icon.material));

            var op = Addressables.LoadAssetAsync<Sprite>(mod.SpritePath);
            op.Completed += result => icons.ForEach(icon => icon.sprite = result.Result);
            CurrentModifier = mod;

            // Check if there's already a button containing this modifier
            if (buttonList.Select(b => b.CurrentModifier.GetType()).Contains(mod.GetType()))
            {
                var btn = buttonList.First(b => b.CurrentModifier.GetType() == mod.GetType());
                btn.icons.Add(icons.First());
                btn.attachedBGs.Add(bgSprite);
                btn.CurrentModifier.Tier++;
                transform.SetParent(btn.transform);
                print(btn.CurrentModifier.Tier);
                Destroy(this);
                Destroy(GetComponent<Button>());
                return;
            }
            buttonList.Add(this);
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            eventData.selectedObject = gameObject;
            OnSelect(null);
        }
        public void OnPointerExit(PointerEventData _)
        {
            icons.ForEach(icon => icon.material.SetInt(Enabled, 0));
            SlotHoverTooltip.Instance.SetVisible(false);
        }

        public void OnSelect(BaseEventData _)
        {
            icons.ForEach(icon => icon.material.SetInt(Enabled, 1));
            SlotHoverTooltip.Instance.SetVisible(true);
            SlotHoverTooltip.Instance.UpdateText(CurrentModifier);
        }
        public void OnDeselect(BaseEventData _)
        {
            icons.ForEach(icon => icon.material.SetInt(Enabled, 0));
            SlotHoverTooltip.Instance.SetVisible(false);
        }

        private void OnDisable()
        {
            if (buttonList.Contains(this)) buttonList.Remove(this);
        }

        public void OnClick()
        {
            print("Submit prssed");
            SetSelected(this);
        }
    }
}