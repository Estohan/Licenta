using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MazeCellData {

    public MazeCoords coordinates;
    public bool[] walls;
    public int[] cornerFaces; //  NW, NE, SE, SW
    public int sector;
    public CellType type;

    public CellStats cellStats;

    // (stage, type, index)
    // 0 - floor
    // 1..4 - walls
    // 5..8 - corners
    // 9..17 - subsections
    public (int, int, int)[] objectReferences;
    public bool[] hasObjectReference;
    public MazeDirection[] objectsRotations; // store all rotations here when implementing save/load functionalities !!

    // If part of a room
    public int roomObjStage;
    public RoomData room;
    public MazeCoords offsetToRoomAnchor;

    // If trapped
    public bool obst_anchor;
    public int obst_shapeID;
    public int obst_obstacleID;
    public int obst_difficulty;
    public MazeDirection obst_rotation;

    public MazeCellData(MazeCoords coordinates, int[] data) {
        this.coordinates = coordinates;
        // Save walls, sector and type
        walls = new bool[] { false, false, false, false };
        cornerFaces = new int[] { 2, 2, 2, 2 };
        // North wall
        if(data[0] == 1) {
            walls[0] = true;
            cornerFaces[0]--;
            cornerFaces[1]--;
        }
        // East wall
        if (data[1] == 1) {
            walls[1] = true;
            cornerFaces[1]--;
            cornerFaces[2]--;
        }
        // South wall
        if (data[2] == 1) {
            walls[2] = true;
            cornerFaces[2]--;
            cornerFaces[3]--;
        }
        // West wall
        if (data[3] == 1) {
            walls[3] = true;
            cornerFaces[3]--;
            cornerFaces[0]--;
        }
        // Type
        type = (CellType) data[4];
        // Sector
        sector = data[5];
        // Initialize object references array
        objectReferences = new (int, int, int)[18];
        hasObjectReference = new bool[18];
        objectsRotations = new MazeDirection[18];
    }

    public bool HasWallInDirection(MazeDirection direction) {
        return walls[(int) direction];
    }
}
