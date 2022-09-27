using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *      Harmful part component of an obstacle object.
 */
public class ObstHarmfulPart : MonoBehaviour {
    [SerializeField]
    private float damage;
    [SerializeField]
    private bool isIndependent;
    [SerializeField]
    private ObstActivePart obstActivePart;

    // When damage is done without a collision being necessary
    public void ForcedActivation() {
        GameEventSystem.instance.PlayerHealthAffected(- damage, false);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.CompareTag("Player")) {
            // If this HarmfulPart depends on an ActivePart
            if (!isIndependent) {
                // Check ActivePart's state
                if (obstActivePart.GetState() == ObstacleState.sprung_active) {
                    GameEventSystem.instance.PlayerHealthAffected(- damage, false);
                }
                // If this HarmfulPart is independent, damage the player directly
            } else {
                GameEventSystem.instance.PlayerHealthAffected(- damage, false);
            }
        }
    }
}
