using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNewControlsRB : MonoBehaviour {
    InputManager inputManager;
    PlayerStats playerStats;
    new Rigidbody rigidbody; // ??? "rigidbody hides inherited member Component.rigidbody. Use the new keyword if hiding was intended

    Vector2 currentMovementFromInput;
    Vector3 playerMovement;

    int groundLayer = 7;
    float playerSpeed;

    bool movementInputDetected;
    bool groundedPlayer;
    bool playerJumped;
    bool playerDodged;

    private void Awake() {
        inputManager = new InputManager();
        rigidbody = this.GetComponent<Rigidbody>();
        playerStats = this.GetComponent<PlayerStats>();

        inputManager.PlayerMovement.Move.started += OnMovementInput;
        inputManager.PlayerMovement.Move.canceled += OnMovementInput;
        inputManager.PlayerMovement.Move.performed += OnMovementInput;

        inputManager.PlayerMovement.Run.started += OnSprintInputStarted;
        inputManager.PlayerMovement.Run.canceled += OnSprintInputCanceled;

        inputManager.PlayerMovement.Jump.started += OnJump;
        inputManager.PlayerMovement.Jump.performed += OnJump;

        // inputManager.PlayerMovement.Dodge.started += OnDodge;
        inputManager.PlayerMovement.Dodge.performed += OnDodge;
    }

    private void Start() {
        playerSpeed = playerStats.speedWalking;
        playerJumped = false;
        playerDodged = false;
    }

    private void Update() {
        //groundedPlayer = Physics.CheckSphere(this.transform.position, 0.5f, groundLayer, QueryTriggerInteraction.Ignore);
        if(groundedPlayer) {
            Debug.Log("Grounded");
        } else {
            Debug.Log("Not grounded");
        }

        // Movement
        if (movementInputDetected) {
            transform.forward = playerMovement;
        }

        // Jump
        if (playerJumped) {
            rigidbody.AddForce(Vector3.up * Mathf.Sqrt(playerStats.jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
            playerJumped = false;
        }

        // Dodge
        if (playerDodged) {
            Vector3 dashVelocity = Vector3.Scale(transform.forward, 
                                                 playerStats.dodgeDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * rigidbody.drag + 1)) / -Time.deltaTime),
                                                                                         0,
                                                                                         (Mathf.Log(1f / (Time.deltaTime * rigidbody.drag + 1)) / -Time.deltaTime)));
            rigidbody.AddForce(dashVelocity, ForceMode.VelocityChange);
            playerDodged = false;
        }
    }

    private void FixedUpdate() {
        rigidbody.MovePosition(rigidbody.position + playerMovement * playerSpeed * Time.deltaTime);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(this.transform.position, 0.5f);
    }

    void OnCollisionEnter(Collision collision) {
        // Check collision with the ground layer
        if (!groundedPlayer && collision.gameObject.layer == groundLayer) {
            groundedPlayer = true;
        }
    }
    void OnCollisionExit(Collision collision) {
        // Check collision with the ground layer
        if (groundedPlayer && collision.gameObject.layer == groundLayer) {
            groundedPlayer = false;
        }
    }

    private void OnMovementInput(InputAction.CallbackContext context) {
        currentMovementFromInput = context.ReadValue<Vector2>();
        playerMovement.x = currentMovementFromInput.x;
        playerMovement.z = currentMovementFromInput.y;
        playerMovement = Quaternion.Euler(0f, 45f, 0f) * playerMovement;
        movementInputDetected = playerMovement.x != 0 || playerMovement.z != 0;
    }

    private void OnJump(InputAction.CallbackContext context) {
        if (groundedPlayer) {
            // Debug.Log("-------------------------" + Mathf.Sqrt(playerStats.jumpHeight * -3.0f * gravityValue));
            playerJumped = true;
        }
    }

    private void OnDodge(InputAction.CallbackContext context) {
        if (groundedPlayer) {
            playerDodged = true;
        }
    }

    private void OnSprintInputStarted(InputAction.CallbackContext context) {
        if (playerStats.isIdle) {
            playerSpeed = playerStats.speedRunning;
        }
    }

    private void OnSprintInputCanceled(InputAction.CallbackContext context) {
        if (playerStats.isIdle) {
            playerSpeed = playerStats.speedWalking;
        }
    }

    private void OnEnable() {
        inputManager.PlayerMovement.Enable();
    }

    private void OnDisable() {
        inputManager.PlayerMovement.Disable();
    }
}
