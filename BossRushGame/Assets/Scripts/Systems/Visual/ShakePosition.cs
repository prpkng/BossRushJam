using System;
using System.Collections;
using PrimeTween;
using UnityEngine;

namespace BRJ.Systems.Visual
{
    public class ShakePosition : MonoBehaviour
    {
        public ShakeSettings shakeSettings;

        public void TriggerShake()
        {
            Tween.ShakeLocalPosition(transform, shakeSettings);
        }
    }
}