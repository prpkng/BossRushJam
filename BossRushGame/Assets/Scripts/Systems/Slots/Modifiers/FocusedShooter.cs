namespace BRJ.Systems.Slots.Modifiers
{
    [ModifierChance(10)]
    public class FocusedShooter : Modifier
    {
        public override string SpritePath => "SlotIcons/FocusedShooter.png";
        public override string Name => "Focused Shooter";
        protected override string Tier1Description => "Increased fire rate\nDecreased movement speed";
        protected override string Tier2Description =>
            "Increased fire rate\n<color=\"aqua\">Just a little bit slower";
        protected override string Tier3Description =>
            "Increased fire rate\n<color=\"aqua\">No downside?";

        public override void ApplyAdvantage()
        {
            Game.Instance.World.Player.activeGun.fireRate *= 1.15f;
        }

        public override void ApplyDownside()
        {
            Game.Instance.World.Player.movementSpeed *= 0.8f;
        }
    }
}