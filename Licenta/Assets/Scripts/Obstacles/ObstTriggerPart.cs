using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstTriggerPart : MonoBehaviour {
    /* OPTs (OnePartTriggers) trigger all obstacles when the player touches the trigger
       TPTs (TwoPartTriggers) anounces that obstacles are about to be triggered when the
        player touches the trigger but only triggers them when the player stops touching
        the trigger
    */
    public bool isTwoPartTrigger;
    MeshRenderer meshRenderer;
    [Space]
    public List<ObstActivePart> obstaclesToBeTriggered;

    public void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // One part trigger: trigger all obstacles in obstaclesToBeTriggered
    private void OPT_TriggerObstacles() {
        if(obstaclesToBeTriggered.Count != 0) {
            for(int i = 0; i < obstaclesToBeTriggered.Count; i ++) {
                obstaclesToBeTriggered[i].Trigger();
            }
        }
    }

    // Two part trigger - first part: the trap is about to be triggered,
    // obstacles or sources of danger will make themselves known at this point
    private void TPT_1_TriggerObstacles() {
        if(obstaclesToBeTriggered.Count != 0) {
            for(int i = 0; i < obstaclesToBeTriggered.Count; i ++) {
                obstaclesToBeTriggered[i].AnounceIfPossible();
            }
        }
    }

    // Two part trigger - second part: the obstacles anounced themselves and
    // are now getting triggered
    private void TPT_2_TriggerObstacles() {
        OPT_TriggerObstacles();
    }

    /*private void OnCollisionEnter(Collision collision) {
        Debug.Log("CollisionEnter triggered by " + collision.gameObject.transform.name);
        if (collision.gameObject.transform.CompareTag("Player")) {
            if (triggerType == 1) {
                TriggerObstacles();
            }
        }
    }

    private void OnCollisionExit(Collision collision) {
        Debug.Log("CollisionExit triggered by " + collision.gameObject.transform.name);
        if (collision.gameObject.transform.CompareTag("Player")) {
            if (triggerType == 2) {
                TriggerObstacles();
            }
        }
    }*/

    private void OnTriggerEnter(Collider other) {
        // Debug.Log("TriggerEnter triggered by " + other.gameObject.transform.name);
        if (other.gameObject.transform.CompareTag("Player")) {
            if (meshRenderer != null) {
                meshRenderer.material.color = Color.red;
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
            if (meshRenderer != null) {
                meshRenderer.material.color = Color.white;
            }
            if (isTwoPartTrigger) {
                TPT_2_TriggerObstacles();
            }
        }
    }
}
