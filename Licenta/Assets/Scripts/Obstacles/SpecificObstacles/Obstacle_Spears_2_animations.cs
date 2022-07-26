using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Spears_2_animations : ObstActivePart {

    /* Open and close animations */

    private Animator animator;
    private int closeHash;
    private int openHash;

    public override void Start() {
        base.Start();
        animator = GetComponent<Animator>();
        openHash = Animator.StringToHash("open");
        closeHash = Animator.StringToHash("close");
    }

    public override void Activate() {
        base.Activate();
        animator.SetTrigger(openHash);
    }

    public override void Return() {
        base.Return();
        animator.SetTrigger(closeHash);
    }
}
