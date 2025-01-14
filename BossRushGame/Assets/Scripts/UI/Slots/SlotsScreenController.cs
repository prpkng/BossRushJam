using System;
using System.Collections.Generic;
using System.Linq;
using Game.Systems.Slots;
using UnityEngine;
using UnityEngine.Events;

namespace Game.UI.Slots
{
    public class SlotsScreenController : MonoBehaviour
    {
        public List<Spinner> spinners;
        public Animator slotsAnimator;
        public float spinnerToSlotSpeed;
        
        public UnityEvent onSpinnersEnd;

        private bool waitingForSpinners = true;

        private void FixedUpdate()
        {
            if (waitingForSpinners && spinners.All(s => !s))
            {
                onSpinnersEnd.Invoke();
                waitingForSpinners = false;
            }

            slotsAnimator.speed = 1f + spinners.First().CurrentSpinSpeed * spinnerToSlotSpeed;
        }
    }
}