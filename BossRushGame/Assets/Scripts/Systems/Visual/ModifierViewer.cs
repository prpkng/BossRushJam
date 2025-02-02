namespace BRJ.UI
{
    using System;
    using System.Linq;
    using TMPro;
    using Unity.VisualScripting;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.UI;

    public class ModifierViewer : MonoBehaviour
    {
        public TMP_Text titleText;
        public TMP_Text descriptionText;
        public Image iconImage;
        public Color greenColor;
        public Color redColor;
        public GameObject noModContainer;
        public GameObject modContainer;

        private void OnEnable()
        {
            var mod = Game.Instance.World.CurrentActiveModifier;
            if (mod == null)
            {
                noModContainer.SetActive(true);
                modContainer.SetActive(false);
                return;
            }
            titleText.text = mod.Name + mod.Tier switch
            {
                2 => "+",
                3 => "++",
                _ => ""
            };
            var split = mod.Description.Split('\n');
            descriptionText.text = $"<color=#{greenColor.ToHexString()}>- Advantages:</color>\n    {split[0]}\n<color=#{redColor.ToHexString()}>- Disadvantages:</color>\n    {split[1]}\n{(split.Length > 2 ? split[2] : "")}";
            iconImage.sprite = mod.iconSprite;
        }

    }
}