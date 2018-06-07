using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyMenu : MonoBehaviour {

    private AutoLobbyManager alm;
    private Player.PlayerColor color;

    // Called on initialisation
    public void Start() {
        alm = (AutoLobbyManager)NetworkManager.singleton;
    }

    // Called before destroy
    public void OnDestroy() {
        // Set the local players color before going on to the next scene
        // This way the player that joined last also knows what colors all players picked
        if (Player.localPlayer) {
            Player.localPlayer.CmdSetColor(color);
        }
    }

    // Update chosen color
    public void SetColor (UISelect select) {
        color = (Player.PlayerColor) select.value;
    }

    // Break all network activity
    public void StopSearch () {
        alm.StopLobby();
    }

    // Start lobby
	public void StartSearch() {
		alm.StartLobby();
	}

    // Quit to desktop
	public void QuitGame () {
		Application.Quit();
	}
}
