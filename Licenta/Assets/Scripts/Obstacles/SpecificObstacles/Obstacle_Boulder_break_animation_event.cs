using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Boulder_break_animation_event : MonoBehaviour {
    [SerializeField]
    private GameObject boulderSlot;
    private Animator boulderSlotAnimator;

    private int closeHash;

    private void Start() {
        boulderSlotAnimator = boulderSlot.GetComponent<Animator>();
        closeHash = Animator.StringToHash("close");
    }

    public void OnEndOfBreakAnimation() {
        // Fourth part: close the boulder slot back up
        boulderSlotAnimator.SetTrigger(closeHash);
    }
}
