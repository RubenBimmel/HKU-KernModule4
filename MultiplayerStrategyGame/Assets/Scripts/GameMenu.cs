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
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        //These two pieces are pretty standard (but will depend on your php code!)
        //	First is the sessionID we probably received during our login process, and we send it with each request so php knows who's talking
        //	Then we also add a "request", which tells the php script what we're trying to do
        //		(You could just have different php scripts, and use a different URL for each type of request)
        //formData.Add(new MultipartFormDataSection("sessionID=" + sessionID + "&request=" + request));

        //This is a handy way you can add arguments (in pairs of 2, "&name=value") to the webrequest
        formData.Add(new MultipartFormDataSection("&gameid=6&score=" + score));

        //We're using Post by default, because we're always sending sessionID and request
        UnityWebRequest www = UnityWebRequest.Post("http://studenthome.hku.nl/~ruben.bimmel/HKU/jaar2dev/Kernmodule4/database/confirmscore.php", formData);

        //This blocks
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            Application.OpenURL("http://studenthome.hku.nl/~ruben.bimmel/HKU/jaar2dev/Kernmodule4/database/confirmscore.php");
        }
        /*else {
            //If we're expecting something, we can add a 
            if (callback != null) {
                callback(www.downloadHandler.text);
            }
        }*/

        yield return null;
    }

}
