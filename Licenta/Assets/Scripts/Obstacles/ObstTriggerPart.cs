using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstTriggerPart : MonoBehaviour {
    /* OPTs (OnePartTriggers) trigger all obstacles when the player touches the trigger
       TPTs (TwoPartTriggers) anounces that obstacles are about to be triggered when the
        player touches the trigger but only triggers them when the player stops touching
        the trigger
    */
    public List<ObstActivePart> obstaclesToBeTriggered;
    [Space]

    [SerializeField]
    private bool isTwoPartTrigger; // triggers in two steps
    [SerializeField]
    private bool isOneTimeTrigger; // can be triggered only once
    [SerializeField]
    private Collider triggerCollider;
    [SerializeField]
    private bool canToleratePosture; // does not trigger if player is in toleratedPosture
    [SerializeField]
    private PlayerControls.PlayerPostureState toleratedPosture;
    [SerializeField]
    private bool changeStateOnTrigger;
    [SerializeField]
    private bool changeColorOnTrigger;
    [SerializeField]
    private GameObject colorChangingPart;
    [SerializeField]
    private Color newColor;
    
    [Header("Pre-trigger state")]
    [SerializeField]
    private GameObject preTriggerState;
    [Header("Post-trigger state")]
    [SerializeField]
    private GameObject postTriggerState;

    private MeshRenderer colorChPtMeshRenderer;
    private Color originalColor; //

    public void Start() {
        // If trigger can signal being triggered by its changing color
        if (changeColorOnTrigger && colorChangingPart != null) {
            colorChPtMeshRenderer = colorChangingPart.gameObject.GetComponent<MeshRenderer>();
            originalColor = colorChPtMeshRenderer.sharedMaterial.color;
        } else {
            // in case changeColorOnTrigger was true but no colorChangingPart attached
            changeColorOnTrigger = false;
        }
        // If trigger has multiple states
        if (changeStateOnTrigger) {
            // show set trigger
            preTriggerState.SetActive(true);
            // hide broken trigger 
            postTriggerState.SetActive(false);
        }
    }

    // One part trigger: trigger all obstacles in obstaclesToBeTriggered
    private void OPT_TriggerObstacles() {
        if (obstaclesToBeTriggered.Count != 0) {
            // Trigger all attached obstacle active parts
            for (int i = 0; i < obstaclesToBeTriggered.Count; i ++) {
                obstaclesToBeTriggered[i].Trigger();
            }
        }
        // Disable collider if this can trigger only once
        if (isOneTimeTrigger) {
            // this.transform.GetComponent<BoxCollider>().enabled = false;
            triggerCollider.enabled = false;
        }
        // Change trigger state if possible
        if (changeStateOnTrigger) {
            // hide set trigger
            preTriggerState.SetActive(false);
            // show broken trigger
            postTriggerState.SetActive(true);
        }
    }

    // Two part trigger - first part: the trap is about to be triggered,
    // obstacles or sources of danger will make themselves known at this point
    private void TPT_1_TriggerObstacles() {
        if (obstaclesToBeTriggered.Count != 0) {
            for (int i = 0; i < obstaclesToBeTriggered.Count; i ++) {
                obstaclesToBeTriggered[i].AnounceIfPossible();
            }
        }
    }

    // Two part trigger - second part: the obstacles anounced themselves and
    // are now getting triggered
    private void TPT_2_TriggerObstacles() {
        OPT_TriggerObstacles();
    }

    private void OnTriggerEnter(Collider other) {
        // Debug.Log("TriggerEnter triggered by " + other.gameObject.transform.name);
        if (other.gameObject.transform.CompareTag("Player")) {
            // If player's current posture is tolerated the trigger does nothing
            if(canToleratePosture &&
                other.gameObject.GetComponent<PlayerStats>().currentPosture == toleratedPosture) {
                return;
            }
            // Change trigger color if possible
            if (changeColorOnTrigger) {
                colorChPtMeshRenderer.material.color = newColor;
            }
            if (!isTwoPartTrigger) {
                OPT_TriggerObstacles();
            } else {
                TPT_1_TriggerObstacles();
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        // Debug.Log("TriggerExit triggered by " + other.gameObject.transform.name);
        if (other.gameObject.transform.CompareTag("Player")) {
            if (canToleratePosture) {
                // If player's current posture is tolerated the trigger does nothing
                if (other.gameObject.GetComponent<PlayerStats>().currentPosture == toleratedPosture) {
                    return;
                    // If player's posture when exiting the collider is one not tolerated , the
                    // trigger is activated
                } else {
                    // Change trigger color if possible
                    if (changeColorOnTrigger) {
                        colorChPtMeshRenderer.material.color = newColor;
                    }
                    if (!isTwoPartTrigger) {
                        OPT_TriggerObstacles();
                    } else {
                        TPT_1_TriggerObstacles();
                    }
                }
            }
            // Revert trigger to its initial color
            if (changeColorOnTrigger) {
                colorChPtMeshRenderer.material.color = originalColor;
            }
            if (isTwoPartTrigger) {
                TPT_2_TriggerObstacles();
            }
        }
    }
}
