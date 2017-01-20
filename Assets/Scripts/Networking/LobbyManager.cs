﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using System.Collections;

/**
 * Handles multiplayer lobby, when activated
 */
public class LobbyManager : NetworkLobbyManager {

    static short MsgKicked = MsgType.Highest + 1; //what the hell is this?
    static public LobbyManager instance;

    [Header("Unity UI Lobby")]
    [Tooltip("Time in second between all players ready & match start")]
    public float prematchCountdown = 5.0f;

    //Client numPlayers from NetworkManager is always 0, so we count (throught connect/destroy in LobbyPlayer) the number
    //of players, so that even client know how many player there is.
    [HideInInspector]
    public int _playerNumber = 0;

    //used to disconnect a client properly when exiting the matchmaker
    [HideInInspector]
    public bool _isMatchmaking = false;

    protected bool _disconnectServer = false;

    protected ulong _currentMatchID;

    protected LobbyHook lobbyHook;


    void Start() {
        instance = this;
        lobbyHook = GetComponent<LobbyHook>();
        
        //DontDestroyOnLoad(gameObject); removed bc makes too much
    }

    void Update() {

    }


    /*
     * Server management
     */

    public void AddLocalPlayer() {
        TryToAddPlayer();
    }

    public void RemovePlayer(LobbyPlayer player) {
        player.RemovePlayer();
    }

    public void SimpleBackClbk() {
    }

    public void StopHostClbk() {
        if (_isMatchmaking) {
            matchMaker.DestroyMatch((NetworkID)_currentMatchID, 0, OnDestroyMatch);
            _disconnectServer = true;
        }
        else {
            StopHost();
        }
    }

    public void StopClientClbk() {
        StopClient();

        if (_isMatchmaking) {
            StopMatchMaker();
        }
    }

    public void StopServerClbk() {
        StopServer();
    }

    class KickMsg : MessageBase { }
    public void KickPlayer(NetworkConnection conn) {
        conn.Send(MsgKicked, new KickMsg());
    }
    
    public void KickedMessageHandler(NetworkMessage netMsg) {
        LobbyUIController.instance.SetStatusText("Kicked by server");
        netMsg.conn.Disconnect();
    }

    //===================

    public override void OnStartHost() {
        base.OnStartHost();
    }

    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo) {
        base.OnMatchCreate(success, extendedInfo, matchInfo);
        _currentMatchID = (System.UInt64)matchInfo.networkId;
    }

    public override void OnDestroyMatch(bool success, string extendedInfo) {
        base.OnDestroyMatch(success, extendedInfo);
        if (_disconnectServer) {
            StopMatchMaker();
            StopHost();
        }
    }

    //allow to handle the (+) button to add/remove player
    public void OnPlayersNumberModified(int count) {
        _playerNumber += count;

        int localPlayerCount = 0;
        foreach (PlayerController p in ClientScene.localPlayers)
            localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

        //addPlayerButton.SetActive(localPlayerCount < maxPlayersPerConnection && _playerNumber < maxPlayers);
    }

    // ----------------- Server callbacks ------------------

    //we want to disable the button JOIN if we don't have enough player
    //But OnLobbyClientConnect isn't called on hosting player. So we override the lobbyPlayer creation
    public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId) {

        GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;
        LobbyPlayer newPlayer = obj.GetComponent<LobbyPlayer>();
        //newPlayer.ToggleJoinButton(numPlayers + 1 >= minPlayers);


        for (int i = 0; i < lobbySlots.Length; ++i) {
            LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

            if (p != null) {
                p.RpcUpdateRemoveButton();
                //p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
            }
        }

        return obj;
    }

    public override void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId) {
        for (int i = 0; i < lobbySlots.Length; ++i) {
            LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

            if (p != null) {
                p.RpcUpdateRemoveButton();
                //p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
            }
        }
    }

    public override void OnLobbyServerDisconnect(NetworkConnection conn) {
        for (int i = 0; i < lobbySlots.Length; ++i) {

            LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

            if (p != null) {
                p.RpcUpdateRemoveButton();
                //p.ToggleJoinButton(numPlayers >= minPlayers);
            }
        }

    }

    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer) {
        //This hook allows you to apply state data from the lobby-player to the game-player
        //just subclass "LobbyHook" and add it to the lobby object.

        if (lobbyHook)
            lobbyHook.OnLobbyServerSceneLoadedForPlayer(this, lobbyPlayer, gamePlayer);

        return true;
    }

    // --- Countdown management

    public override void OnLobbyServerPlayersReady() {
        bool allready = true;
        for (int i = 0; i < lobbySlots.Length; ++i) {
            if (lobbySlots[i] != null)
                allready &= lobbySlots[i].readyToBegin;
        }

        if (allready)
            StartCoroutine(ServerCountdownCoroutine());
    }

    public IEnumerator ServerCountdownCoroutine() {
        float remainingTime = prematchCountdown;
        int floorTime = Mathf.FloorToInt(remainingTime);

        while (remainingTime > 0) {
            yield return null;

            remainingTime -= Time.deltaTime;
            int newFloorTime = Mathf.FloorToInt(remainingTime);

            if (newFloorTime != floorTime) {//to avoid flooding the network of message, we only send a notice to client when the number of plain seconds change.
                floorTime = newFloorTime;

                for (int i = 0; i < lobbySlots.Length; ++i) {
                    if (lobbySlots[i] != null) {//there is maxPlayer slots, so some could be == null, need to test it before accessing!
                        (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(floorTime);
                    }
                }
            }
        }

        for (int i = 0; i < lobbySlots.Length; ++i) {
            if (lobbySlots[i] != null) {
                (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(0);
            }
        }

        ServerChangeScene(playScene);
    }


    /*
     * Client Callbacks
     * Called when a client does something`
     */

    public override void OnClientConnect(NetworkConnection conn) {
        base.OnClientConnect(conn);

        conn.RegisterHandler(MsgKicked, KickedMessageHandler);

        if (!NetworkServer.active) {//only to do on pure client (not self hosting client)
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn) {
        base.OnClientDisconnect(conn);
    }

    public override void OnClientError(NetworkConnection conn, int errorCode) {
        LobbyUIController.instance.SetStatusText("Cient error : " + (errorCode == 6 ? "timeout" : errorCode.ToString()));
    }


}