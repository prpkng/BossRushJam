namespace Game.Systems.Slots.Modifiers
{
    [ModifierChance(10)]
    public class StickyBoots : Modifier
    {
        public override string SpritePath => "SlotIcons/StickyBoots.png";
        public override string Name => "Sticky Boots";
        protected override string Tier1Description => "Increased movement speed\nCannot roll anymore";
        protected override string Tier2Description =>
            "<color=aqua>Incredible</color> movement speed\nCannot roll anymore";
        protected override string Tier3Description => 
            "<color=aqua>AMAZING</color> movement speed\nCannot roll anymore";
        
        public override void ApplyAdvantage()
        {
            WorldManager.Instance.Player.movementSpeed *= 1.45f;
        }

        public override void ApplyDownside()
        {
            WorldManager.Instance.Player.CanRollOverride = false;
        }
    }
}