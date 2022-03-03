using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstHarmfulPart : MonoBehaviour {
    public float damage;
    public bool hasSideEffect;
    // public SideEffects effect;

    private void OnTriggerEnter(Collider other) {
        if(other.transform.CompareTag("Player")) {
            GameEventSystem.instance.PlayerHit(damage);
        }
    }
}
