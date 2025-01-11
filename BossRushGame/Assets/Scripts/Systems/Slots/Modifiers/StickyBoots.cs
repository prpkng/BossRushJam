namespace Game.Systems.Slots.Modifiers
{
    [ModifierChance(10)]
    public class StickyBoots : Modifier
    {
        public override string SpritePath => "SlotIcons/StickyBoots.png";
        public override string Name => "Sticky Boots";
        public override string Description => "Increased movement speed\nCannot roll anymore";
        
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