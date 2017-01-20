using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/**
 * Local list of players in the lobby
 */
[RequireComponent(typeof(VerticalLayoutGroup))]
public class LobbyPlayerList : MonoBehaviour {
    public static LobbyPlayerList _instance = null;

    public RectTransform playerListContentTransform;
    public GameObject warningDirectPlayServer;

    protected VerticalLayoutGroup _layout;
    protected List<LobbyPlayer> list_players = new List<LobbyPlayer>();

    /**
     * Get components
     */
    public void OnEnable() {
        _instance = this;
        _layout = playerListContentTransform.GetComponent<VerticalLayoutGroup>();
    }

    public void DisplayDirectServerWarning(bool enabled) {
        if (warningDirectPlayServer != null)
            warningDirectPlayServer.SetActive(enabled);
    }

    void Update() {
        //this dirty the layout to force it to recompute evryframe (a sync problem between client/server
        //sometime to child being assigned before layout was enabled/init, leading to broken layouting)
        //TODO: could be updated in a coroutine to use less frames
        if (_layout)
            _layout.childAlignment = Time.frameCount % 2 == 0 ? TextAnchor.UpperCenter : TextAnchor.UpperLeft;
    }

    /**
     * Adds a player to the list
     */
    public void AddPlayer(LobbyPlayer player) {
        if (list_players.Contains(player))
            return;

        list_players.Add(player);

        player.transform.SetParent(playerListContentTransform, false);

        PlayerListModified();
    }

    /**
     * Removes a player from the list
     */
    public void RemovePlayer(LobbyPlayer player) {
        list_players.Remove(player);
        PlayerListModified();
    }

    /**
     * Does nothing for now
     * On old code, this changed the color of the players.
     * Not usefeul for now, but code exists for future use
     */
    public void PlayerListModified() {
        int i = 0;
        foreach (LobbyPlayer p in list_players) {
            //p.OnPlayerListChanged(i);
            ++i;
        }
    }
}
