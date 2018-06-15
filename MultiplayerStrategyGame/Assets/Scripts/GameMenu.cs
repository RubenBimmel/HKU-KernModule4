using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameMenu : MonoBehaviour {

    public bool debugMode;

    public GameObject SetupUI;
    public GameObject MainUI;
    public GameObject GameOverUI;

    public GameObject SetupCollider; //Collider to prevent player from pressing objects inside the scene when pressing on UI buttons

    private AutoLobbyManager alm;

    private static string session;

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
        if (!debugMode) {
            if (Player.localPlayer.state == Player.PlayerState.setup ||
                Player.localPlayer.state == Player.PlayerState.placingPawn ||
                Player.localPlayer.state == Player.PlayerState.removingPawn) {
                if (!Player.localPlayer.spawnAllPawns) {
                    return;
                }
            }
        }

        SetupUI.SetActive(false);
        SetupCollider.SetActive(false);
        Player.localPlayer.CmdSetState(Player.PlayerState.waiting);
    }

    // Stop game and break network connection
    public void Quit () {
        alm.StopLobby();
    }

    // Called at the end of the game
    public void OnGameOver (int winner, int[] scores) {
        MainUI.SetActive(false);
        GameOverUI.SetActive(true);
        if (winner == Player.localPlayer.playerID) {
            GameOverUI.transform.Find("Result").GetComponent<TMPro.TextMeshPro>().text = "You won!";
        } else {
            GameOverUI.transform.Find("Result").GetComponent<TMPro.TextMeshPro>().text = "You lost!";
        }
        GameOverUI.transform.Find("EndScore").GetComponent<TMPro.TextMeshPro>().text = scores[Player.localPlayer.playerID].ToString();
    }

    public void Post () {
        PostScore.GetInstance().Post();
    }

}
