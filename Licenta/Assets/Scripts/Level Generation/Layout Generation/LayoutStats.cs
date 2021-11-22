using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutStats {
    public int sizeZ, sizeX;
    public int outerPaddingValZ, outerPaddingValX;
    public int innerPaddingValZ, innerPaddingValX;

    public MazeCoords startCell, finishCell;

    public List<MazeCoords>[] sectorBorder;
    public List<(MazeCoords, MazeDirection)>[] nextSectorExit;
    public int[] sectorCellCount;
    public int numberOfSectors;

    public LayoutStats(int sizeZ, int sizeX, int numberOfSectors) {
        this.sizeZ = sizeZ;
        this.sizeX = sizeX;

        // Initialize sectors data
        this.numberOfSectors = numberOfSectors;
        sectorBorder = new List<MazeCoords>[numberOfSectors];
        nextSectorExit = new List<(MazeCoords, MazeDirection)>[numberOfSectors];
        sectorCellCount = new int[numberOfSectors]; // Default initialization with 0
        for(int i = 0; i < numberOfSectors; i++) {
            sectorBorder[i] = new List<MazeCoords>();
            nextSectorExit[i] = new List<(MazeCoords, MazeDirection)>();
        }
    }

    public void InitializePadding(int outerPaddingZ, int outerPaddingX, int innerPaddingZ, int innerPaddingX) {
        this.outerPaddingValZ = outerPaddingZ;
        this.outerPaddingValX = outerPaddingX;
        this.innerPaddingValZ = innerPaddingZ;
        this.innerPaddingValX = innerPaddingX;
    }

    public void SetStartAndFinish(MazeCoords startCell, MazeCoords finishCell) {
        this.startCell = startCell;
        this.finishCell = finishCell;
    }
}
