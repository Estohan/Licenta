using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *      [ TEST CLASS ] Does not affect the game.
 *      Used to check the behavior of different types of collisions.
 */
public class DangerousObject : MonoBehaviour {

    MeshRenderer meshRenderer;

    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnCollisionEnter(Collision collision) {
        // Debug.Log("[ COLLISION ]");
        if (collision.gameObject.CompareTag("Player")) {
            // GameEventSystem.instance.PlayerHit();
            // Debug.Log("Collided with: " + collision.gameObject.name);
            meshRenderer.material.color = Color.green;
        } else {
            // Debug.Log("Collided with: " + collision.gameObject.name);
            meshRenderer.material.color = Color.red;
        }
    }

    private void OnCollisionExit(Collision collision) {
        meshRenderer.material.color = Color.white;
    }

    private void OnTriggerEnter(Collider other) {
        // Debug.Log("[ TRIGGER ]");
        if (other.gameObject.CompareTag("Player")) {
            // GameEventSystem.instance.PlayerHit();
            // Debug.Log("Collided with: " + other.gameObject.name);
            meshRenderer.material.color = Color.green;
        } else {
            // Debug.Log("Collided with: " + other.gameObject.name);
            meshRenderer.material.color = Color.red;
        }
    }
    private void OnTriggerExit(Collider other) {
        meshRenderer.material.color = Color.white;
    }
}
