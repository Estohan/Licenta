using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;

public class Input_UI : MonoBehaviour {

    [SerializeField]
    private bool fullMapVisible;
    [SerializeField]
    private bool miniMapVisible;
    InputManager inputManager;

    private void Awake() {
        fullMapVisible = false;
        miniMapVisible = true;
        // Map.FullmapWindow.Show();

        inputManager = new InputManager();
        inputManager.UI.Openmap.started += _ => FullMapToggle();
        inputManager.UI.ShowHideminimap.started += _ => MiniMapToggle();
        inputManager.UI.Mapzoom.started += context => FullMapZoom(context.ReadValue<float>());
        inputManager.UI.Grabmap.started += _ => PanMapCameraStarted();
        inputManager.UI.Grabmap.canceled += _ => PanMapCameraStopped();
    }


    private void Update() {

        /*
        // Fullmap camera zoom
        if (fullMapVisible && Input.GetAxis("Mouse ScrollWheel") != 0f) {
            Map.FullmapWindow.instance.Zoom(Input.GetAxis("Mouse ScrollWheel"));
        }*/
    }

    private void OnEnable() {
        inputManager.UI.Enable();
    }

    private void OnDisable() {
        inputManager.UI.Disable();
    }

    private void FullMapToggle() {
        if (fullMapVisible) {
            Map.FullmapWindow.Hide();
            Map.MinimapWindow.Show();
            fullMapVisible = false;
        } else {
            Map.FullmapWindow.Show();
            Map.MinimapWindow.Hide();
            fullMapVisible = true;
        }
    }

    private void MiniMapToggle() {
        if (miniMapVisible) {
            Map.MinimapWindow.Hide();
            miniMapVisible = false;
        } else {
            Map.MinimapWindow.Show();
            miniMapVisible = true;
        }
    }

    private void FullMapZoom(float scrollOffset) {
        if (fullMapVisible) {
            Map.FullmapWindow.instance.Zoom(scrollOffset);
        }
    }

    private void PanMapCameraStarted() {
        if (fullMapVisible) {
            Map.FullmapWindow.instance.PanMapCamera(true);
        }
    }

    private void PanMapCameraStopped() {
        if (fullMapVisible) {
            Map.FullmapWindow.instance.PanMapCamera(false);
        }
    }
}
