using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Boulder_break_animation_event : MonoBehaviour {
    [SerializeField]
    private SkinnedMeshRenderer brokenBoulderRenderer;

    public void OnEndOfBreakAnimation() {
        // Broken boulder is hidden so that it won't be seen when
        // Announce() is called and it is brought back up
        brokenBoulderRenderer.enabled = false;
    }
}
