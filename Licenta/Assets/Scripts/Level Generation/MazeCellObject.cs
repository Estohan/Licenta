using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCellObject : MonoBehaviour {

    public MazeCellData data;

    public float size;

    // GAME OBJECTS ?
    /*public GameObject Prf_FloorGrey;
    public GameObject Prf_FloorGreen;
    public GameObject Prf_FloorRed;
    public GameObject Prf_FloorWhite;*/

    public MazeCellObject(MazeCoords coordinates) {
        this.data.coordinates = coordinates;
    }
}
