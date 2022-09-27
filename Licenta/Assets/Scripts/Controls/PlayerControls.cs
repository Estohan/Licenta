using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 *      This controls all the character's actions and posture changes triggered
 *  by player input.
 *      CharacterController is used for movement and all the physics simulation
 *  operations associated with the character's movement can be found in the
 *  Update() function.
 */
public class PlayerControls : MonoBehaviour {
    [SerializeField]
    private Camera mainCamera;
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
        characterController = this.GetComponent<CharacterController>();
        playerStats = this.GetComponent<PlayerStats>();
        playerAnimationHandler = this.GetComponent<PlayerAnimationHandler>();

        waitDodgeCooldown = new WaitForSeconds(dodgeCooldown);
    }

    private void Start() {
        playerJumped = false;
        playerDodged = false;
        dodgeOnCooldown = false;

        GameManager.inputManager.PlayerMovement.Run.started += OnSprintInputStarted;
        GameManager.inputManager.PlayerMovement.Run.canceled += OnSprintInputCanceled;

        GameManager.inputManager.PlayerMovement.Jump.performed += OnJump;

        GameManager.inputManager.PlayerMovement.Dodge.performed += OnDodge;

        GameManager.inputManager.PlayerMovement.Sneak.performed += OnSneak;

        GameManager.inputManager.PlayerMovement.Crawl.performed += OnCrawl;

        // WASD used to be the main form of movement, that is why this is called "Alternatemove"
        GameManager.inputManager.PlayerMovement.Alternatemove.started += OnAltMovementInput;
        GameManager.inputManager.PlayerMovement.Alternatemove.canceled += OnAltMovementInput;
        GameManager.inputManager.PlayerMovement.Alternatemove.performed += OnAltMovementInput;
    }

    private void Update() {
        groundedPlayer = characterController.isGrounded;

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
        if (playerStats.currentPosture != PlayerPostureState.Standing) {
            ChangeStatsByPosture(PlayerPostureState.Standing);
            playerAnimationHandler.ChangePostureAnimation(PlayerPostureState.Standing);
        } else {
            playerStats.speed = playerStats.speedWalking;
        }

        playerAnimationHandler.Run(false);
        playerStats.isRunning = false;
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

    private IEnumerator DodgeCooldownCoroutine() {
        yield return waitDodgeCooldown;
        dodgeOnCooldown = false;
    }

    public enum PlayerPostureState {
        Standing,
        Sneaking,
        Crawling
    }
}
