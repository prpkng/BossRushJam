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
            GameManager.Instance.Player.movementSpeed *= 1.45f;
        }

        public override void ApplyDownside()
        {
            GameManager.Instance.Player.CanRollOverride = false;
        }
    }
}