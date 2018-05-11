using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyPlayer : NetworkLobbyPlayer {
	
	public void Start() {
		DontDestroyOnLoad(transform.gameObject);
		SendReadyToBeginMessage();
	}
}
