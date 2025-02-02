namespace BRJ.Systems.Lobby
{
    using BRJ.Systems.Saving;
    using global::Game.Levels;
    using UnityEngine;

    public class DoorCompletion : MonoBehaviour
    {
        public DoorDestination destination;

        private void Start()
        {
            switch (destination)
            {
                case DoorDestination.Joker:
                    gameObject.SetActive(SaveManager.GetSaveData().HasBeatJoker);
                    break;
                case DoorDestination.The_Hand:
                    gameObject.SetActive(SaveManager.GetSaveData().HasBeatSnooker);
                    break;
                default:
                    gameObject.SetActive(false);
                    break;
            }
        }
    }
}