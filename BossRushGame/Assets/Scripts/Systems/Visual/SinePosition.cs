using System;
using System.Collections;
using PrimeTween;
using UnityEngine;

namespace BRJ.Systems.Visual
{
    public class SinePosition : MonoBehaviour
    {
        public TweenSettings<Vector3> sineTween;

        private IEnumerator Start()
        {
            if (!this)
                yield break;
            yield return Tween.Position(transform, sineTween).ToYieldInstruction();
            yield return Tween.Position(transform, 
                    sineTween.WithDirection(false)).ToYieldInstruction();

            yield return Start();
        }
    }
}