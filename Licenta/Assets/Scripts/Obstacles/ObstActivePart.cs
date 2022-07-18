using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstActivePart : MonoBehaviour {

    public bool canReturn;
    public bool canAnounce;
    public float timeToReturn;
    [Space]
    [SerializeField]
    private ObstacleState state;

    public virtual void Start() {
        state = ObstacleState.idle;
    }

    public ObstacleState GetState() {
        return state;
    }

    // Trigger trap
    public void Trigger() {
        if (state == ObstacleState.idle || state == ObstacleState.sprung_waiting) {
            // trigger trap/obstacle
            Activate();
            // return to initial state if possible, after timeToDeactivate seconds
            if (canReturn) {
                StartCoroutine(WaitAndReturn(timeToReturn));
            } else {
                state = ObstacleState.broken;
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

    private IEnumerator WaitAndReturn(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        Return();
    }

}

public enum ObstacleState {
    idle,
    sprung_waiting,
    sprung_active,
    broken
}
