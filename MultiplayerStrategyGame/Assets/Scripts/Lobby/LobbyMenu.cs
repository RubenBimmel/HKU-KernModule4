using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyMenu : MonoBehaviour {

	public void StartGame () {
		AutoLobbyManager alm = (AutoLobbyManager) NetworkManager.singleton;
		alm.StartLobby();
	}

	public void QuitGame () {
		Application.Quit();
	}
}
