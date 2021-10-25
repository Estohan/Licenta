using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCellData {

    public MazeCoords coordinates;
    public bool[] walls;
    public int[] cornerFaces;
    // WIP
    public int sector;
    public CellType type;

    public MazeCellData(MazeCoords coordinates, int[] wallsAndType) {
        this.coordinates = coordinates;
        // Save walls, sector and type
        walls = new bool[] { false, false, false, false };
        cornerFaces = new int[] { 2, 2, 2, 2 };
        // North wall
        if(wallsAndType[0] == 1) {
            walls[0] = true;
            cornerFaces[0]--;
            cornerFaces[1]--;
        }
        // East wall
        if (wallsAndType[1] == 1) {
            walls[1] = true;
            cornerFaces[1]--;
            cornerFaces[2]--;
        }
        // South wall
        if (wallsAndType[2] == 1) {
            walls[2] = true;
            cornerFaces[2]--;
            cornerFaces[3]--;
        }
        // West wall
        if (wallsAndType[3] == 1) {
            walls[3] = true;
            cornerFaces[3]--;
            cornerFaces[0]--;
        }
        sector = wallsAndType[4] % 10;
        type = (CellType) (wallsAndType[4] / 10);
    }

    public bool HasWallInDirection(MazeDirection direction) {
        return walls[(int) direction];
    }
}
