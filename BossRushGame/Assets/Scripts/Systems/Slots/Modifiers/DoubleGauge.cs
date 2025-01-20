namespace Game.Systems.Slots.Modifiers
{
    [ModifierChance(7)]
    public class DoubleGauge : Modifier
    {
        public override string SpritePath => "SlotIcons/DoubleGauge.png";
        public override string Name => "Double Gauge";
        protected override string Tier1Description => "Amplifies your weapon\nJust watch its recoil";

        protected override string Tier2Description =>
            "Amplified your weapon\n<color=aqua>A little bit of recoil";
        protected override string Tier3Description => 
            "Amplifies your weapon\n<color=aqua>Almost the same as before";
        
        public override void ApplyAdvantage()
        {
            WorldManager.Instance.Player.activeGun.bulletDamage *= 1.15f;
        }

        public override void ApplyDownside()
        {
            WorldManager.Instance.Player.activeGun.bulletRecoil = 10f;
        }
    }
}