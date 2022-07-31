using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Boulder_open_animation_event : MonoBehaviour {
    [SerializeField]
    private GameObject boulder;
    private Animator animatorBoulder;
    private int rollHash;

    private void Start() {
        animatorBoulder = boulder.GetComponent<Animator>();
        rollHash = Animator.StringToHash("roll");
    }

    // This is called as an animation event at the end of the boulder slot
    // opening animation
    public void OnEndOfOpenAnimation() {
        // Second part: roll the boulder down the hallway
        animatorBoulder.SetTrigger(rollHash);
    }

    public void OnEndOfCloseAnimation() {

    }
}
