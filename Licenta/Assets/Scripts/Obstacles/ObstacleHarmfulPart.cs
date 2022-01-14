using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleHarmfulPart : MonoBehaviour {
    public float damage;
    public bool hasSideEffect;
    public Effects effect;

    private void OnTriggerEnter(Collider other) {
        if(other.transform.CompareTag("Player")) {
            GameEventSystem.instance.PlayerHit(damage);
        }
    }
}

// [ TODO ] Should probably move this somewhere else
public enum Effects {
    Confusion,
    Amnesia,
    Fatigue,
    Dizziness,
    Blindness,
    Poison,
    Bleed,
    Sleep
}
