using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_Raycast : MonoBehaviour
{
    private int layerMask;

    private RaycastHit hitInfo;
    private bool hit;
    private ObjectConcealed objectConcealed;

    private void Start() {
        layerMask = (1 << LayerMask.NameToLayer("ConcealableObjects"));
    }


    private void OnTriggerEnter(Collider other) {

        objectConcealed = other.GetComponent<ObjectConcealed>();
        //if (objectConcealed.GetDoesConceal()) {
            hit = Physics.Linecast(this.transform.position, other.transform.position, out hitInfo, layerMask); // DELETE hit variable
            Debug.DrawLine(this.transform.position, other.transform.position, Color.white, 7f);
            //Debug.Log(other.transform.position + " " + hitInfo.transform + " " + hit);
            if (hit) {
                Debug.DrawLine(this.transform.position, hitInfo.transform.position + new Vector3(0.1f, 0.0f, 0.1f), Color.red, 7f);
                Debug.Log("Hit :" + hitInfo.transform.name + ", " + hitInfo.transform.position + " instead of " + other.transform.name + ", " + other.transform.position);
            }
            if (hit && Vector3.Distance(hitInfo.transform.position, other.transform.position) < Vector3.kEpsilon) {
                Debug.DrawLine(this.transform.position, other.transform.position + new Vector3(-0.1f, -0.0f, -0.1f), Color.green, 7f);
                other.gameObject.GetComponent<MeshRenderer>().enabled = false;
                objectConcealed.SetIsConcealed(true);
            }
        //}

        //}
    }

    private void OnTriggerExit(Collider other) {
        // Debug.Log("!");
        objectConcealed = other.GetComponent<ObjectConcealed>();
        if (objectConcealed.GetIsConcealed()) {
            other.gameObject.GetComponent<MeshRenderer>().enabled = true;
            objectConcealed.SetIsConcealed(false);
        }
    }


    /*private void OnTriggerEnter(Collider other) {
        // Debug.Log("!!");
        //if(((this.transform.position - other.transform.position).normalized.x > 0) &&
        //    (this.transform.position - other.transform.position).normalized.z < 0) {
        if (Physics.Linecast(this.transform.position, other.transform.position, out hit, layerMask)) {
            Debug.DrawLine(this.transform.position, other.transform.position + offset, Color.white, 6.5f, false);
            Debug.DrawLine(this.transform.position, hit.transform.position, Color.red, 6.5f, false);
            Debug.Log("Aimed for " + other.transform.name + " but hit " + hit.transform.name);
        } else {
            Debug.DrawLine(this.transform.position, other.transform.position, Color.green, 6.5f, false);
        }

        *//*if (other.GetComponent<ObjectConcealed>().DoesConceal() &&
            !Physics.Linecast(this.transform.position, other.transform.position, layerMask)) {
            other.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }*//*
        //}
    }*/
}
