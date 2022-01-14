using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    public bool canReturn;
    public bool canAnounce;
    public float timeToReturn;
    [Space]
    public bool breakable;
    public float breakChance;
    [Space]
    [SerializeField]
    private ObstacleState state;

    public virtual void Start() {
        state = ObstacleState.idle;
    }

    // Trigger trap
    public void Trigger() {
        if (state == ObstacleState.idle || state == ObstacleState.waiting) {
            // trigger trap/obstacle
            Activate();
            // return to initial state if possible, after timeToDeactivate seconds
            if (canReturn) {
                StartCoroutine(WaitAndReturn(timeToReturn));
            }
        }
    }

    // Anounce that the trap is about to be triggered
    public void AnounceIfPossible() {
        if (state == ObstacleState.idle && canAnounce) {
            Anounce();
        }
    }

    public virtual void Anounce() {
        state = ObstacleState.waiting;
    }

    public virtual void Activate() {
        // Debug.Log(this.transform.name + " is activated");
        // meshRenderer.material.color = activeColor;
        state = ObstacleState.active;
    }

    public virtual void Return() {
        // Debug.Log(this.transform.name + " is returning to idle state");
        // meshRenderer.material.color = idleColor;
        state = ObstacleState.idle;
    }

    private IEnumerator WaitAndReturn(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        Return();
    }
}

public enum ObstacleState {
    idle,
    waiting,
    active,
    broken
}
