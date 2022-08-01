using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectConcealed : MonoBehaviour
{
    [SerializeField]
    private List<float> concealingRotations;
    [SerializeField]
    private GameObject concealedObject;


    // Start is called before the first frame update
    void Start() {
        float rotation = this.transform.rotation.y;

        foreach(float concealingRotation in concealingRotations) {
            Debug.Log(rotation + " " + concealingRotation);
            if (rotation == concealingRotation) {
                concealedObject.layer = LayerMask.NameToLayer("ConcealableObjects");
            } else {
                concealedObject.layer = LayerMask.NameToLayer("Default");
            }
        }
    }
}
