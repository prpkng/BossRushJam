using BRJ.Player;
using BRJ.Systems;
using UnityEngine;

namespace BRJ.Bosses.Snooker
{
    public class SnookerBallHazard : Hazard
    {
        [SerializeField] private Rigidbody2D ballRb;
        [SerializeField] private float knockbackVelocityThreshold;
        protected override Vector2 CalculateKnockback(PlayerHitbox player)
        {
            if (ballRb.linearVelocity.magnitude <= knockbackVelocityThreshold) 
                return base.CalculateKnockback(player);
            
            var normalizedVelocity = ballRb.linearVelocity.normalized;
            return Utilities.ChooseRandom(new[] {Vector2.Perpendicular(normalizedVelocity), -Vector2.Perpendicular(normalizedVelocity)});
        }
    }
}