using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNewControlsCC : MonoBehaviour {
    InputManager inputManager;
    CharacterController characterController;
    PlayerStats playerStats;
    PlayerAnimationHandler playerAnimationHandler;

    Vector2 currentMovementFromInput;
    Vector3 playerVelocity;
    Vector3 playerMovement;

    public float gravityValue;
    public Vector3 dragValue;
    int groundLayer = 7;
    //float playerSpeed;

    bool movementInputDetected;
    bool groundedPlayer;
    bool playerJumped;
    bool playerDodged;

    private void Awake() {
        inputManager = new InputManager();
        characterController = this.GetComponent<CharacterController>();
        playerStats = this.GetComponent<PlayerStats>();
        playerAnimationHandler = this.GetComponent<PlayerAnimationHandler>();

        inputManager.PlayerMovement.Move.started += OnMovementInput;
        inputManager.PlayerMovement.Move.canceled += OnMovementInput;
        inputManager.PlayerMovement.Move.performed += OnMovementInput;

        inputManager.PlayerMovement.Run.started += OnSprintInputStarted;
        inputManager.PlayerMovement.Run.canceled += OnSprintInputCanceled;

        //inputManager.PlayerMovement.Jump.started += OnJump;
        inputManager.PlayerMovement.Jump.performed += OnJump;

        // inputManager.PlayerMovement.Dodge.started += OnDodge;
        inputManager.PlayerMovement.Dodge.performed += OnDodge;

        inputManager.PlayerMovement.Sneak.performed += OnSneak;

        inputManager.PlayerMovement.Crawl.performed += OnCrawl;
    }

    private void Start() {
        // playerSpeed = playerStats.speedWalking;
        playerJumped = false;
        playerDodged = false;
    }

    private void Update() {
        groundedPlayer = characterController.isGrounded;
        // groundedPlayer = Physics.CheckSphere(this.transform.position, 0.5f, groundLayer, QueryTriggerInteraction.Ignore);
        // Debug.Log(groundedPlayer + " " + playerVelocity);

        if (groundedPlayer && playerVelocity.y < 0.5f) {
            playerVelocity.y = -0.5f;
        }

        // Movement
        if (movementInputDetected) {
            characterController.Move(playerMovement * playerStats.speed * Time.deltaTime);
            transform.forward = playerMovement;
        }

        // Gravity and jump
        if(playerJumped && groundedPlayer) {
            playerVelocity.y += Mathf.Sqrt(playerStats.jumpHeight * -3.0f * gravityValue);
            playerJumped = false;
        }

        // Dodge
        if(playerDodged && groundedPlayer) {
            playerVelocity += Vector3.Scale(transform.forward,
                                            playerStats.dodgeDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * dragValue.x + 1)) / -Time.deltaTime),
                                                                                    0,
                                                                                    (Mathf.Log(1f / (Time.deltaTime * dragValue.z + 1)) / -Time.deltaTime)));
            playerDodged = false;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);

        playerVelocity.x /= 1 + dragValue.x * Time.deltaTime;
        playerVelocity.y /= 1 + dragValue.y * Time.deltaTime;
        playerVelocity.z /= 1 + dragValue.z * Time.deltaTime;
    }

    /*private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(this.transform.position, 0.5f);
    }*/

    private void OnMovementInput(InputAction.CallbackContext context) {
        currentMovementFromInput = context.ReadValue<Vector2>();
        playerMovement.x = currentMovementFromInput.x;
        playerMovement.z = currentMovementFromInput.y;
        playerMovement = Quaternion.Euler(0f, 45f, 0f) * playerMovement;
        movementInputDetected = playerMovement.x != 0 || playerMovement.z != 0;

        if (context.canceled) { // stopped moving
            playerStats.isIdle = true;
            playerStats.isWalking = false;
        } else { // is moving
            playerStats.isIdle = false;
            playerStats.isWalking = true;
        }
    }

    private void OnJump(InputAction.CallbackContext context) {
        if(groundedPlayer) {
            playerJumped = true;
        }
    }

    private void OnDodge(InputAction.CallbackContext context) {
        //if(groundedPlayer) {
            playerDodged = true;
        //}
    }

    private void OnSprintInputStarted(InputAction.CallbackContext context) {
        if(playerStats.isIdleOnWalk && groundedPlayer) {
            //playerSpeed = playerStats.speedRunning;
            playerStats.speed = playerStats.speedRunning;

            playerStats.isRunning = true;
        }
    }

    private void OnSprintInputCanceled(InputAction.CallbackContext context) {
        if (playerStats.isIdleOnWalk && groundedPlayer) {
            // playerSpeed = playerStats.speedWalking;
            playerStats.speed = playerStats.speedWalking;

            playerStats.isRunning = false;
        }
    }

    private void OnSneak(InputAction.CallbackContext context) {
        if(playerStats.isIdleOnWalk || playerStats.isIdleOnCrawl) {
            playerStats.isIdleOnWalk = false;
            playerStats.isIdleOnCrawl = false;
            playerStats.isIdleOnSneak = true;
        } else {
            playerStats.isIdleOnWalk = true;
            playerStats.isIdleOnSneak = false;
        }
    }

    private void OnCrawl(InputAction.CallbackContext context) {

    }

    private void OnEnable() {
        inputManager.PlayerMovement.Enable();
    }

    private void OnDisable() {
        inputManager.PlayerMovement.Disable();
    }
}
