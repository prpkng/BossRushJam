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
            throw new System.NotImplementedException();
        }

        public override void ApplyDownside()
        {
            throw new System.NotImplementedException();
        }
    }
}