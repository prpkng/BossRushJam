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
            throw new System.NotImplementedException();
        }

        public override void ApplyDownside()
        {
            throw new System.NotImplementedException();
        }
    }
}