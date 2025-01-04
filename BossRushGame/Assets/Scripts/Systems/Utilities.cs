using System;

namespace Game.Systems
{
    public struct Maybe<T> where T : class
    {
        public T Value { get; set; }

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