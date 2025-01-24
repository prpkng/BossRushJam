namespace BRJ.Bosses.Joker
{
    using BRJ.Systems.Common;
    using UnityEngine;

    public class JokerCard : HealthBehavior
    {
        public event System.Action OnHit;
        public void ClearCallbacks() => OnHit = null;

        public override void ApplyDamage(float damage)
        {
            OnHit?.Invoke();
        }
    }
}