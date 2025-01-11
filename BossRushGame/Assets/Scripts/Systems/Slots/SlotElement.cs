using System;
using Game.Systems.Slots.Modifiers;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Systems.Slots
{
    public class SlotElement : MonoBehaviour
    {
        private MeshRenderer meshRenderer;
        private bool canChange = true;

        public Modifier currentModifier;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            if (transform.position.z > 0 && !canChange) canChange = true;
        }

        public void SetRandom()
        {
            if (!canChange || transform.position.z > 0) return;
            canChange = false;
            var modifier = Modifier.ModifierList.ChooseRandom();
            currentModifier = (Modifier)Activator.CreateInstance(modifier);
            var texture = Addressables.LoadAssetAsync<Texture>(currentModifier.SpritePath);
            texture.Completed += t => meshRenderer.material.mainTexture = t.Result;
        }
    }
}