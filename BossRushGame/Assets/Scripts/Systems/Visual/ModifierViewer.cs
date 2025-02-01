namespace BRJ.UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.UI;

    public class ModifierViewer : MonoBehaviour
    {
        public TMP_Text titleText;
        public TMP_Text descriptionText;
        public Image iconImage;

        private void OnEnable()
        {
            var mod = Game.Instance.World.CurrentActiveModifier;
            if (mod == null) return;
            titleText.text = mod.Name;
            descriptionText.text = mod.Description;
            iconImage.sprite = mod.iconSprite;
        }

    }
}