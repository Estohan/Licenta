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
        fullMapVisible = false;
        miniMapVisible = true;
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
        
        if(Input.GetKeyDown(KeyCode.O)) {
            if (miniMapVisible) {
                Map.MinimapWindow.Hide();
                miniMapVisible = false;
            } else {
                Map.MinimapWindow.Show();
                miniMapVisible = true;
            }
        }
    }
}
