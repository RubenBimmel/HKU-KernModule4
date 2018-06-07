using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour {

    private AutoLobbyManager alm;

    // Called on initialisation
    private void Start() {
        alm = (AutoLobbyManager)UnityEngine.Networking.NetworkManager.singleton;
    }

    // Update player state
    public void PlacePawn () {
        Player.localPlayer.CmdSetState(Player.PlayerState.placingPawn);
	}

    // Update player state
    public void RemovePawn() {
        Player.localPlayer.CmdSetState(Player.PlayerState.removingPawn);
    }

    // Update player state
    public void EndTurn () {
        Player.localPlayer.CmdSetState(Player.PlayerState.waiting);
    }

    // Stop game and break network connection
    public void Quit () {
        alm.StopLobby();
    }

}
