namespace Game
{
    using Game.Player;
    using Pixelplacement;
    using UnityEngine;

    public class GameManager : Singleton<GameManager>
    {
        public PlayerManager Player;
    }
}