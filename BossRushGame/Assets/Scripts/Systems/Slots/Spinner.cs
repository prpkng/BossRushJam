using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.UI.Slots;
using PrimeTween;
using UnityEngine;

namespace Game.Systems.Slots
{
    public class Spinner : MonoBehaviour
    {
        public Canvas canvas;
        public GameObject uiSlotPrefab;
        public Texture[] slotSprites;

        public float spinDuration = 4f;
        public float spinAcceleration = 3f;
        public float spinDeceleration = 3f;
        public float maxSpinSpeed = 50f;
        
        private float spinSpeed;
        public float CurrentSpinSpeed => spinSpeed;
        private SlotElement[] slots;



        private void Awake()
        {
            slots = GetComponentsInChildren<SlotElement>();

            foreach (var slot in slots) slot.SetRandom();
        }

        private async UniTaskVoid SelectNearest()
        {
            enabled = false;
            await Tween.Custom(
                transform,
                transform.eulerAngles.x,
                Utilities.RoundToMultiple(transform.eulerAngles.x, 45),
                .5f,
                (t, f) =>
                {
                    var angles = t.eulerAngles;
                    angles.x = f;
                    t.eulerAngles = angles;
                },
                Ease.InOutQuad
            );
            
            var selectedSlot = slots.OrderBy(s => s.transform.position.z).First();

            var obj = Instantiate(uiSlotPrefab, canvas.transform, false);
            obj.transform.position = new Vector3(transform.position.x, transform.position.y, -20f);
            obj.GetComponent<SlotButton>().SetupFromModifier(selectedSlot.currentModifier);
            Destroy(this);
            foreach (var slot in slots) Destroy(slot);
        }
        
        private float counter = 0f;
        private void Update()
        {
            counter += Time.deltaTime;
            if (spinSpeed > 0f)
                transform.Rotate(spinSpeed * Time.deltaTime * Vector3.right);

            if (counter > spinDuration)
            {
                if (spinSpeed > 0f)
                    spinSpeed -= Time.deltaTime * spinDeceleration;
                else
                {
                    SelectNearest().Forget();
                }

                return;
            }

            if (spinSpeed < maxSpinSpeed)
                spinSpeed += Time.deltaTime * spinAcceleration;

            foreach (var slot in slots)
                slot.SetRandom();
        }
    }
}