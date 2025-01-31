namespace BRJ.Systems.Visual
{
    using UnityEngine;

    public class SwitchWithInput : MonoBehaviour
    {
        public Sprite pcInput;
        public Sprite xboxInput;
        public Sprite psInput;
        public new SpriteRenderer renderer;

        private void OnEnable()
        {
            InputManager.InputChanged += SwitchSprite;
        }

        private void OnDisable()
        {
            InputManager.InputChanged -= SwitchSprite;
        }

        public void SwitchSprite()
        {
            renderer.sprite = InputManager.isUsingGamepad ? (InputManager.isPsGamepad ? psInput : xboxInput) : pcInput;
        }
    }
}