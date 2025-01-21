using System;
using PrimeTween;
using UnityEngine;

namespace BRJ.Systems.Common
{
    public class TweenAtStart : MonoBehaviour
    {
        public TweenSettings<Vector3> TweenPosition;

        private void Start()
        {
            Tween.Position(transform, TweenPosition);
            Destroy(this);
        }
    }
}