using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectConcealer : MonoBehaviour {

    [SerializeField]
    // private float checkFrequency;
    // private bool canCheck;
    // private int layerMask;
    private ObjectConcealed objectConcealed;
    // private RaycastHit hitInfo;
    // private bool hit;
    // private WaitForSeconds waitSecondsBeforeCheck;

    private void Start() {
        /*layerMask = (1 << LayerMask.NameToLayer("ConcealableObjects"));

        if (checkFrequency == 0f) {
            checkFrequency = 1f;
        }

        canCheck = true;
        waitSecondsBeforeCheck = new WaitForSeconds(checkFrequency);*/
    }

    private void OnTriggerEnter(Collider other) {

        objectConcealed = other.GetComponent<ObjectConcealed>();
        if (objectConcealed.GetDoesConceal()) {
            Conceal(other.gameObject, objectConcealed);
        }
    }

    // (1) Raycasts
    /*private void OnTriggerEnter(Collider other) {

        objectConcealed = other.GetComponent<ObjectConcealed>();
        if (objectConcealed.GetDoesConceal()) {
            if (objectConcealed.GetIsUnreachable()) {
                Conceal(other.gameObject, objectConcealed);
            }

            hit = Physics.Linecast(this.transform.position, other.transform.position, out hitInfo, layerMask);

            *//*Debug.DrawLine(this.transform.position, other.transform.position, Color.white, 7f);
            if (hit) {
                Debug.DrawLine(this.transform.position, hitInfo.transform.position + new Vector3(0.1f, 0.0f, 0.1f), Color.red, 7f);
                Debug.Log("Hit :" + hitInfo.transform.name + ", " + hitInfo.transform.position + " instead of " + other.transform.name + ", " + other.transform.position);
            }*//*
            if (hit && Vector3.Distance(hitInfo.transform.position, other.transform.position) < Vector3.kEpsilon) {
                // Debug.DrawLine(this.transform.position, other.transform.position + new Vector3(-0.1f, -0.0f, -0.1f), Color.green, 7f);
                Conceal(other.gameObject, objectConcealed);
            }
        }
    }*/

    // (1) RaycastsOnTriggerStay max
    /*private void OnTriggerStay(Collider other) {

        objectConcealed = other.GetComponent<ObjectConcealed>();
        if (objectConcealed.GetDoesConceal() && !objectConcealed.GetIsConcealed()) {
            Debug.Log("Call!");
            hit = Physics.Linecast(this.transform.position, other.transform.position, out hitInfo, layerMask);
            if (Vector3.Distance(hitInfo.transform.position, other.transform.position) < Vector3.kEpsilon) {
                Conceal(other.gameObject, objectConcealed);
            }
        }
    }*/

    // (1.2) Raycasts OnTriggerStay rare
    /*private void OnTriggerStay(Collider other) {
        if(canCheck) {
            StartCoroutine(OnTriggerStayCheck(other));
        }
    }

    private IEnumerator OnTriggerStayCheck(Collider other) {

        canCheck = false;

        objectConcealed = other.GetComponent<ObjectConcealed>();
        if (objectConcealed.GetDoesConceal() && !objectConcealed.GetIsConcealed()) {
            Debug.Log("Call!");
            hit = Physics.Linecast(this.transform.position, other.transform.position, out hitInfo, layerMask);
            //Debug.DrawLine(this.transform.position, other.transform.position + new Vector3(-0.1f, -0.0f, -0.1f), Color.green, 7f);
            if (Vector3.Distance(hitInfo.transform.position, other.transform.position) < Vector3.kEpsilon) {
                Conceal(other.gameObject, objectConcealed);
            }
        }

        yield return waitSecondsBeforeCheck;
        canCheck = true;
    }*/

    private void OnTriggerExit(Collider other) {
        objectConcealed = other.GetComponent<ObjectConcealed>();
        if (objectConcealed.GetIsConcealed()) {
            Reveal(other.gameObject, objectConcealed);
        }
    }

    private void Conceal(GameObject obj, ObjectConcealed objConcealed) {
        obj.gameObject.GetComponent<MeshRenderer>().enabled = false;
        objConcealed.SetIsConcealed(true);
    }

    private void Reveal(GameObject obj, ObjectConcealed objConcealed) {
        obj.gameObject.GetComponent<MeshRenderer>().enabled = true;
        objConcealed.SetIsConcealed(false);
    }

    // (1) Raycasts
    /*private void OnTriggerExit(Collider other) {
        objectConcealed = other.GetComponent<ObjectConcealed>();
        if (objectConcealed.GetIsConcealed()) {
            Reveal(other.gameObject, objectConcealed);
        }
    }*/

}
