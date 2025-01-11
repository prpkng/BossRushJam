using System;
using System.Linq;
using PrimeTween;
using UnityEngine;

namespace Game.Systems.Slots
{
    public class Spinner : MonoBehaviour
    {
        private SlotElement[] slots;
        public Texture[] slotSprites;

        public float spinAcceleration = 3f;
        public float spinDeceleration = 3f;
        public float maxSpinSpeed = 50f;
        private float spinSpeed;

        public float spinDuration = 4f;


        private void Awake()
        {
            slots = GetComponentsInChildren<SlotElement>();

            foreach (var slot in slots) slot.SetRandom();
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
                    Tween.Custom(
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
                    var selectedSlot = slots.OrderBy(s => -Mathf.Abs(s.transform.eulerAngles.x))
                        .First();
                    print(selectedSlot.currentModifier.Name);
                    Destroy(this);
                    foreach (var slot in slots) Destroy(slot);
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