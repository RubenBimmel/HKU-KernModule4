using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Square : MonoBehaviour {

	public List<Pawn> pawns;
    public bool canHavePawn;
    [HideInInspector]
	public bool active;

	private bool mouseOver;
    private SquareShader shader;
	//private MeshRenderer meshRenderer;

	private void Awake() {
        shader = GetComponent<SquareShader> ();
        if (Player.localPlayer) {
            shader.selectedColor = Player.localPlayer.GetColor();
        }
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
            if (canHavePawn) {
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
                        }
                        else {
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
                        }
                        else if (GetPlayerPawn()) {
                            active = false;
                        }
                        else if (GameBoard.GetDistance(this, Pawn.selectedPawn.square) > Pawn.selectedPawn.moveDistance) {
                            active = false;
                        }
                        else {
                            active = true;
                        }
                        break;
                }
            } else {
                active = false;
            }
            ApplyMaterial();
        }
	}

    // Updates the material for visual representation of the squares state, called every frame
    private void ApplyMaterial () {
        if (Player.localPlayer.state == Player.PlayerState.waiting 
            || Player.localPlayer.state == Player.PlayerState.updating 
            || Player.localPlayer.state == Player.PlayerState.gameOver) {
            shader.selected = false;
            shader.targetEmission = 0;
            shader.targetSaturation = 1;
        }
        else if (Pawn.selectedPawn && Pawn.selectedPawn.square == this) {
            shader.selected = true;
            shader.targetEmission = 0;
            shader.targetSaturation = 1;
        }
        else if (active) {
            if (mouseOver) {
                shader.selected = false;
                shader.targetEmission = .5f;
                shader.targetSaturation = 1;
            }
            else {
                shader.selected = false;
                shader.targetEmission = 0;
                shader.targetSaturation = 1;
            }
        }
        else {
            shader.selected = false;
            shader.targetEmission = 0;
            shader.targetSaturation = 0;
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
