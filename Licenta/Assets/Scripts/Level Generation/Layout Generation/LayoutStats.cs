using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LayoutStats {
    // General stats
    public int sizeZ, sizeX;
    public int outerPaddingValZ, outerPaddingValX;
    public int totalOuterPadding;
    public int innerPaddingValZ, innerPaddingValX;
    public int totalInnerPadding;
    public int totalCore;

    public MazeCoords startCell, finishCell;

    // Sectors ----------------------------------------------------------- Nested containers are not serialized
    public List<MazeCoords>[] sectorBorder;
    public List<(MazeCoords, MazeDirection)>[] nextSectorExit;
    public List<(MazeCoords, MazeDirection)>[] passages;
    public List<MazeCoords>[] sectorCells;
    public int[] _accessibleCellsCount;
    public int numberOfSectors;

    // Rooms
    public List<RoomData>[] rooms;
    public List<RoomData> test;

    // Solution and dead ends
    public List<MazeCoords> solution;
    public CellStats[,] cellsStats;
    public List<MazeCoords> deadEnds;
    public List<MazeCoords> intersections;

    // Obstacles
    public List<(MazeCoords, int, MazeDirection, int)> obstacles; // anchor, shapeID, rotation and rec. diff.
    public int difficulty;
    public int chanceOfObstDowngrade;
    public int chanceOfObstUpgrade;

    public LayoutStats(int sizeZ, int sizeX, int numberOfSectors) {
        this.sizeZ = sizeZ;
        this.sizeX = sizeX;
        test = new List<RoomData>();

        // Initialize sectors data
        this.numberOfSectors = numberOfSectors;
        sectorBorder = new List<MazeCoords>[numberOfSectors];
        nextSectorExit = new List<(MazeCoords, MazeDirection)>[numberOfSectors];
        passages = new List<(MazeCoords, MazeDirection)>[numberOfSectors];
        _accessibleCellsCount = new int[numberOfSectors];
        sectorCells = new List<MazeCoords>[numberOfSectors];
        for (int i = 0; i < numberOfSectors; i++) {
            sectorBorder[i] = new List<MazeCoords>();
            nextSectorExit[i] = new List<(MazeCoords, MazeDirection)>();
            passages[i] = new List<(MazeCoords, MazeDirection)>();
            sectorCells[i] = new List<MazeCoords>();
            _accessibleCellsCount[i] = 0;
        }
        // Initialize rooms
        rooms = new List<RoomData>[numberOfSectors];
        for (int i = 0; i < numberOfSectors; i++) {
            rooms[i] = new List<RoomData>();
        }

        // Initialize solution and dead ends
        solution = new List<MazeCoords>();
        deadEnds = new List<MazeCoords>();
        intersections = new List<MazeCoords>();
        cellsStats = new CellStats[sizeZ, sizeX];
        for(int z = 0; z < sizeZ; z ++) {
            for(int x = 0; x < sizeX; x ++) {
                cellsStats[z, x] = new CellStats();
            }
        }

        // Initialize obstacles list
        obstacles = new List<(MazeCoords, int, MazeDirection, int)>();
    }

    public void InitializePadding(int outerPaddingZ, int outerPaddingX, int innerPaddingZ, int innerPaddingX) {
        this.outerPaddingValZ = outerPaddingZ;
        this.outerPaddingValX = outerPaddingX;
        this.innerPaddingValZ = innerPaddingZ;
        this.innerPaddingValX = innerPaddingX;
        totalOuterPadding = (sizeZ * outerPaddingValX * 2) + // Full left and right side of the frame
                            (outerPaddingValZ * (sizeX - 2* outerPaddingValX)) * 2; // What's left of top and down
    }

    public void SetStartAndFinish(MazeCoords startCell, MazeCoords finishCell) {
        this.startCell = startCell;
        this.finishCell = finishCell;
    }
}

[System.Serializable]
public class CellStats {
    //public bool isReachable; // ?
    public bool isInSolution;
    public bool isAdjacentToSectorGate;
    public bool isDeadEnd;
    public MazeDirection reachableFrom;

    public int distanceToStart;
    public int distanceToEnd;
    public int distanceToSolution;
    public int accessPoints; /* min. 1, max. 4 */
    public int sector;
    public List<(int, MazeDirection)> mappedObstacleShapes;

    public CellStats() {
        isInSolution = false;
        isAdjacentToSectorGate = false;
        reachableFrom = MazeDirection.North;

        distanceToStart = -1;
        distanceToEnd = -1;
        distanceToSolution = -1;
        accessPoints = 0;
        sector = -1;

        mappedObstacleShapes = new List<(int, MazeDirection)>();
    }
}
