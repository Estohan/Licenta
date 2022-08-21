using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MyMonoBehaviour {
    public float maxHealth;
    public float currHealth; // persistent stats should be stored in a SO?
    [Space]
    public bool isAlive;
    public bool isInactive;
    public bool isDamageImmune;
    [Space]
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
    public PlayerControls.PlayerPostureState currentPosture; // persistent?
    public ColliderDims[] CapsuleColliders;

    public MazeCellData currentCellData;

    /*private void Awake() {
        animator = new Animator();
    }*/

    // Damage test
    public bool test1_damage;
    public bool test2_heal;
    public bool test3_reduce;
    public bool test4_extend;

    private void Update() {
        int val = 0;
        if (test1_damage) {
            test1_damage = false;
            val = UnityEngine.Random.Range(5, 15);
            GameEventSystem.instance.PlayerHealthAffected(- val, false);
            GameEventSystem.instance.PlayerStatsChanged();
        }
        if (test2_heal) {
            test2_heal = false;
            val = UnityEngine.Random.Range(5, 15);
            GameEventSystem.instance.PlayerHealthAffected(val, false);
            GameEventSystem.instance.PlayerStatsChanged();
        }
        if (test3_reduce) {
            test3_reduce = false;
            val = UnityEngine.Random.Range(5, 15);
            GameEventSystem.instance.PlayerHealthAffected(- val, true);
            GameEventSystem.instance.PlayerStatsChanged();
        }
        if (test4_extend) {
            test4_extend = false;
            val = UnityEngine.Random.Range(5, 15);
            GameEventSystem.instance.PlayerHealthAffected(val, true);
            GameEventSystem.instance.PlayerStatsChanged();
        }
    }

    protected override void Start() {
        base.Start();
        currHealth = maxHealth;
        isAlive = true;
        isInactive = false;
        isDamageImmune = false;
        // controls
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

    protected override void OnEnable() {
        base.OnEnable();
    }

    protected override void SafeOnEnable() {
        // events
        GameEventSystem.instance.OnHealthAffected += PlayerHealthAffectedReaction;
        GameEventSystem.instance.OnPlayerDeath += PlayerDeathReaction;
        //GameEventSystem.instance.OnPlayerMoveToAnotherCell += MoveToAnotherCellReaction;
    }

    /*private void MoveToAnotherCellReaction(object sender, MazeCellData cellData) {
        if(currentCellData != cellData) {
            currentCellData = cellData;
            GameEventSystem.instance.PlayerStatsChanged();
        }
    }*/

    private void PlayerDeathReaction(object sender) {
        currHealth = 0;
        isAlive = false;
        isInactive = true;
    }

    private void PlayerHealthAffectedReaction(object sender, float amount, bool onMaxHealth) {
        // Debug.Log("PlayerStats: Player was hit for " + damage + " points of damage!");
        // Damage or healing
        if (!onMaxHealth) {
            // Damage
            if (amount < 0) {
                if (!isDamageImmune) {
                    // Damage immunity is reset in PlayerAnimationHandler, at the end of the
                    // hit effect coroutine
                    isDamageImmune = true;
                    // Check if this hit kills the player
                    if (currHealth + amount <= 0) {
                        currHealth = 0;
                        GameEventSystem.instance.PlayerDeath();
                    } else {
                        currHealth += amount;
                    }
                    GameEventSystem.instance.PlayerStatsChanged();
                }
            // Healing
            } else {
                // Prevent overheal
                if (currHealth + amount > maxHealth) {
                    currHealth = maxHealth;
                } else {
                    currHealth += amount;
                }
                GameEventSystem.instance.PlayerStatsChanged();
            }
        // Max health reduction or extension
        } else {
            // Reduction
            if (amount < 0) {
                // Check if maximum health reaches 0 and kills the player
                if (maxHealth + amount <= 0) {
                    maxHealth = 0;
                    GameEventSystem.instance.PlayerDeath();
                } else {
                    maxHealth += amount;
                }
                GameEventSystem.instance.PlayerStatsChanged();
            // Extension
            } else {
                maxHealth += amount;
                // Player is also healed
                currHealth += amount;
                GameEventSystem.instance.PlayerStatsChanged();
            }
        }
    }


    // Dimensions of the character controller's capsule collider
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

    private void OnDisable() {
        GameEventSystem.instance.OnHealthAffected -= PlayerHealthAffectedReaction;
        GameEventSystem.instance.OnPlayerDeath -= PlayerDeathReaction;
    }
}
