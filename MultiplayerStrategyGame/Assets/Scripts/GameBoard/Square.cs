using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour {

	public Pawn pawn;

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

	// Called when the player clicks on this square
	private void OnMouseDown () {
		if (Pawn.selectedPawn) {
			ExecuteMove ();
		}
	}
}
