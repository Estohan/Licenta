using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Push_trap_2_animations : ObstActivePart {
    /* Open and close animations */

    private Animator animator;
    private int extendHash;
    private int returnHash;

    public override void Start() {
        base.Start();
        animator = GetComponent<Animator>();
        extendHash = Animator.StringToHash("extend");
        returnHash = Animator.StringToHash("return");
    }

    public override void Activate() {
        base.Activate();
        animator.SetTrigger(extendHash);
    }

    public override void Return() {
        base.Return();
        animator.SetTrigger(returnHash);
    }
}
