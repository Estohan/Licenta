using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float speed;
    public float speedWalking;
    public float speedRunning;
    public Vector3 targetPos;
    public bool isMoving;
    public bool isRunning;
    const int mouseLMB = 0;

    // Use this for initialization1
    void Start() {
        targetPos = transform.position;
        isMoving = false;
        isRunning = false;
        speedWalking = 3.0f;
        speedRunning = 6.0f;
        speed = speedWalking;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButton(mouseLMB)) {
            SetTarggetPosition();
            // Running
            if (Input.GetKey(KeyCode.LeftShift) && isMoving) {
                isRunning = true;
                speed = speedRunning;
            // Walking
            } else {
                isRunning = false;
                speed = speedWalking;
            }

            if (isMoving) {
                MoveObject();
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

        isMoving = true;
    }

    void MoveObject() {
        // Rotate the transform so that the forward vector points to targetPos
        transform.LookAt(targetPos);
        // Move towards the targetPos by speed*Time.deltaTime
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (transform.position == targetPos)
            isMoving = false;
        Debug.DrawLine(transform.position, targetPos, Color.red);
    }
}
