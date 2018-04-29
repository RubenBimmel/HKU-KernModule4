using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIButton : UIElement {
	public UnityEvent OnClick;

	private void OnMouseDown () {
		OnClick.Invoke ();
	}
}
