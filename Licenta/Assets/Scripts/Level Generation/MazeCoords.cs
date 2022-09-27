using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *      Representation of a pair of coordinates: z (row) and x (column).
 */
[System.Serializable]
public class MazeCoords {
    public int z;
    public int x;

    public MazeCoords(int z, int x) {
        this.z = z;
        this.x = x;
    }

    public MazeCoords(MazeCoords coords) {
        this.z = coords.z;
        this.x = coords.x;
    }

    public static MazeCoords operator + (MazeCoords a, MazeCoords b) {
        /*a.z += b.z;
        a.x += b.x;*/
        return new MazeCoords(a.z + b.z, a.x + b.x);
    }

    public static MazeCoords operator + (MazeCoords a, (int, int) intPair) {
        return new MazeCoords(a.z + intPair.Item1, a.x + intPair.Item2);
    }

    public override string ToString() {
        return "(" + z + ", " + x + ")";
    }
}
