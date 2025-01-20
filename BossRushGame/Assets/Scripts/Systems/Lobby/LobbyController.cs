using AYellowpaper.SerializedCollections;
using Game;
using Pixelplacement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyController : Singleton<LobbyController> {
    private string playerLastEnteredBoss = null;
    public Vector3 playerDoorSpawnOffset = Vector3.down * 4;

    [SerializedDictionary("Name", "Door")]
    public SerializedDictionary<string, GameObject> doors;

    private void Awake() {
        if (PlayerPrefs.HasKey("LastEnteredBoss")) {
            playerLastEnteredBoss = PlayerPrefs.GetString("LastEnteredBoss");
        }
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
        PlayerPrefs.SetString("LastEnteredBoss", levelName);

        SceneManager.LoadScene(levelName);
    }
}