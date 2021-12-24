using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private Animator animator;
    private PlayerStats playerStats;

    // animator parameter hashing variables
    // private int isWalkingHash;
    private int isIdleHash;
    private int isRunningHash;
    private int jumpedHash;
    private int dodgedHash;
    private int postureStateHash;

    private void Awake() {
        animator = this.GetComponent<Animator>();
        playerStats = this.GetComponent<PlayerStats>();

        // isWalkingHash = Animator.StringToHash("isWalking");
        isIdleHash = Animator.StringToHash("isIdle");
        isRunningHash = Animator.StringToHash("isRunning");
        jumpedHash = Animator.StringToHash("jump");
        dodgedHash = Animator.StringToHash("dodge");
        postureStateHash = Animator.StringToHash("postureState");
    }

    private void Start() {
        //playerStats.isWalking = animator.GetBool(isWalkingHash);
        playerStats.isIdle = animator.GetBool(isIdleHash);
        playerStats.isRunning = animator.GetBool(isRunningHash);
        playerStats.currentPosture = (PlayerControls.PlayerPostureState)animator.GetInteger(postureStateHash);
    }

    private void Update() {

        // Idle player
        /*if(playerStats.isIdle) {
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
        }*/
    }

    public void ChangePostureAnimation(PlayerControls.PlayerPostureState newPosture) {
        animator.SetInteger(postureStateHash, (int)newPosture);
    }

    public void Run(bool isPlayerRunning) {
        animator.SetBool(isRunningHash, isPlayerRunning);
    }

    public void Move(bool isPlayerMoving) {
        animator.SetBool(isIdleHash, isPlayerMoving);
    }

    public void Jump() {
        animator.SetTrigger(jumpedHash);
    }

    public void Dodge() {
        animator.SetTrigger(dodgedHash);
    }
}
