using AYellowpaper.SerializedCollections;
using Game;
using Game.Systems.Saving;
using Pixelplacement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyController : Singleton<LobbyController> {
    private string playerLastEnteredBoss = null;
    public Vector3 playerDoorSpawnOffset = Vector3.down * 4;

    [SerializedDictionary("Name", "Door")]
    public SerializedDictionary<string, GameObject> doors;

    private void Awake() {
        var _lastEnteredBoss = SaveManager.GetLastEnteredBoss();
        if (_lastEnteredBoss != "")
            playerLastEnteredBoss = _lastEnteredBoss;
    }

    private void Start() {
        if (playerLastEnteredBoss == null) return;

        if (doors.ContainsKey(playerLastEnteredBoss)) {
            GameManager.Instance.Player.transform.position = 
                doors[playerLastEnteredBoss].transform.position + playerDoorSpawnOffset;
        }
    }

    public void LoadBoss(string levelName) {

        playerLastEnteredBoss = levelName;
        SaveManager.SetLastEnteredBoss(playerLastEnteredBoss);

        SceneManager.LoadScene(levelName);
    }
}