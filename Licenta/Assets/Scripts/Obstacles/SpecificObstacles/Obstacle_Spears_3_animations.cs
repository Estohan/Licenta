using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Spears_3_animations : ObstActivePart {

    /* Open, close and announce animations */

    private Animator animator;
    private int closeHash;
    private int openHash;
    private int announceHash;

    public override void Start() {
        base.Start();
        animator = GetComponent<Animator>();
        openHash = Animator.StringToHash("open");
        closeHash = Animator.StringToHash("close");
        announceHash = Animator.StringToHash("announce");
    }

    public override void Activate() {
        if (state == ObstacleState.sprung_waiting) {
            base.Activate();
            animator.SetTrigger(openHash);
        }
    }

    public override void Return() {
        base.Return();
        animator.SetTrigger(closeHash);
    }

    public override void Anounce() {
        if (state == ObstacleState.idle) {
            base.Anounce();
            animator.SetTrigger(announceHash);
        }
    }
}
