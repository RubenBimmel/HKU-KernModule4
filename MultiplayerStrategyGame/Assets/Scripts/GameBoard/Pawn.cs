using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Pawn : NetworkBehaviour {

    public static Pawn selectedPawn;

    public int moveDistance;
    public int strength;
    public int boostToAllPawns;
    public int boostToSamePawns;
    public bool gameOverAtKill;
    public int amount;
    public int points;

    [HideInInspector]
	public bool isAlive = true;
    [HideInInspector]
    public int team;
    [HideInInspector]
    public Square square;
    [HideInInspector]
    public Square targetSquare;


    // Initialise pawns team and position
    [ClientRpc]
    public void RpcInitialise (int playerID, int squareIndex) {
        team = playerID;
        foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) {
            mesh.material = Player.ActivePlayers[playerID].GetColor();
        }
        Move(GameBoard.squares[squareIndex]);
    }

    // Kill this pawn
    public void Kill() {
        if (gameOverAtKill) {
            GameManager.instance.GameOver(Player.ActivePlayers[1 - team]);
        }
        RpcDestroy();
    }

    // Remove this pawn on all clients
    [ClientRpc]
    public void RpcDestroy () {
        square.pawns.Remove(this);
        square.UpdatePawnPositions();
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    // Move this pawn to a new square
    public void CmdMove(int targetSquareID) {
        RpcMove (targetSquareID);
    }

    [ClientRpc]
    // Move this pawn to a new square
    private void RpcMove(int targetSquareID) {
        Move (GameBoard.squares[targetSquareID]);
    }

    // Move pawn to target square
    private void Move(Square target) {
        // Update old square
        if (square) {
            square.pawns.Remove(this);
        }
        if (targetSquare) {
            targetSquare.pawns.Remove(this);
        }

        // Update new square
        target.pawns.Add(this);

        // Update this pawn
        square = target;
        transform.parent = target.transform;
        square.UpdatePawnPositions();
    }

    // Move pawn on client side
    public void PrepareMove (Square target) {
        if (square) {
            square.pawns.Remove(this);
        }

        if (targetSquare) {
            targetSquare.pawns.Remove(this);
        }

        target.pawns.Add(this);
        transform.parent = target.transform;
        target.UpdatePawnPositions();

        targetSquare = target;
    }

    // Return an array of all neighbouring pawns
    public Pawn[] GetNeighbours () {
        List<Pawn> neighbours = new List<Pawn>();
        foreach (Square s in GameBoard.GetNeighbours(square)) {
            foreach (Pawn p in s.pawns) {
                if (p.team == team) {
                    neighbours.Add(p);
                }
            }
        }
        return neighbours.ToArray();
    }

    // Return the current strength of the pawn
    public int GetStrength () {
        int currentStrength = strength;
        foreach(Pawn p in GetNeighbours()) {
            currentStrength += p.boostToAllPawns;
            if (p.name == name) {
                currentStrength += p.boostToSamePawns;
            }
        }
        return currentStrength;
    }

	// Attack another pawn
	public static void Attack (Pawn p1, Pawn p2) {
        Debug.Log("P" + p1.team + ": " + p1.GetStrength() + " VS P" + p2.team + ": " + p2.GetStrength());
        if (p1.GetStrength() < p2.GetStrength()) {
            GameManager.instance.AddScore(p2.team, p1.points);
            p1.Kill();
        }
        else if (p2.GetStrength() < p1.GetStrength()) {
            GameManager.instance.AddScore(p1.team, p2.points);
            p2.Kill();
        }
        else {
            p1.Kill();
            p2.Kill();
        }
	}

	// Called when the player clicks on this pawn
	private void OnMouseDown() {
		Select (this);
	}

	// Select a pawn
	public static void Select (Pawn pawn) {
		selectedPawn = pawn;
	}

    // Deselect pawn
	public static void Deselect () {
		selectedPawn = null;
	}
}
