using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Square : MonoBehaviour {

	public List<Pawn> pawns;
	public bool active;
	private bool mouseOver;
	private MeshRenderer meshRenderer;

	private void Awake() {
        meshRenderer = GetComponent<MeshRenderer> ();
	}

    // Returns the pawn belonging to the local player
    public Pawn GetPlayerPawn() {
        foreach (Pawn p in pawns) {
            if (p.team == Player.localPlayer.playerID) {
                return p;
            }
        }
        return null;
    }

    // Update position of pawns on this square
    public void UpdatePawnPositions () {
        if (pawns.Count == 1) {
            pawns[0].transform.localPosition = Vector3.zero;
        }
        else if (pawns.Count == 2) {
            if (pawns[0].team == 0) {
                pawns[0].transform.localPosition = Vector3.back * .15f * pawns[0].transform.localScale.z;
                pawns[1].transform.localPosition = Vector3.forward * .15f * pawns[0].transform.localScale.z;
            }
            else {
                pawns[0].transform.localPosition = Vector3.forward * .15f * pawns[0].transform.localScale.z;
                pawns[1].transform.localPosition = Vector3.back * .15f * pawns[0].transform.localScale.z;
            }
        }
    }

    // Called every frame
    private void Update() {
        if (Player.localPlayer) {
                switch (Player.localPlayer.state) {
                case Player.PlayerState.waiting:
                    active = false;
                    break;
                case Player.PlayerState.setup:
                    active = false;
                    break;
                case Player.PlayerState.placingPawn:
                    if (Player.localPlayer.playerID == 0) {
                        active = pawns.Count == 0 && GameBoard.GetCoordinates(this)[1] < GameBoard.setupAreaSize;
                    } else {
                        active = pawns.Count == 0 && GameBoard.GetCoordinates(this)[1] >= GameBoard.GetSize()[1] - GameBoard.setupAreaSize;
                    }
                    break;
                case Player.PlayerState.removingPawn:
                    active = GetPlayerPawn();
                    break;
                case Player.PlayerState.battle:
                    active = GetPlayerPawn();
                    break;
                case Player.PlayerState.movingPawn:
                    if (!Pawn.selectedPawn) {
                        active = false;
                    } else if (GetPlayerPawn()) {
                        active = false;
                    } else if (GameBoard.GetDistance(this, Pawn.selectedPawn.square) > Pawn.selectedPawn.moveDistance) {
                        active = false;
                    } else {
                        active = true;
                    }
                    break;
            }
            ApplyMaterial();
        }
	}

    // Updates the material for visual representation of the squares state, called every frame
    private void ApplyMaterial () {
        if (active) {
            if (mouseOver) {
                meshRenderer.material = Player.localPlayer.GetColor();
            }
            else {
                meshRenderer.material = Resources.Load<Material>("Materials/ActiveSquare");
            }
        }
        else {
            meshRenderer.material = Resources.Load<Material>("Materials/DisabledSquare");
        }
    }

    // Update mouseover state
	private void OnMouseEnter () {
		mouseOver = true;
	}

    // Update mouseover state
    private void OnMouseExit () {
		mouseOver = false;
	}
}
