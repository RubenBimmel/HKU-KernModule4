using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameMenu : MonoBehaviour {

    public bool debugMode;

    public GameObject SetupUI;
    public GameObject MainUI;
    public GameObject GameOverUI;
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

    public void PostScore () {
        StartCoroutine(PostPlayerScore(Player.localPlayer.score));
    }

    IEnumerator PostPlayerScore(int score) {
        //First we create a form to add information to
        WWWForm form = new WWWForm();
        form.AddField("gameid", 6);
        form.AddField("score", score);

        //We're using Post by default, because we're always sending sessionID
        string url = "http://studenthome.hku.nl/~ruben.bimmel/HKU/jaar2dev/Kernmodule4/database/posttosession.php";

        //Pass the current session id if there is one active
        if (session != null) {
            url += "?sessionid=" + session;
        }

        //Post game data
        UnityWebRequest www = UnityWebRequest.Post(url, form);

        //This blocks
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            //Save session id
            session = www.downloadHandler.text;

            //Go to confirmation page
            Application.OpenURL("http://studenthome.hku.nl/~ruben.bimmel/HKU/jaar2dev/Kernmodule4/database/confirmfromgame.php?sessionid=" + session);
        }

        yield return null;
    }

}
