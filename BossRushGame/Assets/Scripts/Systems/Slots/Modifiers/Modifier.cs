using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BRJ.Systems.Slots.Modifiers
{
    public abstract class Modifier
    {
        public static List<Type> ModifierList;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterModifiers()
        {
            ModifierList = new List<Type>();
            var modifiers = Assembly
                .GetAssembly(typeof(Modifier))
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Modifier)))
                .ToArray();
            Debug.Log($"Registered {modifiers.Length} modifiers");
            foreach (var mod in modifiers)
            {
                var odds = 1;
                var attr = mod.GetCustomAttribute<ModifierChanceAttribute>();
                if (attr != null) odds = attr.Odds;
                for (var i = 0; i < odds; i++) ModifierList.Add(mod);
            }
            Debug.Log($"Registered {ModifierList.Count} modifiers (With weight)");
        }

        public string Description => Tier switch
        {
            1 => Tier1Description,
            2 => Tier2Description,
            3 => Tier3Description,
            _ => throw new ArgumentOutOfRangeException()
        };

        public int Tier = 1;
        public abstract string SpritePath { get; }
        public abstract string Name { get; }
        protected abstract string Tier1Description { get; }
        protected virtual string Tier2Description => Tier1Description;
        protected virtual string Tier3Description => Tier1Description;
        public abstract void ApplyAdvantage();
        public abstract void ApplyDownside();
    }
}