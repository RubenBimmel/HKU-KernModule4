using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;

public class AutoLobbyManager : NetworkLobbyManager {

	public void StartLobby () {
		if(matchMaker == null) {
			StartMatchMaker();
		}

		matchMaker.ListMatches(0, 10, matchName, true, 0, 0, OnMatchFind);
    }

	//this method is called when a list of matches is returned
	private void OnMatchFind(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        if (success)
        {
			if (matches.Count != 0)
            {
                matchMaker.JoinMatch(matches[0].networkId, "", "", "", 0, 0, OnMatchJoin);
            }
            else
            {
                matchMaker.CreateMatch(matchName, 2, true, "", "", "", 0, 0, OnMatchCreate);
            }
        }
        else
        {
            Debug.LogError("Couldn't connect to match maker");
        }
    }

	//this method is called when your request to join a match is returned
    private void OnMatchJoin(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            MatchInfo hostInfo = matchInfo;
            StartClient(hostInfo);
        }
        else
        {
            Debug.LogError("Join match failed");
        }
    }

	//this method is called when your request for creating a match is returned
    public void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            MatchInfo hostInfo = matchInfo;
            NetworkServer.Listen(hostInfo, 9000);

            StartHost(hostInfo);
        }
        else
        {
            Debug.LogError("Create match failed");
        }
    }
}
