using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCoords {

    // Holds the coordinates of a maze cell
    // by z (Row) and x (Column)
    public int z, x;

    public MazeCoords(int z, int x) {
        this.z = z;
        this.x = x;
    }

    public static MazeCoords operator + (MazeCoords a, MazeCoords b) {
        a.z += b.z;
        a.x += b.x;
        return a;
    }

    public override string ToString() {
        return "(" + z + ", " + x + ")";
    }
}