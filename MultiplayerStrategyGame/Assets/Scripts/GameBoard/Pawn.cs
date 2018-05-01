using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour {

	public static Pawn selectedPawn;

	public bool isAlive = true;
	public int team;
	public Square square;

	private MeshRenderer renderer;

	private void Awake () {
		renderer = GetComponentInChildren<MeshRenderer> ();
	}

	private void Update () {
		if (selectedPawn == this) {
			renderer.material = Resources.Load<Material> ("Materials/SelectedSquare");
		} else {
			renderer.material = Resources.Load<Material> ("Materials/ActiveSquare");
		}
	}

	// Move this pawn to a new square
	public void Move (Square targetSquare) {
		if (square) {
			square.pawn = null;
		}
		square = targetSquare;
		transform.parent = targetSquare.transform;
		transform.localPosition = Vector3.zero;
	}

	// Attack another pawn
	public void Attack (Pawn other) {
	}

	// Called when the player clicks on this pawn
	private void OnMouseDown() {
		Select (this);
	}

	// Select a pawn
	public static void Select (Pawn pawn) {
		selectedPawn = pawn;
	}

	public static void Deselect () {
		selectedPawn = null;
	}
}
