using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;

public class InputHandler : MonoBehaviour
{
    // This should probably be moved somewhere else, a GameState maybe
    [SerializeField]
    private bool fullMapVisible;
    [SerializeField]
    private bool miniMapVisible;

    private void Awake() {
        fullMapVisible = true;
        miniMapVisible = true;
        Map.FullmapWindow.Show();
    }

    // Update is called once per frame
    void Update()
    {
        // Fullmap visibility
        if(Input.GetKeyDown(KeyCode.Tab)) {
            if (fullMapVisible) {
                Map.FullmapWindow.Hide();
                fullMapVisible = false;
            } else {
                Map.FullmapWindow.Show();
                fullMapVisible = true;
            }
        }
        
        // Minimap visibility
        if(Input.GetKeyDown(KeyCode.O)) {
            if (miniMapVisible) {
                Map.MinimapWindow.Hide();
                miniMapVisible = false;
            } else {
                Map.MinimapWindow.Show();
                miniMapVisible = true;
            }
        }

        // Restart game
        if(Input.GetKeyDown(KeyCode.R)) {
            GameManager.instance.RestartGame();
        }

        // Fullmap camera zoom
        if(fullMapVisible && Input.GetAxis("Mouse ScrollWheel") != 0f) {
            Map.FullmapWindow.instance.Zoom(Input.GetAxis("Mouse ScrollWheel"));
        }
    }
}
