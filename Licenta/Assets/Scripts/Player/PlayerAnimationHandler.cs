using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private Animator animator;
    private PlayerStats playerStats;

    // animator parameter hashing variables
    private int isWalkingHash;
    private int isRunningHash;

    private void Awake() {
        animator = this.GetComponent<Animator>();
        playerStats = this.GetComponent<PlayerStats>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
    }

    private void Start() {
        playerStats.isWalking = animator.GetBool(isWalkingHash);
        playerStats.isRunning = animator.GetBool(isRunningHash);
    }

    private void Update() {
        // Idle player
        if(playerStats.isIdle) {
            if(playerStats.isIdleOnWalk) {
                animator.SetBool(isWalkingHash, false);
                animator.SetBool(isRunningHash, false);
            }
        } else { // Moving player
            if (playerStats.isWalking) {
                animator.SetBool(isWalkingHash, true);
            }
            if(playerStats.isRunning) {
                animator.SetBool(isRunningHash, true);
            }
        }
    }
}
