using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *      Active part component of the boulder obstacle.
 */
public class Obstacle_Boulder_trap : ObstActivePart {
    private Animator animatorSlot;

    // Components from the boulder GameObject
    [SerializeField]
    private GameObject boulder;
    private SkinnedMeshRenderer boulderRenderer;
    private Animator animatorBoulder;

    // Components from the split boulder GameObjects
    [SerializeField]
    private GameObject brokenBoulder;
    private SkinnedMeshRenderer brokenBoulderRenderer;
    private Animator animatorBrokenBoulder;

    // Animation string hashes
    private int openHash;
    private int closeHash;
    private int announceHash;
    private int resetHash;
    private int rollHash;

    public override void Start() {
        base.Start();
        animatorSlot = GetComponent<Animator>();
        animatorBoulder = boulder.GetComponentInParent<Animator>();
        animatorBrokenBoulder = brokenBoulder.GetComponentInParent<Animator>();

        openHash = Animator.StringToHash("open");
        announceHash = Animator.StringToHash("announce");
        resetHash = Animator.StringToHash("reset");
        rollHash = Animator.StringToHash("roll");

        boulderRenderer = boulder.GetComponent<SkinnedMeshRenderer>();
        brokenBoulderRenderer = brokenBoulder.GetComponent<SkinnedMeshRenderer>();

        brokenBoulderRenderer.enabled = false;
    }

    public override void Activate() {
        if (state == ObstacleState.idle || state == ObstacleState.sprung_waiting) {
            base.Activate();
            // First part: open the boulder slot
            animatorSlot.SetTrigger(openHash);
        }
    }

    public override void Anounce() {
        if (state == ObstacleState.idle) {
            base.Anounce();
            animatorSlot.SetTrigger(announceHash);
        }
    }

    public override void Return() {
        base.Return();
    }

    // Animation events

    // This is called as an animation event at the end of the boulder slot
    // opening animation
    public void OnEndOfOpenAnimation() {
        // Second part: roll the boulder down the hallway
        animatorBoulder.SetTrigger(rollHash);
    }

    // This is called as an animation event at the end of the boulder slot
    // closing animation
    public void OnEndOfCloseAnimation() {
        boulderRenderer.enabled = true;
        brokenBoulderRenderer.enabled = false;
        animatorBrokenBoulder.SetTrigger(resetHash);
    }
}
