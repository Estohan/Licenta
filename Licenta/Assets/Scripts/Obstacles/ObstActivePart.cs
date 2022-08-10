using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstActivePart : MonoBehaviour {

    [SerializeField]
    protected bool canReturn;
    [SerializeField]
    protected bool canAnounce;
    [SerializeField]
    private float activeTime;
    [SerializeField]
    private float returnTime;
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
    public virtual void Trigger() {
        if ((!canAnounce && state == ObstacleState.idle) || state == ObstacleState.sprung_waiting) {
            // Trigger trap/obstacle
            StartCoroutine(WaitActiveCoroutine(activeTime));
            // Return to initial state if possible, after timeToDeactivate seconds
            if (canReturn) {
                StartCoroutine(WaitToReturnCoroutine(returnTime));
            // Or stay active for timeActive seconds and then become broken
            } else {
                StartCoroutine(WaitToBreakCoroutine(returnTime));
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

    public virtual void Recover() {
        state = ObstacleState.sprung_returning;
    }

    public virtual void Return() {
        state = ObstacleState.idle;
    }

    public virtual void Break() {
        state = ObstacleState.broken;
    }

    private IEnumerator WaitToReturnCoroutine(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        Return();
    }

    private IEnumerator WaitToBreakCoroutine(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        Break();
    }

    private IEnumerator WaitActiveCoroutine(float waitTime) {
        Activate();
        yield return new WaitForSeconds(waitTime);
        Recover();
    }
}

public enum ObstacleState {
    idle,
    sprung_waiting,
    sprung_active,
    sprung_returning,
    broken
}
