namespace Game.Systems.Slots.Modifiers
{
    [ModifierChance(10)]
    public class FocusedShooter : Modifier
    {
        public override string SpritePath => "SlotIcons/FocusedShooter.png";
        public override string Name => "Focused Shooter";
        public override string Description => "Increased fire rate\nDecreased movement speed";
        
        public override void ApplyAdvantage()
        {
            GameManager.Instance.Player.activeGun.fireRate *= 1.15f;
        }

        public override void ApplyDownside()
        {
            GameManager.Instance.Player.movementSpeed *= 0.8f;
        }
    }
}