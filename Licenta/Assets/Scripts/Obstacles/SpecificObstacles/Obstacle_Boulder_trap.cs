using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public override void Start() {
        base.Start();
        animatorSlot = GetComponent<Animator>();
        animatorBoulder = boulder.GetComponentInParent<Animator>();
        animatorBrokenBoulder = brokenBoulder.GetComponentInParent<Animator>();

        openHash = Animator.StringToHash("open");
        announceHash = Animator.StringToHash("announce");
        resetHash = Animator.StringToHash("reset");

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
            boulderRenderer.enabled = true;
            animatorSlot.SetTrigger(announceHash);
        }
    }

    public override void Return() {
        base.Return();
        animatorBrokenBoulder.SetTrigger(resetHash);
    }
}
