using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
 
public class UICamera : MonoBehaviour {

	private static Camera cam;
	private static List<UIElement> elements;
	public static Bounds bounds;

	// Use this for initialization
	void Awake () {
		elements = Object.FindObjectsOfType<UIElement> ().ToList();

		if (cam) {
			Debug.LogError ("Multiple UI cameras in scene");
		}
		cam = GetComponent<Camera> ();
	}

	void Start () {
		AlignElements ();
	}

	void Update () {
		Bounds b = GetCameraBounds ();
		if (b != bounds) {
			bounds = b;
			AlignElements ();
		}
	}

	private void AlignElements () {
		foreach (UIElement e in elements) {
			e.Align (bounds);
		}
	}

	private Bounds GetCameraBounds () {
		float height = 2f * cam.orthographicSize;
		float width = height * cam.aspect;
		float length = 2f * -cam.transform.localPosition.z;
		return new Bounds (Vector3.zero, new Vector3(width, height, length));
	}
}
