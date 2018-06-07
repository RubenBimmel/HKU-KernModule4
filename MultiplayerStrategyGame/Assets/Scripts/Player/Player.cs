using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour {

	public enum PlayerColor {
        Blue,
        Green,
        Indigo,
        Orange,
        Pink,
        Red
    }

    public enum PlayerState {
        lobbying,
        waiting,
        setup,
        battle,
        updating,
        placingPawn,
        removingPawn,
        movingPawn,
        gameOver
    }

    public static Player localPlayer;
    public static List<Player> ActivePlayers = new List<Player>();
    public int playerID { get { return ActivePlayers.IndexOf(this); } }

    private Transform cam;
    private int movementScreenBorderSize = 20;
    private float movementExtents = 5f;
    private float moveSpeed = 8f;

    private List<Pawn> spawnedPawns;
    private List<Pawn> movedPawns;

    [SyncVar]
    public PlayerColor color;
    [SyncVar]
    public PlayerState state;

    // Called on initialisation
    public void Start() {
        ActivePlayers.Add(this);
        SceneManager.sceneLoaded += SetLevelState;

        if (isLocalPlayer) {
            localPlayer = this;
        }

        spawnedPawns = new List<Pawn>();
        movedPawns = new List<Pawn>();
        DontDestroyOnLoad(this);
    }

    // Called every frame
    public void Update() {
        if (isLocalPlayer) {
            if (state != PlayerState.lobbying) {
                UpdateInput();
            }
        }
    }

    // Called before being destroyed
    public void OnDestroy() {
        ActivePlayers.Remove(this);
        SceneManager.sceneLoaded -= SetLevelState;

        if (isLocalPlayer) {
            localPlayer = null;
        }
    }


    // Called whenever a new scene is loaded
    public void SetLevelState (Scene scene, LoadSceneMode mode) {
        if (isLocalPlayer) {
            switch (scene.buildIndex) {
                case 0:
                    localPlayer.CmdSetState(PlayerState.lobbying);
                    break;
                case 1:
                    localPlayer.CmdSetState(PlayerState.setup);
                    InitialiseGame();
                    break;
            }
        }
    }

    // Called when the main scene is loaded
    public void InitialiseGame () {
        // Get reference to the main camera target
        Camera[] cameras = Camera.allCameras;
        foreach (Camera c in cameras) {
            if (c.name == "MainCamera") {
                cam = c.transform.parent;
            }
        }

        // Set camera orientation for this player so that players are facing eachother
        if (playerID != 0) {
            cam.transform.Rotate(Vector3.up, 180);
        }
    }

    // Return the players color as a material
    public Material GetColor() {
        return Resources.Load<Material>("Materials/Player_" + color.ToString());
    }

    // Execute all moves
    public void ExecuteAllMoves() {
        foreach (Pawn p in movedPawns) {
            p.CmdMove(GameBoard.GetIndex(p.targetSquare));
        }
        movedPawns.Clear();
    }

    // Set color for this player
    [Command]
    public void CmdSetColor(PlayerColor c) {
        color = c;
    }

    // Set state for this player
    [Command]
    public void CmdSetState(PlayerState newState) {
        state = newState;
    }

    // Called every frame within the main scene
    private void UpdateInput() {
        // Input is only applied to the local player
        if (isLocalPlayer) {
            // Detect mouse click
            if (Input.GetMouseButtonDown(0)) {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100.0f)) {

                    // Check if the player clicked on a square
                    Square hitSquare = hit.transform.GetComponent<Square>();
                    if (hitSquare) {
                        SelectSquare(hitSquare);
                    }
                }
            }

            // Update camera movement
            if (Input.mousePosition.x < movementScreenBorderSize) {
                MoveCamera(Vector3.left);
            }
            else if (Input.mousePosition.x > Screen.width - movementScreenBorderSize) {
                MoveCamera(Vector3.right);
            }
            if (Input.mousePosition.y < movementScreenBorderSize) {
                MoveCamera(Vector3.back);
            }
            else if (Input.mousePosition.y > Screen.height - movementScreenBorderSize) {
                MoveCamera(Vector3.forward);
            }

            // Update cancel button
            if (Input.GetButtonDown("Cancel")) {
                if (state == PlayerState.placingPawn || state == PlayerState.removingPawn) {
                    localPlayer.CmdSetState(PlayerState.setup);
                }
                else if (state == PlayerState.movingPawn) {
                    localPlayer.CmdSetState(PlayerState.battle);
                    Pawn.Deselect();
                }
            }
        }
    }

    // Update camera movement
    private void MoveCamera (Vector3 direction) {
        if (cam) {
            // Update camera position with movement relative to camera angle
            Vector3 position = cam.transform.position + cam.transform.InverseTransformVector(direction * moveSpeed * Time.deltaTime);

            // Clamp position values
            position.x = Mathf.Clamp(position.x, -movementExtents, movementExtents);
            position.y = cam.transform.localPosition.y;
            position.z = Mathf.Clamp(position.z, -movementExtents, movementExtents);

            // Update camera position
            cam.transform.position = position;
        }
    }

    // Called when player clicks on a square
    private void SelectSquare(Square square) {
        if (square.active) {
            // Index of the square inside the gameboards array, used for command functions
            int index = GameBoard.GetIndex(square);

            switch (state) {
                case PlayerState.placingPawn:
                    Pawn pawn = Resources.Load<Pawn>("Prefabs/" + PawnSelector.activePawn);
                    // Check how many pawns are spawned
                    int count = 0;
                    foreach (Pawn p in spawnedPawns) {
                        if (p.name == pawn.name) {
                            count++;
                        }
                    }

                    // Add pawn if available
                    if (count < pawn.amount) {
                        spawnedPawns.Add(pawn);
                        CmdAddPawn(PawnSelector.activePawn, index, playerID);
                    }
                    break;
                case PlayerState.removingPawn:
                    spawnedPawns.Remove(GameBoard.squares[index].pawns[0]);
                    CmdRemovePawn(index);
                    break;
                case PlayerState.battle:
                    if (square.active) {
                        Pawn.Select(square.GetPlayerPawn());
                        localPlayer.CmdSetState(PlayerState.movingPawn);
                    }
                    break;
                case PlayerState.movingPawn:
                    if (Pawn.selectedPawn) {
                        Pawn.selectedPawn.PrepareMove(square);
                        movedPawns.Add(Pawn.selectedPawn);
                    }
                    break;
            }
        }
    }

    // Add a pawn for this player
    [Command]
    private void CmdAddPawn (string type, int squareIndex, int team) {
        Pawn pawn = Instantiate(Resources.Load<Pawn>("Prefabs/" + type));
        NetworkServer.SpawnWithClientAuthority(pawn.gameObject, gameObject);
        pawn.RpcInitialise(team, squareIndex);
    }

    // Remove a pawn from this player
    [Command]
    private void CmdRemovePawn(int squareIndex) {
        GameBoard.squares[squareIndex].pawns[0].RpcDestroy();
    }
}
