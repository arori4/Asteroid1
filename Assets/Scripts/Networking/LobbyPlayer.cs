using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/**
 * Defines a player to use in the lobby
 */
public class LobbyPlayer : NetworkLobbyPlayer {
    
    public InputField nameInput;

    [Header("Info")]
    public Image playerIcon;
    public Button changeNameButton;
    public Text playerNameText;
    public Toggle readyToggle;
    public Dropdown colorDropdown;
    public Button kickButton;

    //OnMyName function will be invoked on clients when server change the value of playerName
    [SyncVar(hook = "OnMyName")]
    public string playerName = "";
    public bool playerReady = false;

    static Color JoinColor = new Color(255.0f / 255.0f, 0.0f, 101.0f / 255.0f, 1.0f);
    static Color NotReadyColor = new Color(34.0f / 255.0f, 44 / 255.0f, 55.0f / 255.0f, 1.0f);
    static Color ReadyColor = new Color(0.0f, 204.0f / 255.0f, 204.0f / 255.0f, 1.0f);
    static Color TransparentColor = new Color(0, 0, 0, 0);

    private void Awake() {
        //set toggle button override
        readyToggle.onValueChanged.AddListener(OnToggleChange);
        Debug.Log("Its me");
    }

    /**
     * Sets up the player upon entering the lobby.
     */
    public override void OnClientEnterLobby() {
        base.OnClientEnterLobby();

        if (LobbyManager.instance != null) LobbyManager.instance.OnPlayersNumberModified(1);

        LobbyPlayerList._instance.AddPlayer(this);
        LobbyPlayerList._instance.DisplayDirectServerWarning(isServer && LobbyManager.instance.matchMaker == null);

        if (isLocalPlayer) {
            SetupLocalPlayer();
        }
        else {
            SetupOtherPlayer();
        }

        //setup the player data on UI. The value are SyncVar so the player
        //will be created with the right value currently on server
        OnMyName(playerName);
    }

    public override void OnStartAuthority() {
        base.OnStartAuthority();

        //if we return from a game, color of text can still be the one for "Ready"
        readyToggle.transform.GetChild(0).GetComponent<Text>().color = Color.white;

        SetupLocalPlayer();
    }

    /**
     * Sets up the prefab for a different player
     */
    void SetupOtherPlayer() {
        nameInput.interactable = false;
        kickButton.interactable = NetworkServer.active;

        readyToggle.transform.GetChild(0).GetComponent<Text>().text = "...";
        readyToggle.interactable = false;

        OnClientReady(false);
    }

    /**
     * Sets up the prefab for the current player
     */
    void SetupLocalPlayer() {
        nameInput.interactable = true;

        CheckRemoveButton();

        readyToggle.transform.GetChild(0).GetComponent<Text>().text = "JOIN";
        readyToggle.interactable = true;

        //have to use child count of player prefab already setup as "this.slot" is not set yet
        if (playerName == "")
            CmdNameChanged("Player" + (LobbyPlayerList._instance.playerListContentTransform.childCount - 1));

        //we switch from simple name display to name input
        nameInput.interactable = true;

        nameInput.onEndEdit.RemoveAllListeners();
        nameInput.onEndEdit.AddListener(OnNameChanged);

        //readyToggle.onClick.RemoveAllListeners();
        //readyToggle.onClick.AddListener(OnReadyClicked);

        //when OnClientEnterLobby is called, the loval PlayerController is not yet created, so we need to redo that here to disable
        //the add button if we reach maxLocalPlayer. We pass 0, as it was already counted on OnClientEnterLobby
        if (LobbyManager.instance != null) LobbyManager.instance.OnPlayersNumberModified(0);
    }

    //This enable/disable the remove button depending on if that is the only local player or not
    public void CheckRemoveButton() {
        if (!isLocalPlayer)
            return;

        int localPlayerCount = 0;
        foreach (PlayerController p in ClientScene.localPlayers)
            localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

        kickButton.interactable = localPlayerCount > 1;
    }

    /**
     * Sets the ready state of the clients.
     */
    public override void OnClientReady(bool readyState) {
        playerReady = readyState;

        if (readyState) {
            Text textComponent = readyToggle.transform.GetChild(0).GetComponent<Text>();
            textComponent.text = "Ready";
            textComponent.color = ReadyColor;
            nameInput.interactable = false;
        }
        else {
            Text textComponent = readyToggle.transform.GetChild(0).GetComponent<Text>();
            textComponent.text = "Not Ready";
            textComponent.color = NotReadyColor;
            readyToggle.interactable = isLocalPlayer;
            nameInput.interactable = isLocalPlayer;
        }
    }

    ///===== callback from sync var

    public void OnMyName(string newName) {
        playerName = newName;
        nameInput.text = playerName;
    }

    //===== UI Handler

    public void OnReadyClicked() {
        SendReadyToBeginMessage();
    }

    public void OnNameChanged(string str) {
        CmdNameChanged(str);
    }

    public void OnRemovePlayerClick() {
        if (isLocalPlayer) {
            RemovePlayer();
        }
        else if (isServer)
            LobbyManager.instance.KickPlayer(connectionToClient);
    }

    /**
     * Listener to toggle button
     */
    void OnToggleChange(bool val) {
        Debug.Log("Hello world!");
        if (val) {
            SendReadyToBeginMessage();
        }
        else {
            SendNotReadyToBeginMessage();
        }
    }

    [ClientRpc]
    public void RpcUpdateCountdown(int countdown) {
        //LobbyManager.instance.countdownPanel.UIText.text = "Match Starting in " + countdown;
        //LobbyManager.instance.countdownPanel.gameObject.SetActive(countdown != 0);
    }

    [ClientRpc]
    public void RpcUpdateRemoveButton() {
        CheckRemoveButton();
    }

    //====== Server Command

    [Command]
    public void CmdNameChanged(string name) {
        playerName = name;
    }

    //Cleanup thing when get destroy (which happen when client kick or disconnect)
    public void OnDestroy() {
        LobbyPlayerList._instance.RemovePlayer(this);
        if (LobbyManager.instance != null) LobbyManager.instance.OnPlayersNumberModified(-1);

    }
}
