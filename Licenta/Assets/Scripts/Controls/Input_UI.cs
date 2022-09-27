using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InGameUI;

/*
 *      Subscribes to InputManager events and calls FullMap/Minimap functions
 *  when their corresponding keys are pressed.
 */

public class Input_UI : MonoBehaviour {

    [SerializeField]
    private bool fullMapVisible;
    [SerializeField]
    private bool miniMapVisible;

    private void Awake() {
        fullMapVisible = false;
        miniMapVisible = true;
    }

    private void Start() {
        GameManager.inputManager.UI.Openmap.started += _ => FullMapToggle();
        GameManager.inputManager.UI.ShowHideminimap.started += _ => MiniMapToggle();
        GameManager.inputManager.UI.Mapzoom.started += context => FullMapZoom(context.ReadValue<float>());
        GameManager.inputManager.UI.Grabmap.started += _ => PanMapCameraStarted();
        GameManager.inputManager.UI.Grabmap.canceled += _ => PanMapCameraStopped();
    }

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
