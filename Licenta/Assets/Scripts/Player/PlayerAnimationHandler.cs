using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MyMonoBehaviour
{
    [SerializeField]
    private int hitFlashCount;
    [SerializeField]
    private float hitFlashDuration;
    [SerializeField]
    private Material hitFlashMaterial_1;
    [SerializeField]
    private Material hitFlashMaterial_2;
    [SerializeField]
    private SkinnedMeshRenderer playerSkMeshRenderer;

    [Space]
    [Header("Special animations animator indexes:")]
    [SerializeField]
    private int cheerAnimationIndex;
    [SerializeField]
    private int standUpAnimationIndex;
    // starts from 0 - special case for dying while crawling
    [SerializeField]
    private int deathAnimationsCount;


    private Material originalMaterial;

    private Animator animator;
    private PlayerStats playerStats;

    // animator parameter hashing variables
    // private int isWalkingHash;
    private int isIdleHash;
    private int isRunningHash;
    private int jumpedHash;
    private int dodgedHash;
    private int postureStateHash;
    private int expireHash;
    private int deathAnimationHash;
    private int playSpecialHash;
    private int specialAnimationHash;

    private WaitForSeconds hitFlashTimer;

    private void Awake() {
        animator = this.GetComponent<Animator>();
        playerStats = this.GetComponent<PlayerStats>();

        // isWalkingHash = Animator.StringToHash("isWalking");
        isIdleHash = Animator.StringToHash("isIdle");
        isRunningHash = Animator.StringToHash("isRunning");
        jumpedHash = Animator.StringToHash("jump");
        dodgedHash = Animator.StringToHash("dodge");
        postureStateHash = Animator.StringToHash("postureState");
        expireHash = Animator.StringToHash("expire");
        deathAnimationHash = Animator.StringToHash("deathAnimation");
        playSpecialHash = Animator.StringToHash("playSpecial");
        specialAnimationHash = Animator.StringToHash("specialAnimation");
    }

    protected override void Start() {
        base.Start();
        //playerStats.isWalking = animator.GetBool(isWalkingHash);
        playerStats.isIdle = animator.GetBool(isIdleHash);
        playerStats.isRunning = animator.GetBool(isRunningHash);
        playerStats.currentPosture = (PlayerControls.PlayerPostureState)animator.GetInteger(postureStateHash);

        hitFlashTimer = new WaitForSeconds(hitFlashDuration);
        originalMaterial = playerSkMeshRenderer.sharedMaterial;

        // Default values of flashing effect variables
        if (hitFlashCount == 0) {
            hitFlashCount = 3;
        }

        if (hitFlashDuration == 0f) {
            hitFlashDuration = 0.2f;
        }
    }

    protected override void OnEnable() {
        base.OnEnable();
    }

    protected override void SafeOnEnable() {
        // events
        GameEventSystem.instance.OnPlayerHit += HitPlayerReaction;
        GameEventSystem.instance.OnPlayerDeath += PlayerDeathReaction;
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

    public void Expire() {
        if (playerStats.currentPosture == PlayerControls.PlayerPostureState.Crawling) {
            // 0 - first death animations is special for crawling posture state
            animator.SetInteger(deathAnimationHash, 0);
        } else {
            // random death animation from 1 - deathAnimationsCount
            int randAnimation = UnityEngine.Random.Range(1, deathAnimationsCount);
            animator.SetInteger(deathAnimationHash, randAnimation);
        }

        animator.SetTrigger(expireHash);
    }

    public void StandUp() {
        animator.SetInteger(specialAnimationHash, standUpAnimationIndex);
        animator.SetTrigger(playSpecialHash);
    }

    public void Cheer() {
        animator.SetInteger(specialAnimationHash, cheerAnimationIndex);
        animator.SetTrigger(playSpecialHash);
    }

    private void HitPlayerReaction(object sender, float damage) {
        // Debug.Log("Animation Handler: Player was hit! Showing " + damage + " points of damage.");
        if (playerStats.isAlive && !playerStats.isDamageImmune) {
            StartCoroutine(PlayerHitFlashEffect());
        }

    }

    private void PlayerDeathReaction(object sender) {
        Expire();
    }
    public void AnimationPause() {
        animator.speed = 0f;
    }

    public void AnimationResume() {
        animator.speed = 1f;
    }

    private IEnumerator PlayerHitFlashEffect() {
        for (int i = 0; i < hitFlashCount; i ++) {
            playerSkMeshRenderer.material = hitFlashMaterial_1;
            yield return hitFlashTimer;
            playerSkMeshRenderer.material = hitFlashMaterial_2;
            yield return hitFlashTimer;
        }
        playerSkMeshRenderer.material = originalMaterial;
        if (playerStats.isAlive) {
            playerStats.isDamageImmune = false;
        }
    }

    private void OnDisable() {
        GameEventSystem.instance.OnPlayerHit -= HitPlayerReaction;
        GameEventSystem.instance.OnPlayerDeath -= PlayerDeathReaction;
    }
}
