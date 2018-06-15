using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnSelector : MonoBehaviour {

    public static string activePawn = "Warrior";

    public void SetActivePawn (string pawn) {
        activePawn = pawn;
    }
}
