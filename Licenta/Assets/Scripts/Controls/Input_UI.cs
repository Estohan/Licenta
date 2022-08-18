using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InGameUI;

public class Input_UI : MonoBehaviour {

    [SerializeField]
    private bool fullMapVisible;
    [SerializeField]
    private bool miniMapVisible;
    //InputManager inputManager;

    private void Awake() {
        fullMapVisible = false;
        miniMapVisible = true;
        // Map.FullmapWindow.Show();

    }

    private void Start() {
        //inputManager = new InputManager();
        GameManager.inputManager.UI.Openmap.started += _ => FullMapToggle();
        GameManager.inputManager.UI.ShowHideminimap.started += _ => MiniMapToggle();
        GameManager.inputManager.UI.Mapzoom.started += context => FullMapZoom(context.ReadValue<float>());
        GameManager.inputManager.UI.Grabmap.started += _ => PanMapCameraStarted();
        GameManager.inputManager.UI.Grabmap.canceled += _ => PanMapCameraStopped();
    }

    private void Update() {

        /*
        // Fullmap camera zoom
        if (fullMapVisible && Input.GetAxis("Mouse ScrollWheel") != 0f) {
            Map.FullmapWindow.instance.Zoom(Input.GetAxis("Mouse ScrollWheel"));
        }*/
    }

    /*private void OnEnable() {
        inputManager.UI.Enable();
    }

    private void OnDisable() {
        inputManager.UI.Disable();
    }*/

    private void FullMapToggle() {
        if (fullMapVisible) {
            InGameUI.FullmapWindow.Hide();
            InGameUI.MinimapWindow.Show();
            fullMapVisible = false;
        } else {
            InGameUI.FullmapWindow.Show();
            InGameUI.MinimapWindow.Hide();
            fullMapVisible = true;
        }
    }

    private void MiniMapToggle() {
        if (miniMapVisible) {
            InGameUI.MinimapWindow.Hide();
            miniMapVisible = false;
        } else {
            InGameUI.MinimapWindow.Show();
            miniMapVisible = true;
        }
    }

    private void FullMapZoom(float scrollOffset) {
        if (fullMapVisible) {
            InGameUI.FullmapWindow.instance.Zoom(scrollOffset);
        }
    }

    private void PanMapCameraStarted() {
        if (fullMapVisible) {
            InGameUI.FullmapWindow.instance.PanMapCamera(true);
        }
    }

    private void PanMapCameraStopped() {
        if (fullMapVisible) {
            InGameUI.FullmapWindow.instance.PanMapCamera(false);
        }
    }
}
