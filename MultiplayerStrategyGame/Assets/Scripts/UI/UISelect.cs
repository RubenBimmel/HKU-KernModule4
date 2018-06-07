using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UISelect : MonoBehaviour {
    public int value = 0;

	public List<UIOption> options;

    public UnityEvent OnValueChange;

    public void Set (UIOption option) {
		if (options.Contains(option)) {
			options[value].OnDeselect.Invoke();
			option.OnSelect.Invoke();
			value = options.IndexOf(option);
		}
        OnValueChange.Invoke();
	}
}
