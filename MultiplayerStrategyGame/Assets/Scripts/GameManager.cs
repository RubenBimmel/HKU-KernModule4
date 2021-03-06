﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

    public static GameManager instance;

    public float maxTurnTime = 30f;
    public float maxSetupTime = 600f;
    public Player winner;

    private int[] playerScores;
    private int turnCount;
    private float turnTimer;
    private float maxTime;

	// Use this for initialization
	void Start () {
        instance = this;
        playerScores = new int[2];
        turnCount = 0;
        maxTime = maxSetupTime;
    }
	
	// Update is called once per frame
	void Update () {
        if (isServer) {
            turnTimer += Time.deltaTime;
            if (turnTimer > maxTime - 5) {
                RpcShowTimer();
            }
            if (turnTimer > maxTime) {
                RpcHideTimer();
                RpcSetLocalPlayerState((int)Player.PlayerState.waiting);
                turnTimer = float.MinValue;
            }
            CheckPlayerStates();
        }
	}

    // Add score for players
    public void AddScore (int playerID, int amount) {
        if (isServer) {
            playerScores[playerID] += amount;
            RpcUpdateScores(playerScores);
        } else {
            Debug.LogWarning("Trying to add score as a client.");
        }
    }

    // Update score on clients
    [ClientRpc]
    private void RpcUpdateScores (int[] scores) {
        PlayerPrefs.SetInt("score", scores[Player.localPlayer.playerID]);
        GameObject.Find("Score").GetComponent<TMPro.TextMeshPro>().text = scores[Player.localPlayer.playerID].ToString();
    }

    // Show a notification of time
    [ClientRpc]
    private void RpcShowTimer () {
        if (Player.localPlayer.state != Player.PlayerState.waiting) {
            GameObject.Find("OutOfTime").GetComponentInChildren<TMPro.TextMeshPro>(true).gameObject.SetActive(true); // Hacked way to get a referance to an inactive object
        }
    }

    // Hide a notification of time
    [ClientRpc]
    private void RpcHideTimer() {
        GameObject.Find("OutOfTime").GetComponentInChildren<TMPro.TextMeshPro>(true).gameObject.SetActive(false); // Hacked way to get a referance to an inactive object
    }

    // Called when a game is finished
    public void GameOver (Player _winner) {
        winner = _winner;
    }

    // Called at the end of the update when the game is finished
    public void OnGameOver () {
        int winBonus = 2 * (100 - turnCount);
        playerScores[winner.playerID] += winBonus;
        RpcUpdateScores(playerScores);
        RpcSetLocalPlayerState((int)Player.PlayerState.gameOver);
        RpcOpenGameMenu(winner.playerID);
    }

    // Open the gameover menu
    [ClientRpc]
    public void RpcOpenGameMenu (int winner) {
        GameObject.Find("UI").GetComponent<GameMenu>().OnGameOver(winner, playerScores);
    }

    // Check if all players are ready for the next turn
    private void CheckPlayerStates () {
        bool playersAreWaiting = true;
        foreach (Player player in Player.ActivePlayers) {
            if (player.state != Player.PlayerState.waiting) {
                playersAreWaiting = false;
            }
        }

        if (playersAreWaiting) {
            UpdatePlayerStates();
            Invoke("UpdateGameState", .5f);
        }
    }

    // Update all player moves
    private void UpdatePlayerStates () {
        RpcSetLocalPlayerState((int)Player.PlayerState.updating);
        RpcExecuteAllPlayerMoves();
    }

    // Update all attacking pawns and prepare for next turn
    private void UpdateGameState () {
        if (turnCount == 0) { 
            // Setup stage
            maxTime = maxTurnTime; // Max turn time changes in battle state
        } else { 
            // Battle stage
            foreach (Square square in GameBoard.squares) {
                if (square.pawns.Count == 2) {
                    Pawn.Attack(square.pawns[0], square.pawns[1]);
                }
            }

            if (winner) {
                OnGameOver();
            }
        }

        turnCount++;
        turnTimer = 0;
        if (!winner) {
            RpcSetLocalPlayerState((int)Player.PlayerState.battle);
        }
    }

    [ClientRpc]
    // Update local player states
    private void RpcSetLocalPlayerState (int state) {
        Player.localPlayer.CmdSetState((Player.PlayerState)state);
    }

    [ClientRpc]
    // Receive all player moves
    private void RpcExecuteAllPlayerMoves() {
        Player.localPlayer.ExecuteAllMoves();
    }
}
