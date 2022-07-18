using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstHarmfulPart : MonoBehaviour {
    public float damage;
    public ObstActivePart obstActivePart;

    public void ForcedActivation() {
        GameEventSystem.instance.PlayerHit(damage);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.transform.CompareTag("Player") && 
            obstActivePart.GetState() == ObstacleState.sprung_active) {
            GameEventSystem.instance.PlayerHit(damage);
        }
    }
}
