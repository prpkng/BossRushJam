namespace Game.Bosses
{
    using Game.Systems;
    using UnityEngine;

    public class BossHitbox : MonoBehaviour
    {
        public HealthBehavior health;

        private void OnTriggerEnter2D(Collider2D other)
        {
            health.ApplyDamage(GameManager.Instance.Player.activeGun.bulletDamage);
            Destroy(other.gameObject);
        }
    }
}