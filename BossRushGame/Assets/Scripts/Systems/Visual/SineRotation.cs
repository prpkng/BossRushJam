using System;
using System.Collections;
using PrimeTween;
using UnityEngine;

namespace Game.Systems.Visual
{
    public class SineRotation : MonoBehaviour
    {
        public TweenSettings<float> sineTween;

        private IEnumerator Start()
        {
            if (!this)
                yield break;
            yield return Tween.Custom(transform, sineTween,
                    (t, f) => t.eulerAngles = Vector3.forward * f)
                .ToYieldInstruction();
            yield return Tween.Custom(transform, sineTween.WithDirection(false, false),
                    (t, f) => t.eulerAngles = Vector3.forward * f)
                .ToYieldInstruction();

            yield return Start();
        }
    }
}