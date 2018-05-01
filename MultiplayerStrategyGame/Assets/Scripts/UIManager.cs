using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

	private GameObject placePawnButton;

	// Use this for initialization
	void Start () {
		placePawnButton = transform.Find ("Button_PlacePawn").gameObject;
	}

	// Update is called once per frame
	void Update () {
		if (GameManager.gameState == GameManager.GameState.battle) {
			placePawnButton.SetActive (false);
		}
	}
}
