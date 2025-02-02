namespace BRJ.Systems.Slots.Modifiers
{
    [ModifierChance(10)]
    public class MachineGun : Modifier
    {
        public override string SpritePath => "SlotIcons/MachineGun.png";
        public override string Name => "Machine Gun";
        protected override string Tier1Description => "Turns your stick into a Machine Gun\nYour bullets are weaker";
        protected override string Tier2Description =>
            "Turns your stick into a Machine Gun\n<color=\"aqua\">But with steel bullets";
        protected override string Tier3Description =>
            "<color=\"aqua\">Turns your stick into a UZI\nWOW";


        public override void ApplyAdvantage()
        {
            Game.Instance.World.Player.activeGun.fireRate *= 2;
        }

        public override void ApplyDownside()
        {
            Game.Instance.World.Player.activeGun.bulletDamage *= 0.5f;
        }
    }
}