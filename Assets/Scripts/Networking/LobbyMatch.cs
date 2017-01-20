using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/**
 * Defines the lobby match prefab in the multiplayer server lobby
 */
public class LobbyMatch : MonoBehaviour {

    [Header("Match Info")]
    public ulong matchID;
    public string matchName;
    public int matchNumPlayers;
    const int MAX_PLAYERS = 4; //should tie to lobby manager

    [Header("UI")]
    public Text matchNameText;
    public Text matchPlayerCount;
    public Button matchJoin;

    //Other
    LobbyUIController controller = LobbyUIController.instance;

    void Start() {
        matchJoin.onClick.AddListener(delegate {
            controller.JoinGame(matchID);
        });
    }
	
	void Update () {
        matchNameText.text = matchName;
        matchPlayerCount.text = matchNumPlayers + "/" + MAX_PLAYERS;
	}
}
