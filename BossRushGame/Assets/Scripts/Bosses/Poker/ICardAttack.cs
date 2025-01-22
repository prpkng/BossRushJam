namespace BRJ.Bosses.Poker
{
    public interface ICardAttack
    {
        /// <summary>
        /// Start the card's attack
        /// </summary>
        /// <returns>The attack duration</returns>
        float StartAttack();

        /// <summary>
        /// Stop the card's attack
        /// </summary>
        void StopAttack();
    }
}