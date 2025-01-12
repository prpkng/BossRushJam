namespace Game.Systems.Slots.Modifiers
{
    [ModifierChance(10)]
    public class MachineGun : Modifier
    {
        public override string SpritePath => "SlotIcons/MachineGun.png";
        public override string Name => "Machine Gun";
        public override string Description => "Turns your stick into a Machine Gun\nYour bullets are weaker";
        
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