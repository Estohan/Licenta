using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
    [SerializeField]
    private GameObject player;
    // Offset of the camera to centrate the player in the X axis 
    [SerializeField]
    private float offsetX = -5;
    // Offset of the camera to centrate the player in the Z axis 
    [SerializeField]
    private float offsetZ = 0;
    // Offset of the camera to centrate the player in the Y axis
    [SerializeField]
    private float offsetY = 0;
    // The maximum distance the camera can be away from the player
    [SerializeField]
    private float maximumDistance = 1;


    private PlayerStats playerStats;
    private float playerVelocity;


    private float movementX;
    private float movementZ;
    private float movementY;

    private void Awake() {
        playerStats = player.GetComponent<PlayerStats>();
        // SnapToPlayerPosition();
    }

    void Start() {
        followPlayer();

        if(player.GetComponent<PlayerStats>() != null) {
            playerVelocity = player.GetComponent<PlayerStats>().speed;
        } else {
            playerVelocity = 10;
        }
    }

    private void SnapToPlayerPosition() {
        this.transform.position = new Vector3((player.transform.position.x + offsetX),
                                               (player.transform.position.z + offsetZ),
                                               (player.transform.position.y + offsetY));
    }

    void Update() {
        playerVelocity = playerStats.speed;
        followPlayer();
    }

    void followPlayer() {
        movementX = ((player.transform.position.x + offsetX - this.transform.position.x)) / maximumDistance;
        movementZ = ((player.transform.position.z + offsetZ - this.transform.position.z)) / maximumDistance;
        movementY = ((player.transform.position.y + offsetY - this.transform.position.y)) / maximumDistance;
        this.transform.position += new Vector3((movementX * playerVelocity * Time.deltaTime),
                                               (movementY * playerVelocity * Time.deltaTime),
                                               (movementZ * playerVelocity * Time.deltaTime));
    }
}
