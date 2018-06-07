using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElement : MonoBehaviour {

	public enum VerticalAlign {
		top,
		center,
		bottom
	}

	public enum HorizontalAlign {
		left,
		center,
		right
	}

	public VerticalAlign vAlign = VerticalAlign.center;
	public HorizontalAlign hAlign = HorizontalAlign.center;

	private Vector3 offset;

	private void Awake () {
		offset = transform.localPosition;
        Align(UICamera.bounds);
	}

	// Aligns the camera
	public void Align (Bounds cameraBounds) {
		Vector3 position = offset;

		switch (vAlign) {
		case VerticalAlign.top:
			position.y = cameraBounds.extents.y + position.y;
			break;
		case VerticalAlign.bottom:
			position.y = -cameraBounds.extents.y + position.y;
			break;
		}

		switch (hAlign) {
		case HorizontalAlign.left:
			position.x = -cameraBounds.extents.x + position.x;
			break;
		case HorizontalAlign.right:
			position.x = cameraBounds.extents.x + position.x;
			break;
		}

		transform.localPosition = position;
	}
}
