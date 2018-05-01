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
			Pawn.selectedPawn.Move (this);
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
		switch (GameManager.gameState) {
		case GameManager.GameState.setup:
			switch (GameManager.localState) {
			case GameManager.PlayerState.waiting:
				enabled = false;
				break;
			case GameManager.PlayerState.active:
				enabled = false;
				break;
			case GameManager.PlayerState.placingPawn:
				if (pawn) {
					enabled = false;
				} else {
					enabled = true;
				}
				break;
			}
			break;
		case GameManager.GameState.battle:
			switch (GameManager.localState) {
			case GameManager.PlayerState.waiting:
				enabled = false;
				break;
			case GameManager.PlayerState.active:
				enabled = pawn;
				break;
			case GameManager.PlayerState.movingPawn:
				if ((pawn && pawn.team == Pawn.selectedPawn.team) ||
					GameBoard.GetDistance(this, Pawn.selectedPawn.square) > 2) {
					enabled = false;
				} else {
					enabled = true;
				}
				break;
			}
			break;
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
			case GameManager.PlayerState.active:
				if (GameManager.gameState == GameManager.GameState.battle) {
					if (enabled) {
						Pawn.Select (pawn);
						GameManager.SetPlayerState (GameManager.PlayerState.movingPawn);
					}
				}
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
		mouseOver = true;
	}

	private void OnMouseExit () {
		mouseOver = false;
	}
}
