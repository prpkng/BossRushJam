namespace Game.Systems.Slots.Modifiers
{
    [ModifierChance(8)]
    public class Athletics : Modifier
    {
        public override string SpritePath => "SlotIcons/Athletics.png";
        public override string Name => "Athletics";
        protected override string Tier1Description => "Roll further away\nYour legs can get tired";
        protected override string Tier2Description => 
            "Roll further away\nYour legs can get tired" +
            "\n<color=aqua>- Less than you may think...</color>";
        protected override string Tier3Description => 
            "Roll <color=aqua>even</color> further away\nYour legs can get tired" +
            "\n<color=aqua>- Less than you may think...</color>";

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