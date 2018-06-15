using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;

public class AutoLobbyManager : NetworkManager {

    public float resetTime;

    private bool searching;
    private float timer;

    // Called every frame
    private void Update() {
        // Stop lobby after reset time
        if (searching) {
            timer += Time.deltaTime;
            if (timer > resetTime) {
                StopLobby();
            }
        }
    }

    //use this method to start match making
    public void StartLobby() {
        timer = 0f;
        searching = true;

        if (matchMaker == null) {
            StartMatchMaker();
        }

        matchMaker.ListMatches(0, 10, matchName, true, 0, 0, OnMatchList);
    }

    //use this method to stop match making
    public void StopLobby() {
        StopClient();
        StopHost();
        StopMatchMaker();
        StopServer();
    }

    //this method is called when a list of matches is returned
    public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList) {
        if (success) {
            if (matchList.Count != 0) {
                foreach (MatchInfoSnapshot match in matchList) {
                    if (match.currentSize != 0) {
                        matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, OnMatchJoined);
                    }
                }
            }
            else {
                matchMaker.CreateMatch(matchName, 2, true, "", "", "", 0, 0, OnMatchCreate);
            }
        }
        else {
            Debug.LogError("Couldn't connect to match maker");
        }
    }

    //this method is called when your request to join a match is returned
    public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo) {
        if (success) {
            MatchInfo hostInfo = matchInfo;
            StartClient(hostInfo);
        }
        else {
            Debug.LogError("Join match failed");
        }
    }

    //this method is called when your request for creating a match is returned
    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo) {
        if (success) {
            MatchInfo hostInfo = matchInfo;
            NetworkServer.Listen(hostInfo, 9000);

            StartHost(hostInfo);
        }
        else {
            Debug.LogError("Create match failed");
        }
    }

    //this method is called when a new player is added
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        if (SceneManager.GetActiveScene().name == "Lobby") {
            GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

            if (CheckReadyToBegin()) {
                searching = false;
                StartGame();
            }
        }
    }

    //check if valid player count equals the match size count
    public bool CheckReadyToBegin() {
        int readyCount = 0;

        foreach (var conn in NetworkServer.connections) {
            if (conn == null) {
                Debug.LogError("No connection");
                return false;
            }

            if (conn.playerControllers.Count != 1) {
                Debug.LogError("No player");
                return false;
            }

            if (conn.playerControllers[0].IsValid) {
                readyCount++;
            }
        }

        return (readyCount == matchSize);
    }

    //this method is called when the match is ready to begin
    public void StartGame () {
        ServerChangeScene("Main");
    } 
}
