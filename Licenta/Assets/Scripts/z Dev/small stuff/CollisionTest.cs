using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTest : MonoBehaviour {
    public bool usingOnTrigger;
    MeshRenderer meshRenderer;

    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnCollisionEnter(Collision collision) {
        if(!usingOnTrigger)
            meshRenderer.material.color = Color.red;
    }

    private void OnCollisionExit(Collision collision) {
        if (!usingOnTrigger)
            meshRenderer.material.color = Color.white;
    }

    private void OnTriggerEnter(Collider other) {
        if (usingOnTrigger)
            meshRenderer.material.color = Color.blue;
    }

    private void OnTriggerExit(Collider other) {
        if (usingOnTrigger)
            meshRenderer.material.color = Color.white;
    }
}
