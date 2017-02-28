using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

/**
 * Network Manager class that adds extra functionality to the game.
 */
public class CustomNetworkManager : NetworkManager {

    [Header("Debug")]
    public bool debug;

    [Header("Meta")] //TODO: make both these private later
    public int score = 0;
    public int numPlayersAlive = 0;

    [Header("Game Modes")]
    public int mode = 0;
    public int level = 1;
    public float survivalLevelTime = 20f;

    [Header("Server Use")]
    public Spawner spawner;

    [Header("Local Use")]
    public UIController ui;

    //Constants
    const int LEVEL_MODE = 0;
    const int SURVIVAL_MODE = 1;

    // called when a client connects 
    public override void OnServerConnect(NetworkConnection conn) {
        print("player connected OnServerConnect");
        base.OnServerConnect(conn);
        ui.RpcShowLargeText("Player connected", 2.0f);
    }

    // called when a client is ready
    public override void OnServerReady(NetworkConnection conn) {
        print("player ready OnServerReady");
        base.OnServerReady(conn);
    }

    // called when a client says they're connected
    public override void OnClientConnect(NetworkConnection conn) {
        base.OnClientConnect(conn);
        print("player connected OnClientConnect");
        ClientScene.AddPlayer(0);
    }

    // called when a new player prefab is added for a client
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        var player = (GameObject)GameObject.Instantiate(
            playerPrefab, playerPrefab.transform.position, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        ClientScene.AddPlayer(conn, (short)(numPlayers));
        print("player added OnServerAddPlayer");
    }

    // called when a player is removed for a client
    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player) {
        if (player != null) {
            NetworkServer.Destroy(player.gameObject);
            print("player removed OnServerRemovePlayer");
        }
    }

    // called when a client disconnects
    public override void OnServerDisconnect(NetworkConnection conn) {
        NetworkServer.DestroyPlayersForConnection(conn);
        print("player disconnected OnServerDisconnect");
    }

    public override void OnClientDisconnect(NetworkConnection conn) {
        base.OnClientDisconnect(conn);
        print("player disconnected OnClientDisconnect");
    }

    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo) {
        base.OnMatchCreate(success, extendedInfo, matchInfo);
        print("match created");
    }



 


    // called when a network error occurs
    public override void OnServerError(NetworkConnection conn, int errorCode) {
        print("Network server error " + errorCode);
    }


    void StartGame() {
        //Set score
        if (PlayerPrefs.HasKey("Score")) {
            AddScore(PlayerPrefs.GetInt("Score")); //score starts at 0
        }
        else {
            PlayerPrefs.SetInt("Score", 0);
            AddScore(0);
        }

        //initiate game mode and level
        if (debug) {
            print("Setting level to 1 by debug");
            mode = SURVIVAL_MODE;
            spawner.SetLevelProperties(1, mode);
            StartCoroutine(IncreaseLevels());
        }
        else if (PlayerPrefs.GetInt("Mode") == LEVEL_MODE) {
            mode = LEVEL_MODE;
            spawner.SetLevelProperties(PlayerPrefs.GetInt("Level"), mode);
            ui.RpcShowLargeText("Level" + level, 0.4f);
        }
        else if (PlayerPrefs.GetInt("Mode") == SURVIVAL_MODE) {
            mode = SURVIVAL_MODE;
            spawner.SetLevelProperties(PlayerPrefs.GetInt("Level"), mode);
            ui.RpcShowLargeText("Survive" + level, 0.4f);
            StartCoroutine(IncreaseLevels());
        }
        else {
            print("Mode is invalid, game will be set to survival mode by default. " + mode);
            mode = SURVIVAL_MODE;
            spawner.SetLevelProperties(1, mode);
            ui.RpcShowLargeText("Survive" + level, 0.4f);
            StartCoroutine(IncreaseLevels());
        }

        spawner.StartSpawning(level, mode);
    }

    void Update() {

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

        //Tell of the UI controllers
        ui.RpcGameOver();
    }


    /**
     * Increases the level
     * Only for survival mode
     */
    private IEnumerator IncreaseLevels() {
        while (true) {
            yield return new WaitForSeconds(survivalLevelTime);
            level++;
            spawner.SetLevelProperties(level, mode);
            PlayerPrefs.SetInt("Level", level);
        }
    }
}
