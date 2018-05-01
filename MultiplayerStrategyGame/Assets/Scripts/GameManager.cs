using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public enum GameState {
		setup,
		battle
	}

	public enum PlayerState {
		waiting,
		active,
		placingPawn,
		movingPawn
	}
		
	public static GameState gameState;
	public static PlayerState localState;

	// Use this for initialization
	void Start () {
		localState = PlayerState.active;
		gameState = GameState.setup;
	}
	
	// Update is called once per frame
	void Update () {
		switch (gameState) {
		case GameState.setup:
			if (localState != PlayerState.waiting) {
				if (Input.GetButtonDown ("Cancel")) {
					localState = PlayerState.active;
				}
			} else {
				gameState = GameState.battle;
				localState = PlayerState.active;
			}
			break;
		case GameState.battle:
			if (localState != PlayerState.waiting) {
				if (Input.GetButtonDown ("Cancel")) {
					localState = PlayerState.active;
					Pawn.Deselect ();
				}
			}
			break;
		}
	}

	public static void SetPlayerState (PlayerState state) {
		localState = state;
	}

	// Used for UnityEvents
	public void SetPlayerState (string state) {
		SetPlayerState((PlayerState) System.Enum.Parse(typeof(PlayerState), state));
	}
}
