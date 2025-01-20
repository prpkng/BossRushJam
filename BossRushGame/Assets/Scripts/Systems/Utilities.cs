using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BRJ.Systems
{
    public static class Utilities
    {
        public static void WithComponent<T>(this Component behaviour, Action<T> action) where T : Component
        {
            if (behaviour.TryGetComponent(out T component)) action(component);
        }
        public static Vector2 FromDegrees(float angle) => new(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        public static Vector2 FromRadians(float angle) => new(Mathf.Cos(angle), Mathf.Sin(angle));

        public static float RoundToMultiple(float value, float multiple) => Mathf.Round(value / multiple) * multiple;
        
        public static T ChooseRandom<T>(this ICollection<T> collection) =>
            collection.ElementAt(Random.Range(0, collection.Count));   
    }
    public struct Maybe<T> where T : class
    {
        private T Value { get; set; }

        public Maybe(T value)
        {
            Value = value;
        }

        public readonly void With(Action<T> action)
        {
            if (Value != null) action(Value);
        }

    }
}