using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject player;
    //The offset of the camera to centrate the player in the X axis 
    public float offsetX = -5;
    //The offset of the camera to centrate the player in the Z axis 
    public float offsetZ = 0;
    //The offset of the camera to centrate the player in the Y axis
    public float offsetY = 0;
    //The maximum distance permited to the camera to be far from the player, used to make a smooth movement 
    public float maximumDistance = 2;

    //The velocity of your player, used to determine the speed of the camera 
    private float playerVelocity;

    private float movementX;
    private float movementZ;
    private float movementY;

    void Start() {
        followPlayer();
        // -------------- What if the player is running?
        if(player.GetComponent<PlayerControls>() != null) {
            playerVelocity = player.GetComponent<PlayerControls>().speed;
        } else {
            playerVelocity = 10;
        }
    }

    // Update is called once per frame 
    void Update() {
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
