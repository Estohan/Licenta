using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstActivePart : MonoBehaviour {

    [SerializeField]
    protected bool canReturn;
    [SerializeField]
    protected bool canAnounce;
    [SerializeField]
    private float timeToReturn;
    [SerializeField]
    private float timeActive;
    [Space]
    [SerializeField]
    protected ObstacleState state;

    public virtual void Start() {
        // state = ObstacleState.idle;
    }

    public ObstacleState GetState() {
        return state;
    }

    // Trigger trap
    public void Trigger() {
        if (state == ObstacleState.idle || state == ObstacleState.sprung_waiting) {
            // Trigger trap/obstacle
            Activate();
            // Return to initial state if possible, after timeToDeactivate seconds
            if (canReturn) {
                StartCoroutine(WaitAndReturn(timeToReturn));
            // Or stay active for timeActive seconds and then become broken
            } else {
                StartCoroutine(WaitAndBreak(timeActive));
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
        state = ObstacleState.sprung_waiting;
    }

    public virtual void Activate() {
        state = ObstacleState.sprung_active;
    }

    public virtual void Return() {
        state = ObstacleState.idle;
    }

    public virtual void Break() {
        state = ObstacleState.broken;
    }

    private IEnumerator WaitAndReturn(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        Return();
    }

    private IEnumerator WaitAndBreak(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        Break();
    }
}

public enum ObstacleState {
    idle,
    sprung_waiting,
    sprung_active,
    broken
}
