using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstHarmfulPart : MonoBehaviour {
    [SerializeField]
    private float damage;
    [SerializeField]
    private bool isIndependent;
    [SerializeField]
    private ObstActivePart obstActivePart;

    // When damage is done without a collision being necessary
    public void ForcedActivation() {
        GameEventSystem.instance.PlayerHit(damage);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.CompareTag("Player")) {
            // If this HarmfulPart depends on an ActivePart
            if (!isIndependent) {
                // Check ActivePart's state
                if (obstActivePart.GetState() == ObstacleState.sprung_active) {
                    GameEventSystem.instance.PlayerHit(damage);
                }
                // If this HarmfulPart is independent, damage the player directly
            } else {
                GameEventSystem.instance.PlayerHit(damage);
            }
        }
    }
}
