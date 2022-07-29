using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Boulder_roll_animation_event : MonoBehaviour {
    [SerializeField]
    private GameObject brokenBoulder;
    private Animator animatorBrokenBoulder;

    [SerializeField]
    private GameObject boulderSlot;
    private Animator animatorBoulderSlot;

    [SerializeField]
    private SkinnedMeshRenderer brokenBoulderRenderer;
    [SerializeField]
    private SkinnedMeshRenderer boulderRenderer;

    private int breakHash;
    private int closeHash;

    private void Start() {
        animatorBrokenBoulder = brokenBoulder.GetComponent<Animator>();
        animatorBoulderSlot = boulderSlot.GetComponent<Animator>();
        breakHash = Animator.StringToHash("break");
        closeHash = Animator.StringToHash("close");
    }

    // This is called as an animation event at the end of the rolling
    // animation on the boulder
    public void OnEndOfRollAnimation() {
        // Third part: break the boulder
        boulderRenderer.enabled = false;
        brokenBoulderRenderer.enabled = true;
        animatorBrokenBoulder.SetTrigger(breakHash);
        animatorBoulderSlot.SetTrigger(closeHash);
    }
}
