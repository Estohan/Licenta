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
    public ColliderDims[] CapsuleColliders;

    /*private void Awake() {
        animator = new Animator();
    }*/

    private void Start() {
        isIdle = true;
        isRunning = false;
        jumped = false;
        dodged = false;
        CapsuleColliders = new ColliderDims[3];
        CapsuleColliders[0].SetDimensions(new Vector3(0f, 0.88f, 0f), 0.194f, 1.75f);
        CapsuleColliders[1].SetDimensions(new Vector3(0f, 0.71f, 0f), 0.194f, 1.48f);
        CapsuleColliders[2].SetDimensions(new Vector3(0f, 0.24f, 0f), 0.260f, 0.45f);

        currentPosture = PlayerControls.PlayerPostureState.Standing;

        speed = speedWalking;
        rotationFactor = walkRotationFactor;
    }

    public struct ColliderDims {
        public Vector3 center;
        public float radius;
        public float height;

        public void SetDimensions(Vector3 center, float radius, float height) {
            this.center = center;
            this.radius = radius;
            this.height = height;
        }
    }

}
