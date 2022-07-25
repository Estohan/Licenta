using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcealableObject : MonoBehaviour {
    [SerializeField]
    private GameObject concealablePart;

    public GameObject GetConcealablePart() {
        return concealablePart;
    }
}
