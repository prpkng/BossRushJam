using System;
using AYellowpaper.SerializedCollections;
using BRJ;
using BRJ.Systems;
using BRJ.Systems.Saving;
using BRJ.Systems.Slots.Modifiers;
using Pixelplacement;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace BRJ
{
    public class LobbyController : MonoBehaviour
    {
        private string playerLastEnteredBoss = null;
        public Vector3 playerDoorSpawnOffset = Vector3.down * 4;

        [SerializedDictionary("Name", "Door")]
        public SerializedDictionary<string, GameObject> doors;

        private void Awake()
        {
            var _lastEnteredBoss = SaveManager.GetLastEnteredBoss();
            if (_lastEnteredBoss != "")
                playerLastEnteredBoss = _lastEnteredBoss;
        }

        private void Start()
        {
            if (playerLastEnteredBoss != null)
            {
                if (doors.ContainsKey(playerLastEnteredBoss))
                {
                    Game.Instance.World.Player.transform.position =
                        doors[playerLastEnteredBoss].transform.position + playerDoorSpawnOffset;
                }
            }

            var currentModifierType = SaveManager.GetCurrentModifierType();
            if (currentModifierType != null)
            {
                Game.Instance.World.CurrentActiveModifier = (Modifier)Activator.CreateInstance(currentModifierType);
                print("Current modifier type: " + Game.Instance.World.CurrentActiveModifier.GetType());
            }
        }

        public void LoadBoss(string levelName)
        {
            Game.Instance.Transition.TransitionToScene(levelName);

            playerLastEnteredBoss = levelName;
            SaveManager.SetLastEnteredBoss(playerLastEnteredBoss);
        }
    }
}