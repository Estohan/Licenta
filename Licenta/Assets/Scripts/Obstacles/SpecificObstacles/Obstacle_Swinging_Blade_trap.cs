using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *      Active part component of the swinging blade obstacle.
 */
public class Obstacle_Swinging_Blade_trap : ObstActivePart {

    [Space]
    [SerializeField]
    private bool selfStart;
    [SerializeField]
    private float startDelay;

    private Animator bladeAnimator;

    private int swingHash;

    public override void Start() {
        bladeAnimator = this.GetComponent<Animator>();

        swingHash = Animator.StringToHash("swing");

        if(selfStart) {
            StartCoroutine(DelayedActivation(startDelay));
        }
    }

    public override void Trigger() {
        if(state == ObstacleState.idle) {
            StartCoroutine(DelayedActivation(startDelay));
        }
    }

    private IEnumerator DelayedActivation(float startDelay) {
        yield return new WaitForSeconds(startDelay);
        bladeAnimator.SetBool(swingHash, true);
        base.Activate();
    }
}
