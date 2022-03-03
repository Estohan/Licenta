using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_TestSphere : ObstActivePart {
    private Color idleColor;
    private Color activeColor;
    private Color anounceColor;
    private MeshRenderer meshRenderer;

    public override void Start() {
        base.Start();
        idleColor = Color.white;
        activeColor = Color.red;
        anounceColor = Color.green;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public override void Anounce() {
        base.Anounce();
        meshRenderer.material.color = anounceColor;
    }

    public override void Activate() {
        base.Activate();
        meshRenderer.material.color = activeColor;
    }

    public override void Return() {
        base.Return();
        meshRenderer.material.color = idleColor;
    }
}
