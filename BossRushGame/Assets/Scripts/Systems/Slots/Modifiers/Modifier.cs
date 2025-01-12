using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Game.Systems.Slots.Modifiers
{
    public abstract class Modifier
    {
        public static List<Type> ModifierList;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterModifiers()
        {
            ModifierList = new List<Type>();
            var modifiers = Assembly
                .GetCallingAssembly()
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
        
        public abstract string SpritePath { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract void ApplyAdvantage();
        public abstract void ApplyDownside();
    }
}