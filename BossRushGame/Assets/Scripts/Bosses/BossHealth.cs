namespace Game.Bosses
{
    using Game.Systems;
    using UnityEngine;

    public class BossHealth : HealthBehavior
    {
        private void Awake()
        {
            GameManager.Instance.CreateBossBar();
        }
        public override void ApplyDamage(float damage)
        {
            base.ApplyDamage(damage);

            GameManager.Instance.BossBarController.With(b =>
            {
                b.SetHealthPercentage(currentHealth / totalHealth * 100f);
            });
        }
    }
}