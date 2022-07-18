using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_bear_trap : ObstActivePart {
    private Animator animator;
    private int openHash;

    [Header("Obstacle specific fields")]
    public ObstHarmfulPart obstHarmfulPart;

    public override void Start() {
        base.Start();
        animator = GetComponent<Animator>();
        openHash = Animator.StringToHash("open");
    }

    // Anounce: not implemented

    public override void Activate() {
        base.Activate();
        animator.SetTrigger(openHash);

        if (obstHarmfulPart != null) {
            obstHarmfulPart.ForcedActivation();
        }
    }

    public override void Return() {
        base.Return();
    }
}
