using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameBoard : MonoBehaviour {

	public int width;
	public int length;

    public static int setupAreaSize = 3;

	private static int[] size;
	public static Square[] squares;

	// Awake is called on initialization
	private void Awake () {
        // initialise the board
        squares = new Square[width * length];
        size = new int[] { width, length };
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < length; y++) {
                    squares[x + y * width] = Instantiate(Resources.Load<Square>("Prefabs/Square"), transform);
                    squares[x + y * width].transform.localPosition = new Vector3(x + .5f, 0, y + .5f);
                    squares[x + y * width].name = "Square (" + x + ", " + y + ")";
                }
            }
	}

	// Draw board outline so that it is visible inside the editor
	private void OnDrawGizmos () {
		Gizmos.DrawLine (transform.position, transform.position + new Vector3(width, 0, 0));
		Gizmos.DrawLine (transform.position, transform.position + new Vector3(0, 0, length));
		Gizmos.DrawLine (transform.position + new Vector3(width, 0, 0), transform.position + new Vector3(width, 0, length));
		Gizmos.DrawLine (transform.position + new Vector3(0, 0, length), transform.position + new Vector3(width, 0, length));
	}

	// Get a square given its coordinates
	public static Square GetSquare (int x, int y) {
		if (x < 0 || y < 0 || x >= size[0] || y >= size[1]) {
			Debug.LogWarning ("Grid position out of range (" + x + ", " + y + ")");
			return null;
		}
		return squares [x + y * size[0]];
	}

    // Get coordinates for a given square
    public static int[] GetCoordinates (Square s) {
        int i = GetIndex(s);
        if (i >= 0) {
            int x = i % size[0];
            int y = (i - x) / size[0];
            return new int[] { x, y };
        }
		return null;
	}

    // Get size of the board
    public static int[] GetSize () {
        return size;
    }

    // Get index of a given square
    public static int GetIndex (Square s) {
        for (int i = 0; i < squares.Length; i++) {
            if (squares[i] == s) {
                return i;
            }
        }
        return -1;
    }

    // Get distance between squares
	public static int GetDistance (Square s1, Square s2) {
		int[] c1 = GetCoordinates (s1);
		int[] c2 = GetCoordinates (s2);
		if (c1 != null && c2 != null) {
			return Mathf.Max (Mathf.Abs(c1 [0] - c2 [0]), Mathf.Abs(c1 [1] - c2 [1]));
		}
		return int.MaxValue;
	}

    // Return an array of all neighbouring tiles
    public static Square[] GetNeighbours (Square s) {
        List<Square> neighbours = new List<Square>();
        int[] coordinates = GetCoordinates(s);

        Square neighbour;
        neighbour = GetSquare(coordinates[0] + 1, coordinates[1] - 1);
        if (neighbour) neighbours.Add(neighbour);

        neighbour = GetSquare(coordinates[0] + 1, coordinates[1]);
        if (neighbour) neighbours.Add(neighbour);

        neighbour = GetSquare(coordinates[0] + 1, coordinates[1] + 1);
        if (neighbour) neighbours.Add(neighbour);

        neighbour = GetSquare(coordinates[0], coordinates[1] - 1);
        if (neighbour) neighbours.Add(neighbour);

        neighbour = GetSquare(coordinates[0], coordinates[1] + 1);
        if (neighbour) neighbours.Add(neighbour);

        neighbour = GetSquare(coordinates[0] - 1, coordinates[1] - 1);
        if (neighbour) neighbours.Add(neighbour);

        neighbour = GetSquare(coordinates[0] - 1, coordinates[1]);
        if (neighbour) neighbours.Add(neighbour);

        neighbour = GetSquare(coordinates[0] - 1, coordinates[1] + 1);
        if (neighbour) neighbours.Add(neighbour);

        return neighbours.ToArray();
    }
}
