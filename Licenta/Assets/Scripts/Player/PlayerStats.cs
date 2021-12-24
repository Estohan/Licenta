using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {
    
    public float speed;
    public float speedRunning;
    public float speedWalking;
    public float speedSneaking;
    public float speedCrawling;
    [Space]
    public float rotationFactor;
    public float runRotationFactor;
    public float walkRotationFactor;
    public float sneakRotationFactor;
    public float crawlRotationFactor;
    [Space]
    public float jumpHeight;
    public float dodgeDistance;
    [Space]
    public bool isIdle;
    public bool isRunning;
    public bool jumped;
    public bool dodged;
    public PlayerControls.PlayerPostureState currentPosture;

    /*private void Awake() {
        animator = new Animator();
    }*/

    private void Start() {
        isIdle = true;
        isRunning = false;
        jumped = false;
        dodged = false;
        currentPosture = PlayerControls.PlayerPostureState.Standing;

        speed = speedWalking;
        rotationFactor = walkRotationFactor;
    }

}
