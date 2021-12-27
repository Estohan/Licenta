using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeObjects : MonoBehaviour {
    private MeshRenderer meshRenderer;
    private Color color;

    private void OnCollisionEnter(Collision collision) {
        // Debug.Log("Entered collision with " + collision.gameObject.name);
        meshRenderer = collision.gameObject.transform.GetComponent<MeshRenderer>();
        if(meshRenderer != null) {
            if (meshRenderer.material.GetFloat("_Mode") == 2f) { // 2 - Fade mode
                color = meshRenderer.material.color;
                color.a = 0.5f;
                // Debug.Log(" -----> Fade rendering mode");
                meshRenderer.material.color = color;
            }
        }
    }

    private void OnCollisionExit(Collision collision) {
        // Debug.Log("Left collision with " + collision.gameObject.name);
        meshRenderer = collision.gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null) {
            if (meshRenderer.material.GetFloat("_Mode") == 2f) {
                color = meshRenderer.material.color;
                color.a = 1f;
                meshRenderer.material.color = color;
            }
        }
    }
}
