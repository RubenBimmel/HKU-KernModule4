using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderTest : MonoBehaviour {

    private static Color selectedColor;
    private static ShaderTest selected;
    private static bool gridActive;

    public bool active;

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    private float emission = 0;
    private float targetEmission = 0;

    private float saturation = 1;
    private float targetSaturation = 1;

    // Use this for initialization
    void Start () {
        _propBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
        selectedColor = Color.red;
    }

    // Update is called every frame
    private void Update() {
        emission = Mathf.MoveTowards(emission, targetEmission, Time.deltaTime);
        saturation = Mathf.MoveTowards(saturation, targetSaturation, Time.deltaTime * 5f);
        SetEmission(emission);
        SetSaturation(saturation);

        if (Input.GetKeyDown("s")) {
            SetGridActiveState(true);
        }
        if (Input.GetKeyDown("e")) {
            SetGridActiveState(false);
        }
    }

    void OnMouseEnter () {
        if (gridActive && active) {
            targetEmission = .3f;
        }
    }

    void OnMouseExit() {
        if (gridActive && active) {
            targetEmission = 0f;
        }
    }

    void OnMouseDown() {
        if (gridActive && active) {
            Select(this);
        }
    }

    // Activate the grid
    public void SetGridActiveState (bool state) {
        gridActive = state;
        if (state == true) {
            if (active == false) {
                targetSaturation = 0;
            }
        } else {
            targetEmission = 0;
            targetSaturation = 1;
            SetColor(Color.white);
        }
    }

    // Select a tile
    private static void Select (ShaderTest selection) {
        Deselect();
        selected = selection;
        selection.SetColor(selectedColor);
    }

    // Deselect the current selected tile
    private static void Deselect() {
        if (selected) {
            selected.SetColor(Color.white);
        }
        selected = null;
    }

    private void SetEmission (float value) {
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

    private void SetSaturation (float value) {
        // Get the current value of the material properties in the renderer.
        _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
        _propBlock.SetFloat("_Saturation", value);
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
    }
}
