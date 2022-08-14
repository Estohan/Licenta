using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour {
    [SerializeField]
    private Camera mainCamera;
    private InputManager inputManager;
    private CharacterController characterController;
    private PlayerStats playerStats;
    private PlayerAnimationHandler playerAnimationHandler;

    private Vector3 currentMovementFromInput; // used to be Vec2 while using wasd
    private Vector3 playerVelocity;
    private Vector3 playerMovement;

    [SerializeField]
    private float gravityValue;
    [SerializeField]
    private Vector3 dragValue;
    [SerializeField]
    private float dodgeCooldown;

    private Quaternion currentRotation;
    private Quaternion targetRotation;

    private bool movementInputDetected;
    private bool groundedPlayer;
    private bool playerJumped;
    private bool playerDodged;
    private bool dodgeOnCooldown;
    private float mov_planePoint;
    private Plane mov_plane;
    private Ray mov_ray;

    private WaitForSeconds waitDodgeCooldown;

    private void Awake() {
        inputManager = new InputManager();
        characterController = this.GetComponent<CharacterController>();
        playerStats = this.GetComponent<PlayerStats>();
        playerAnimationHandler = this.GetComponent<PlayerAnimationHandler>();

/*        inputManager.PlayerMovement.Move.started += OnMovementInput;
        inputManager.PlayerMovement.Move.canceled += OnMovementInput;
        inputManager.PlayerMovement.Move.performed += OnMovementInput;*/

        inputManager.PlayerMovement.Run.started += OnSprintInputStarted;
        inputManager.PlayerMovement.Run.canceled += OnSprintInputCanceled;

        //inputManager.PlayerMovement.Jump.started += OnJump;
        inputManager.PlayerMovement.Jump.performed += OnJump;

        // inputManager.PlayerMovement.Dodge.started += OnDodge;
        inputManager.PlayerMovement.Dodge.performed += OnDodge;

        inputManager.PlayerMovement.Sneak.performed += OnSneak;

        inputManager.PlayerMovement.Crawl.performed += OnCrawl;

        // Alternate movement
        inputManager.PlayerMovement.Alternatemove.started += OnAltMovementInput;
        inputManager.PlayerMovement.Alternatemove.canceled += OnAltMovementInput;
        inputManager.PlayerMovement.Alternatemove.performed += OnAltMovementInput;

        waitDodgeCooldown = new WaitForSeconds(dodgeCooldown);
    }

    private void Start() {
        // playerSpeed = playerStats.speedWalking;
        playerJumped = false;
        playerDodged = false;
        dodgeOnCooldown = false;
    }

    private void Update() {
        //Debug.Log("Start " + this.transform.position);
        groundedPlayer = characterController.isGrounded;
        // groundedPlayer = Physics.CheckSphere(this.transform.position, 0.5f, groundLayer, QueryTriggerInteraction.Ignore);
        // Debug.Log(groundedPlayer + " " + playerVelocity);

        if (groundedPlayer && playerVelocity.y < 0.5f) {
            playerVelocity.y = -0.5f;
        }

        // Movement
        if (movementInputDetected) {
            mov_plane = new Plane(Vector3.up, transform.position);
            // Create a ray from the mouse click position
            mov_ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            // Initialization of the enter value
            mov_planePoint = 0f;

            if (mov_plane.Raycast(mov_ray, out mov_planePoint)) {
                // Get the point that is clicked
                currentMovementFromInput = mov_ray.GetPoint(mov_planePoint);
            }

            // [ TODO ]
            // check currentMovementFromInput - this.transform.position for clicking too close
            // to the player

            playerMovement = (currentMovementFromInput - this.transform.position).normalized;
            
            characterController.Move(playerMovement * playerStats.speed * Time.deltaTime);

            // character rotation
            currentRotation = transform.rotation;
            targetRotation = Quaternion.LookRotation(playerMovement);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, playerStats.rotationFactor);
        }

        // Gravity and jump
        if (playerJumped && groundedPlayer) {
            playerVelocity.y += Mathf.Sqrt(playerStats.jumpHeight * -3.0f * gravityValue);
            playerJumped = false;
        }

        // Dodge
        if (playerDodged && groundedPlayer) {
            playerVelocity += Vector3.Scale(transform.forward,
                                            playerStats.dodgeDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * dragValue.x + 1)) / -Time.deltaTime),
                                                                                    0,
                                                                                    (Mathf.Log(1f / (Time.deltaTime * dragValue.z + 1)) / -Time.deltaTime)));
            playerDodged = false;
            StartCoroutine(DodgeCooldownCoroutine());
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);

        playerVelocity.x /= 1 + dragValue.x * Time.deltaTime;
        playerVelocity.y /= 1 + dragValue.y * Time.deltaTime;
        playerVelocity.z /= 1 + dragValue.z * Time.deltaTime;
        //Debug.Log(this.transform.position + "end");
    }

    /*private void Update() {
        groundedPlayer = characterController.isGrounded;
        // groundedPlayer = Physics.CheckSphere(this.transform.position, 0.5f, groundLayer, QueryTriggerInteraction.Ignore);
        // Debug.Log(groundedPlayer + " " + playerVelocity);

        if (groundedPlayer && playerVelocity.y < 0.5f) {
            playerVelocity.y = -0.5f;
        }

        // Movement
        if (movementInputDetected) {
            Debug.Log("Movement vector: " + playerMovement);
            characterController.Move(playerMovement * playerStats.speed * Time.deltaTime);
            // character rotation?
            currentRotation = transform.rotation;
            targetRotation = Quaternion.LookRotation(playerMovement);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, playerStats.rotationFactor);
            // transform.forward = playerMovement;
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
    }*/

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
        } else { // is moving
            playerStats.isIdle = false;
        }
        playerAnimationHandler.Move(playerStats.isIdle);

        // Debug.Log("Keyboard: " + playerMovement);
    }

    private void OnAltMovementInput(InputAction.CallbackContext context) {

        if (context.canceled) { // stopped moving
            playerStats.isIdle = true;
            movementInputDetected = false;
        } else { // is moving
            playerStats.isIdle = false;
            movementInputDetected = true;
        }
        playerAnimationHandler.Move(playerStats.isIdle);
    }


    private void OnJump(InputAction.CallbackContext context) {
        if(groundedPlayer &&
            playerStats.currentPosture != PlayerPostureState.Crawling) {
            playerAnimationHandler.Jump();
            playerJumped = true;
        }
    }

    private void OnDodge(InputAction.CallbackContext context) {
        if(groundedPlayer &&
            !dodgeOnCooldown &&
            playerStats.currentPosture != PlayerPostureState.Crawling) {
            playerAnimationHandler.Dodge();
            playerDodged = true;
            dodgeOnCooldown = true;
        }
    }

    private void OnSprintInputStarted(InputAction.CallbackContext context) {
        if(!playerStats.isIdle && groundedPlayer) {
            ChangeStatsByPosture(PlayerPostureState.Standing);
            playerAnimationHandler.ChangePostureAnimation(PlayerPostureState.Standing);

            playerStats.speed = playerStats.speedRunning;
            playerAnimationHandler.Run(true);
            playerStats.isRunning = true;
        }
    }

    private void OnSprintInputCanceled(InputAction.CallbackContext context) {
        //if (!playerStats.isIdle && groundedPlayer) {
            if (playerStats.currentPosture != PlayerPostureState.Standing) {
                ChangeStatsByPosture(PlayerPostureState.Standing);
                playerAnimationHandler.ChangePostureAnimation(PlayerPostureState.Standing);
            } else {
                playerStats.speed = playerStats.speedWalking;
            }

            playerAnimationHandler.Run(false);
            playerStats.isRunning = false;
        //}
    }

    private void OnSneak(InputAction.CallbackContext context) {
        if(playerStats.isIdle) {
            if(playerStats.currentPosture != PlayerPostureState.Sneaking) {
                ChangeStatsByPosture(PlayerPostureState.Sneaking);
                playerAnimationHandler.ChangePostureAnimation(PlayerPostureState.Sneaking);
            } else {
                ChangeStatsByPosture(PlayerPostureState.Standing);
                playerAnimationHandler.ChangePostureAnimation(PlayerPostureState.Standing);
            }
        }
    }

    private void OnCrawl(InputAction.CallbackContext context) {
        if (playerStats.isIdle) {
            if (playerStats.currentPosture != PlayerPostureState.Crawling) {
                ChangeStatsByPosture(PlayerPostureState.Crawling);
                playerAnimationHandler.ChangePostureAnimation(PlayerPostureState.Crawling);
            } else {
                ChangeStatsByPosture(PlayerPostureState.Standing);
                playerAnimationHandler.ChangePostureAnimation(PlayerPostureState.Standing);
            }
        }
    }

    public void ChangeStatsByPosture(PlayerPostureState newPosture) {
        playerStats.currentPosture = newPosture;

        if(newPosture == PlayerPostureState.Standing) {
            playerStats.speed = playerStats.speedWalking;
            playerStats.rotationFactor = playerStats.walkRotationFactor;
        } else if(newPosture == PlayerPostureState.Sneaking) {
            playerStats.speed = playerStats.speedSneaking;
            playerStats.rotationFactor = playerStats.sneakRotationFactor;
        } else if (newPosture == PlayerPostureState.Crawling) {
            playerStats.speed = playerStats.speedCrawling;
            playerStats.rotationFactor = playerStats.crawlRotationFactor;
        }

        // Change capsule collider dimensions based on posture
        int index = (int)newPosture;
        characterController.center = playerStats.CapsuleColliders[index].center;
        characterController.radius = playerStats.CapsuleColliders[index].radius;
        characterController.height = playerStats.CapsuleColliders[index].height;
    }

    private void OnEnable() {
        inputManager.PlayerMovement.Enable();
    }

    private void OnDisable() {
        inputManager.PlayerMovement.Disable();
    }

    private IEnumerator DodgeCooldownCoroutine() {
        Debug.Log("Dodge cooldown up.");
        yield return waitDodgeCooldown;
        dodgeOnCooldown = false;
        Debug.Log("Dodge cooldown down.");
    }

    public enum PlayerPostureState {
        Standing,
        Sneaking,
        Crawling
    }
    /*private void OnAltMovementInput(InputAction.CallbackContext context) {
        Vector3 targetPos = Vector3.zero;
        Plane plane = new Plane(Vector3.up, transform.position);
        // Create a ray from the mouse click position
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        // Initialization of the enter value
        float point = 0f;


        if (plane.Raycast(ray, out point)) {
            // Get the point that is clicked
            targetPos = ray.GetPoint(point);
        }

        // Move towards the targetPos by speed*Time.deltaTime
        // transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        //Debug.Log("TargetPos: " + targetPos);
        //Debug.Log("Result?: " + (targetPos - this.transform.position).normalized);
        //Debug.DrawLine(transform.position, targetPos, Color.red);

        currentMovementFromInput = (targetPos - this.transform.position).normalized;
        playerMovement.x = currentMovementFromInput.x;
        playerMovement.z = currentMovementFromInput.y;
        playerMovement = Quaternion.Euler(0f, 45f, 0f) * playerMovement;
        movementInputDetected = playerMovement.x != 0 || playerMovement.z != 0;

        if (context.canceled) { // stopped moving
            playerStats.isIdle = true;
        } else { // is moving
            playerStats.isIdle = false;
        }
        playerAnimationHandler.Move(playerStats.isIdle);

        *//*Plane plane = new Plane(Vector3.up, transform.position);
        // Create a ray from the mouse click position
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        // Initialization of the enter value
        float point = 0f;


        if (plane.Raycast(ray, out point)) {
            // Get the point that is clicked
            currentMovementFromInput = ray.GetPoint(point);
        }

        Vector3 movement = new Vector3();
        //currentMovementFromInput = mainCamera.ScreenToViewportPoint(Mouse.current.position.ReadValue());
        *//*currentMovementFromInput = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        movement.x = currentMovementFromInput.x;
        movement.z = currentMovementFromInput.y;
        playerMovement = (this.transform.position - movement).normalized;
        movementInputDetected = playerMovement.x != 0 || playerMovement.z != 0;

        if (context.canceled) { // stopped moving
            movement = Vector3.zero;
            playerStats.isIdle = true;
        } else { // is moving
            playerStats.isIdle = false;
        }
        playerAnimationHandler.Move(playerStats.isIdle);*//*

        // Destination too close to player
        *//*if (transform.position == targetPos)
            isWalking = false;*//*

        Debug.Log("Mouse: " + currentMovementFromInput);
        Debug.DrawLine(this.transform.position, currentMovementFromInput, Color.red);*//*
    }*/
}
