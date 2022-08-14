using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectConcealed : MonoBehaviour {
    [SerializeField]
    private bool alwaysConceal;
    [SerializeField]
    private List<float> concealingRotations;
    /*[SerializeField]
    private bool unreachable; // cannot be reached using raycast/linecast*/
    /*[SerializeField]
    private Material opaqueMaterial;*/
    [SerializeField]
    private Material concealingMaterial;

    // A concealable object is an object that does not conceal when it is
    // instantiated is such a way (rotated) so that it does not obstruct
    // vision - therefore it does not need to conceal even though it can.
    private bool doesConceal;
    private bool isConcealed;


    // Start is called before the first frame update
    void Start() {
        doesConceal = false;
        isConcealed = false;

        if (alwaysConceal) {
            doesConceal = true;
            this.GetComponent<MeshRenderer>().sharedMaterial = concealingMaterial;
            return;
        }

        Quaternion rotation = this.transform.rotation;

        if (concealingRotations == null) {
            return; // not assigned in the inspector
        }
        foreach (float concealingRotation in concealingRotations) {
            if (Quaternion.Angle(rotation, Quaternion.Euler(0f, concealingRotation, 0f)) < Constants.rotationEpsilon) {
                // concealedObject.layer = LayerMask.NameToLayer("ConcealableObjects");
                doesConceal = true;
                this.GetComponent<MeshRenderer>().sharedMaterial = concealingMaterial;
            }
        }
    }

    public bool GetDoesConceal() {
        return doesConceal;
    }

    public bool GetIsConcealed() {
        return isConcealed;
    }

    public void SetIsConcealed(bool isConcealed) {
        this.isConcealed = isConcealed;
    }

    /*public bool GetIsUnreachable() {
        return unreachable;
    }*/
}
