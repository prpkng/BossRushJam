using System;

namespace Game.Systems.Slots.Modifiers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModifierChanceAttribute : Attribute
    {
        public int Odds;
        public ModifierChanceAttribute(int odds)
        {
            Odds = odds;
        }
    }
}