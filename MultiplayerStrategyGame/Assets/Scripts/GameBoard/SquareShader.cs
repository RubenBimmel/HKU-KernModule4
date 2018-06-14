using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareShader : MonoBehaviour {

    public Color selectedColor;
    public bool selected;
    public float targetSaturation = 1;
    public float targetEmission = 0;

    private static bool gridActive;

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    private float emission = 0;
    private float saturation = 1;
    

    // Use this for initialization
    void Start() {
        _propBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
    }

    // Update is called every frame
    private void Update() {
        emission = Mathf.MoveTowards(emission, targetEmission, Time.deltaTime);
        saturation = Mathf.MoveTowards(saturation, targetSaturation, Time.deltaTime * 5f);
        SetEmission(emission);
        SetSaturation(saturation);

        if (selected) {
            SetColor(selectedColor);
        } else {
            SetColor(Color.white);
        }
    }


    private void SetEmission(float value) {
        // Get the current value of the material properties in the renderer.
        _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
        _propBlock.SetFloat("_EmissionStrength", value);
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
    }

    private void SetColor(Color color) {
        // Get the current value of the material properties in the renderer.
        _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
        _propBlock.SetColor("_Color", color);
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
    }

    private void SetSaturation(float value) {
        // Get the current value of the material properties in the renderer.
        _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
        _propBlock.SetFloat("_Saturation", value);
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
    }
}
