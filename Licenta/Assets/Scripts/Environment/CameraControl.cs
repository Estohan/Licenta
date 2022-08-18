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
    [SerializeField]
    private float orthographicSize;


    [Space]
    [Header("Zoom out effect:")]
    [SerializeField]
    private float zoomSpeed;
    [SerializeField]
    private float zoomStartSize;
    [SerializeField]
    private float secondsBeforeZoom;
    private WaitForSeconds waitBeforeZoom;


    private PlayerStats playerStats;
    private float playerVelocity;
    private Camera mainCamera;


    private float movementX;
    private float movementZ;
    private float movementY;

    private void Awake() {
        playerStats = player.GetComponent<PlayerStats>();
        mainCamera = this.GetComponent<Camera>();
    }

    void Start() {
        waitBeforeZoom = new WaitForSeconds(secondsBeforeZoom);
        mainCamera.orthographicSize = orthographicSize;

        followPlayer();

        if(player.GetComponent<PlayerStats>() != null) {
            playerVelocity = player.GetComponent<PlayerStats>().speed;
        } else {
            playerVelocity = 10;
        }
    }

    void Update() {
        playerVelocity = playerStats.speed;
        followPlayer();
    }

    public void SnapToPlayerPosition() {
        this.transform.position = new Vector3((player.transform.position.x + offsetX),
                                               (player.transform.position.y + offsetY),
                                               (player.transform.position.z + offsetZ));
    }

    public void PrepareZoomOutEffect() {
        mainCamera.orthographicSize = zoomStartSize;
        mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x,
                                                            mainCamera.transform.localPosition.y - 2f,
                                                            mainCamera.transform.localPosition.z);
    }

    public void ZoomOutEffect() {
        StartCoroutine(ZoomOutCoroutine());
    }

    private IEnumerator ZoomOutCoroutine() {
        float currentSize = zoomStartSize;

        mainCamera.orthographicSize = zoomStartSize;
        yield return waitBeforeZoom;

        while (currentSize <= mainCamera.orthographicSize) {
            yield return new WaitForEndOfFrame();
            currentSize += zoomSpeed * Time.deltaTime;
            if (currentSize > orthographicSize) {
                mainCamera.orthographicSize = orthographicSize;
                break;
            }
            mainCamera.orthographicSize = currentSize;
        }
    }

    private void followPlayer() {
        movementX = ((player.transform.position.x + offsetX - this.transform.position.x)) / maximumDistance;
        movementZ = ((player.transform.position.z + offsetZ - this.transform.position.z)) / maximumDistance;
        movementY = ((player.transform.position.y + offsetY - this.transform.position.y)) / maximumDistance;
        this.transform.position += new Vector3((movementX * playerVelocity * Time.deltaTime),
                                               (movementY * playerVelocity * Time.deltaTime),
                                               (movementZ * playerVelocity * Time.deltaTime));
    }
}
