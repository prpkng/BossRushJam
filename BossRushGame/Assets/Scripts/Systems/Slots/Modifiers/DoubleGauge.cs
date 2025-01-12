namespace Game.Systems.Slots.Modifiers
{
    [ModifierChance(8)]
    public class DoubleGauge : Modifier
    {
        public override string SpritePath => "SlotIcons/DoubleGauge.png";
        public override string Name => "Double Gauge";
        public override string Description => "Amplifies your weapon\nJust watch its recoil";
        
        public override void ApplyAdvantage()
        {
            GameManager.Instance.Player.activeGun.bulletDamage *= 1.15f;
        }

        public override void ApplyDownside()
        {
            GameManager.Instance.Player.activeGun.bulletRecoil = 10f;
        }
    }
}