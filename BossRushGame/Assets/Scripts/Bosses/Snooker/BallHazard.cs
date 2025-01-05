using Game.Player;
using Game.Systems;
using UnityEngine;

namespace Game.Bosses.Snooker
{
    public class BallHazard : Hazard
    {
        [SerializeField] private Rigidbody2D ballRb;
        [SerializeField] private float knockbackVelocityThreshold;
        protected override Vector2 CalculateKnockback(PlayerHitbox player)
        {
            if (ballRb.linearVelocity.magnitude <= knockbackVelocityThreshold) 
                return base.CalculateKnockback(player);
            
            var normalizedVelocity = ballRb.linearVelocity.normalized;
            return Utilities.Choose(new[] {Vector2.Perpendicular(normalizedVelocity), -Vector2.Perpendicular(normalizedVelocity)});
        }
    }
}