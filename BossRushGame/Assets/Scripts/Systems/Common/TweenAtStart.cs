using System;
using PrimeTween;
using UnityEngine;

namespace Game.Systems.Common
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