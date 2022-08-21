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
    private Material healFlashMaterial_1;
    [SerializeField]
    private Material healFlashMaterial_2;
    [SerializeField]
    private Material reduceFlashMaterial_1;
    [SerializeField]
    private Material reduceFlashMaterial_2;
    [SerializeField]
    private Material extendFlashMaterial_1;
    [SerializeField]
    private Material extendFlashMaterial_2;
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
        GameEventSystem.instance.OnHealthAffected += PlayerHealthAffectedReaction;
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

    private void PlayerHealthAffectedReaction(object sender, float amount, bool onMaxHealth) {
        // Debug.Log("Animation Handler: Player was hit! Showing " + amount + " points of damage.");
        if (playerStats.isAlive) { // && !playerStats.isDamageImmune <- there may be some problems here
            // Damage or healing
            if (!onMaxHealth) {
                // Damage
                if (amount < 0) {
                    StartCoroutine(PlayerFlashEffect(hitFlashMaterial_1, hitFlashMaterial_2));
                // Healing
                } else {
                    StartCoroutine(PlayerFlashEffect(healFlashMaterial_1, healFlashMaterial_2));
                }
            // Max health reduction or extension
            } else {
                // Reduction
                if (amount < 0) {
                    StartCoroutine(PlayerFlashEffect(reduceFlashMaterial_1, reduceFlashMaterial_2));
                // Extension
                } else {
                    StartCoroutine(PlayerFlashEffect(extendFlashMaterial_1, extendFlashMaterial_2));
                }
            }
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

    private IEnumerator PlayerFlashEffect(Material mat1, Material mat2) {
        for (int i = 0; i < hitFlashCount; i ++) {
            playerSkMeshRenderer.material = mat1;
            yield return hitFlashTimer;
            playerSkMeshRenderer.material = mat2;
            yield return hitFlashTimer;
        }

        playerSkMeshRenderer.material = originalMaterial;
        if (playerStats.isAlive) {
            playerStats.isDamageImmune = false;
        }
    }

    private void OnDisable() {
        GameEventSystem.instance.OnHealthAffected -= PlayerHealthAffectedReaction;
        GameEventSystem.instance.OnPlayerDeath -= PlayerDeathReaction;
    }
}
