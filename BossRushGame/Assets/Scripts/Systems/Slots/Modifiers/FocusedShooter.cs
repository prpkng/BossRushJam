namespace Game.Systems.Slots.Modifiers
{
    [ModifierChance(10)]
    public class FocusedShooter : Modifier
    {
        public override string SpritePath => "SlotIcons/FocusedShooter.png";
        public override string Name => "Focused Shooter";
        public override string Description => "Decreased movement speed\nIncreased fire rate";
        
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