using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlsOld : MonoBehaviour
{
    public float speed;
    public float speedWalking;
    public float speedRunning;

    public Vector3 targetPos;

    public bool isIdle;
    public bool isWalking;
    public bool isRunning;

    const int mouseLMB = 0;

    Animator animator;

    // Use this for initialization1
    void Start() {
        animator = GetComponent<Animator>();

        targetPos = transform.position;
        isWalking = false;
        isRunning = false;
        speed = speedWalking;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButton(mouseLMB)) {
            SetTarggetPosition();
            // Running
            if (Input.GetKey(KeyCode.LeftShift) && isWalking) {
                isRunning = true;
                speed = speedRunning;
            // Walking
            } else {
                isRunning = false;
                speed = speedWalking;
            }

            if (isWalking) {
                Move();
                animator.SetBool("isWalking", isWalking);
            }
        }
    }

    void SetTarggetPosition() {
        Plane plane = new Plane(Vector3.up, transform.position);
        // Create a ray from the mouse click position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Initialization of the enter value
        float point = 0f;


        if (plane.Raycast(ray, out point)) {
            // Get the point that is clicked
            targetPos = ray.GetPoint(point);
        }

        isWalking = true;
    }

    void Move() {
        // Rotate the transform so that the forward vector points to targetPos
        transform.LookAt(targetPos);
        // Move towards the targetPos by speed*Time.deltaTime
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (transform.position == targetPos)
            isWalking = false;
        Debug.DrawLine(transform.position, targetPos, Color.red);
    }
}
