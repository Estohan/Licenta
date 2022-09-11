using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObjectsDatabase : MonoBehaviour {

    public static RoomObjectsDatabase instance;

    

    private void Awake() {
        if (instance != null && instance != this) {
            // Debug.LogError("Duplicate instance of RoomObjectDatabase.\n");
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }
}
