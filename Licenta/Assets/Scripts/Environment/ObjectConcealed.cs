using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectConcealed : MonoBehaviour
{
    [SerializeField]
    private List<float> concealingRotations;

    // A concealable object is an object that does not conceal when it is
    // instantiated is such a way (rotated) so that it does not obstruct
    // vision - therefore it does not need to conceal even though it can.
    private bool doesConceal;


    // Start is called before the first frame update
    void Start() {
        doesConceal = false;
        Quaternion rotation = this.transform.rotation;

        foreach(float concealingRotation in concealingRotations) {
            if (Quaternion.Angle(rotation, Quaternion.Euler(0f, concealingRotation, 0f)) < Constants.rotationEpsilon) {
                // concealedObject.layer = LayerMask.NameToLayer("ConcealableObjects");
                doesConceal = true;
            }
        }
    }

    public bool DoesConceal() {
        return doesConceal;
    }
}
