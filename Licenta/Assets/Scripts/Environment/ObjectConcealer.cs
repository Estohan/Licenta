using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectConcealer : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        Debug.Log("!!");
        //if(((this.transform.position - other.transform.position).normalized.x > 0) &&
        //    (this.transform.position - other.transform.position).normalized.z < 0) {

            other.gameObject.GetComponent<MeshRenderer>().enabled = false;
        //}
    }

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
        Debug.Log("!");
        other.gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
}
