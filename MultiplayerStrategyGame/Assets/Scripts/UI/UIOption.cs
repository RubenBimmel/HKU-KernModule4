using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIOption : UIElement {
	public UnityEvent OnSelect;
	public UnityEvent OnDeselect;

    private void OnMouseDown() {
        UISelect select = GetComponentInParent<UISelect>();

        if (!select) {
            Debug.LogError("Option is not parented to a select element");
            return;
        }

        select.Set(this);
    }
}
