using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {
    
    public float speed;
    public float speedWalking;
    public float speedRunning;
    public float speedSneaking;
    public float speedCrawling;
    public float jumpHeight;
    public float dodgeDistance;

    public bool isIdle;

    public bool isIdleOnWalk;
    public bool isIdleOnSneak;
    public bool isIdleOnCrawl;

    public bool isWalking;
    public bool isSneaking;
    public bool isCrawling;
    public bool isRunning;

    /*private void Awake() {
        animator = new Animator();
    }*/

    private void Start() {
        isIdle = true;

        isIdleOnWalk = true;
        isIdleOnSneak = false;
        isIdleOnCrawl = false;

        isWalking = false;
        isSneaking = false;
        isCrawling = false;
        isRunning = false;
        speed = speedWalking;
    }
}
