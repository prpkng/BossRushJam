namespace Game.Systems.Slots.Modifiers
{
    [ModifierChance(8)]
    public class Athletics : Modifier
    {
        public override string SpritePath => "SlotIcons/Athletics.png";
        public override string Name => "Athletics";
        public override string Description => "Roll further away\nYour legs can get tired";
        
        public override void ApplyAdvantage()
        {
            GameManager.Instance.Player.rollDuration *= 1.5f;
        }

        public override void ApplyDownside()
        {
            GameManager.Instance.Player.rollCooldown *= 2f;
        }
    }
}