using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectConcealer : MonoBehaviour {

    private int layerMask;

    private RaycastHit hit;
    private Vector3 offset;

    private void Start() {
        layerMask = (1 << LayerMask.NameToLayer("ConcealableObjects"));
        offset = new Vector3(0.05f, 0.00f, 0.05f);
    }

    private void OnTriggerEnter(Collider other) {
        // Debug.Log("!!");
        //if(((this.transform.position - other.transform.position).normalized.x > 0) &&
        //    (this.transform.position - other.transform.position).normalized.z < 0) {
        if (Physics.Raycast(this.transform.position, other.transform.position, out hit, layerMask)) {
            Debug.DrawLine(this.transform.position, other.transform.position + offset, Color.white, 6.5f, false);
            Debug.DrawLine(this.transform.position, hit.transform.position, Color.red, 6.5f, false);
            Debug.Log("Aimed for " + other.transform.name + " but hit " + hit.transform.name);
        } else {
            Debug.DrawLine(this.transform.position, other.transform.position, Color.green, 6.5f, false);
        }
        
        if (other.GetComponent<ObjectConcealed>().DoesConceal() &&
            !Physics.Linecast(this.transform.position, other.transform.position, layerMask)) {
            other.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        //}
    }

    /*private void OnTriggerStay(Collider other) {
        if (Physics.Linecast(this.transform.position, other.transform.position, out hit, layerMask)) {
            Debug.DrawLine(this.transform.position, other.transform.position, Color.white, 0.5f, false);
            Debug.DrawLine(this.transform.position, hit.transform.position, Color.red, 0.5f, false);
        } else {
            Debug.DrawLine(this.transform.position, other.transform.position, Color.green, 0.5f, false);
        }
        if (other.GetComponent<ObjectConcealed>().IsConcealing() && 
            !Physics.Linecast(this.transform.position, other.transform.position, layerMask)) {
            other.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }*/

    /*private void OnTriggerStay(Collider other) {
        //Debug.Log("!!!");
        if (!alreadyConcealed &&
            other.gameObject.transform.parent.gameObject.CompareTag("Player") &&
            ((this.transform.position - other.transform.position).normalized.x > 0 ||
            (this.transform.position - other.transform.position).normalized.z < 0)) {

            concealablePart.SetActive(false);
            alreadyConcealed = true;
        }
    }*/

    private void OnTriggerExit(Collider other) {
        // Debug.Log("!");
        other.gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
}
