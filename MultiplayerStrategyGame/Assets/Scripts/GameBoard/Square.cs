using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour {

	public Pawn pawn;
	private bool enabled;
	private bool mouseOver;
	private MeshRenderer renderer;

	private void Awake() {
		renderer = GetComponent<MeshRenderer> ();
	}

	// Gets called when a player moves a pawn to this square
	public void ExecuteMove () {
		
		if (!Pawn.selectedPawn) {
			Debug.LogWarning ("Trying to do a move without a selected pawn");
			return;
		}

		// Move if this square is vacant
		if (!pawn) {
			pawn.Move (this);
			pawn = Pawn.selectedPawn;
			return;
		}

		// Attack if this square is occupied by another player
		if (pawn.team != Pawn.selectedPawn.team) {
			Pawn.selectedPawn.Attack (pawn);
			if (!pawn.isAlive) {
				pawn.Move (this);
				pawn = Pawn.selectedPawn;
			}
			return;
		}

		Debug.LogError ("Impossible move to square occupied by same player");
	}

	public void AddPawn(string type) {
		if (!pawn) {
			pawn = Instantiate (Resources.Load<Pawn> ("Prefabs/" + type));
			pawn.Move (this);
		}
	}

	private void Update() {
		if (GameManager.localState == GameManager.PlayerState.waiting ||
			GameManager.localState == GameManager.PlayerState.active) {
			enabled = false;
		} else if (GameManager.localState == GameManager.PlayerState.placingPawn && pawn) {
			enabled = false;
		} else if (GameManager.localState == GameManager.PlayerState.movingPawn && pawn.team == Pawn.selectedPawn.team) {
			enabled = false;
		} else {
			enabled = true;
		}

		if (enabled) {
			if (mouseOver) {
				renderer.material = Resources.Load<Material> ("Materials/SelectedSquare");
			} else {
				renderer.material = Resources.Load<Material> ("Materials/ActiveSquare");
			}
		} else {
			renderer.material = Resources.Load<Material> ("Materials/DisabledSquare");
		}
	}

	// Called when the player clicks on this square
	private void OnMouseDown () {
		if (enabled) {
			switch (GameManager.localState) {
			case GameManager.PlayerState.placingPawn:
				AddPawn ("Pawn");
				break;
			case GameManager.PlayerState.movingPawn:
				if (Pawn.selectedPawn) {
					ExecuteMove ();
				}
				break;
			}
		}
	}

	private void OnMouseOver () {
		if (GameManager.localState == GameManager.PlayerState.placingPawn) {
			mouseOver = true;
		}
	}

	private void OnMouseExit () {
		mouseOver = false;
	}
}
