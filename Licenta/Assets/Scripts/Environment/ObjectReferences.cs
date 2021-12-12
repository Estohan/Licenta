using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectReferences : MonoBehaviour {
    public static ObjectReferences instance;

    // Parent cell object
    public MazeCellObject _MazeCellObject;

    // Floors
    public GameObject _FloorGrey;
    public GameObject _FloorGreen;
    public GameObject _FloorRed;
    public GameObject _FloorWhite;
    public GameObject _Start;
    public GameObject _Finish;
    public GameObject _OuterPadding;
    public GameObject _InnerPadding;

    // Walls
    public GameObject _Wall;

    // Corners
    public GameObject _Corner0;
    public GameObject _Corner1;
    public GameObject _Corner2;


    private void Awake() {
        if (instance != null && instance != this) {
            Debug.LogError("Duplicate instance of ObjectReferences.\n");
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }
}
