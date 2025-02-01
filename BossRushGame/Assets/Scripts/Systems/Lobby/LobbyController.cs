using System;
using AYellowpaper.SerializedCollections;
using BRJ;
using BRJ.Systems;
using BRJ.Systems.Saving;
using BRJ.Systems.Slots.Modifiers;
using Cysharp.Threading.Tasks;
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
        }

        public void LoadBoss(string levelName, bool cutscene = true)
        {
            Game.Instance.Transition.TransitionToScene(
                !cutscene ? $"{levelName}Cutscene" : levelName
            ).Forget();

            SaveManager.SetLastEnteredBoss(playerLastEnteredBoss);
            playerLastEnteredBoss = levelName;
        }
    }
}