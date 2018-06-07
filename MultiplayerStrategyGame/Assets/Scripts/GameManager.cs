using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (isServer) {
            CheckPlayerStates();
        }
	}

    // Called when a game is finished
    public static void GameOver (Player winner) {

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
            UpdateGameState();
        }
    }

    // Update and prepare for next turn
    private void UpdateGameState () {
        RpcSetLocalPlayerState((int)Player.PlayerState.updating);
        RpcExecuteAllPlayerMoves();

        foreach (Square square in GameBoard.squares) {
            if (square.pawns.Count == 2) {
                Pawn.Attack(square.pawns[0], square.pawns[1]);
            }
        }

        RpcSetLocalPlayerState((int)Player.PlayerState.battle);
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
