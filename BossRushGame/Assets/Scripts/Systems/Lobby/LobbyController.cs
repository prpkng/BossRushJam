using System;
using AYellowpaper.SerializedCollections;
using BRJ;
using BRJ.Systems.Saving;
using BRJ.Systems.Slots.Modifiers;
using Pixelplacement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyController : Singleton<LobbyController>
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
                WorldManager.Instance.Player.transform.position =
                    doors[playerLastEnteredBoss].transform.position + playerDoorSpawnOffset;
            }
        }

        var currentModifierType = SaveManager.GetCurrentModifierType();
        if (currentModifierType != null)
        {
            WorldManager.CurrentActiveModifier = (Modifier)Activator.CreateInstance(currentModifierType);
            print("Current modifier type: " + WorldManager.CurrentActiveModifier.GetType());
        }
    }

    public void LoadBoss(string levelName)
    {
        SceneManager.LoadScene(levelName);

        playerLastEnteredBoss = levelName;
        SaveManager.SetLastEnteredBoss(playerLastEnteredBoss);
    }
}