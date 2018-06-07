using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnSelector : MonoBehaviour {

    public static string activePawn = "Warrior";

    private GameObject pawns;



    private void Start() {
        pawns = transform.Find("Pawns").gameObject;
    }

    public void ChangeState () {
        pawns.SetActive(!pawns.activeSelf);
    }

    public void SetActivePawn (string pawn) {
        activePawn = pawn;
    }
}
