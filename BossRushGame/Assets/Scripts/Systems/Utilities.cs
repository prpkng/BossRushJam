using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Game.Systems
{
    public static class Utilities
    {
        public static T Choose<T>(ICollection<T> collection) =>
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