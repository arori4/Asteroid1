using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

/**
 * Network Manager class that adds extra functionality to the game.
 * Contains both code to manage the network and game logic b/c they are implicitly tied together.
 */
public class CustomNetworkManager : NetworkManager {
    
    [Header("Debug")]
    public bool debug;

    [Header("Player")]
    public GameObject deadPlayerPrefab;

    [Header("Meta")] //TODO: make both these private later
    public int score = 0;
    public int numPlayersAlive = 0;

    [Header("Server Use")]
    public Spawner spawner;

    [Header("Local Use")]
    public UIController ui;

    public override void OnStartServer() {
        base.OnStartServer();
        StartGame();
    }

    // called when a client connects 
    public override void OnServerConnect(NetworkConnection conn) {
        base.OnServerConnect(conn);
        ui.RpcShowLargeText("Player connected", 2.0f);
    }

    // called when a client is ready
    public override void OnServerReady(NetworkConnection conn) {
        base.OnServerReady(conn);
    }

    // called when a client says they're connected
    public override void OnClientConnect(NetworkConnection conn) {
        base.OnClientConnect(conn);
        ClientScene.AddPlayer(0);
    }

    // called when a new player prefab is added for a client
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        var player = (GameObject)GameObject.Instantiate(
            playerPrefab, playerPrefab.transform.position, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        ClientScene.AddPlayer(conn, (short)(numPlayers));
        PlayerAdded();
    }

    // called when a player is removed for a client
    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player) {
        if (player != null) {
            NetworkServer.Destroy(player.gameObject);
        }
    }

    // called when a client disconnects
    public override void OnServerDisconnect(NetworkConnection conn) {
        NetworkServer.DestroyPlayersForConnection(conn);
    }

    public override void OnClientDisconnect(NetworkConnection conn) {
        base.OnClientDisconnect(conn);
    }

    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo) {
        base.OnMatchCreate(success, extendedInfo, matchInfo);
    }

    // called when a network error occurs
    public override void OnServerError(NetworkConnection conn, int errorCode) {
        print("Network server error " + errorCode);
    }
    
    /**
     * Starts the game. Sets score and starts spawner.
     */
    private void StartGame() {
        //Set score
        if (PlayerPrefs.HasKey("Score")) {
            score = PlayerPrefs.GetInt("Score");
        }
        else {
            PlayerPrefs.SetInt("Score", 0);
            score = 0;
        }

        //Start Spawner
        spawner.StartGame();
    }

    /**
     * Player Management
     */

    public void PlayerAdded() {
        numPlayersAlive++;
    }

    public void PlayerKilled(NetworkConnection connection, GameObject playerObj) {
        //Replace player
        NetworkServer.ReplacePlayerForConnection(connection, 
            deadPlayerPrefab, 0);
        //Kill old prefab
        Destroy(playerObj);
        NetworkServer.Destroy(playerObj);

        numPlayersAlive--;
        if (numPlayersAlive <= 0) {
            GameOver();
        }
    }

    /**
     * Adds score and updates UI for all players
     */
    public void AddScore(int scoreToAdd) {
        score += scoreToAdd;
        if (ui.enabled) {
            ui.RpcSetScoreText(score);
        }
    }

    public void SplashText(string text, float duration) {
        ui.RpcShowLargeText(text, duration);
    }

    /**
     * Game States
     */

    public void AdvanceLevel() {
        //Add level and score
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
        PlayerPrefs.SetInt("Score", score);

        //Tell all of the UI controllers
        ui.RpcAdvanceLevel();
    }

    public void GameOver() {
        GetComponent<GameSave>().SaveHighScore(score, 1); //for now, only 1 level

        //set the level back to 1
        PlayerPrefs.SetInt("Level", 1);
        PlayerPrefs.SetInt("Score", 0);

        //Stop spawning
        spawner.StopAllCoroutines();

        //Tell of the UI controllers
        ui.RpcGameOver();
    }



}
