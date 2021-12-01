using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutStats {
    // General stats
    public int sizeZ, sizeX;
    public int outerPaddingValZ, outerPaddingValX;
    public int totalOuterPadding;
    public int innerPaddingValZ, innerPaddingValX;
    public int totalInnerPadding;
    public int totalCore;

    public MazeCoords startCell, finishCell;

    // Sectors
    public List<MazeCoords>[] sectorBorder;
    public List<(MazeCoords, MazeDirection)>[] nextSectorExit;
    public List<(MazeCoords, MazeDirection)>[] passages;
    public List<MazeCoords>[] sectorCells;
    //public int[] sectorCellCount;
    public int numberOfSectors;

    public LayoutStats(int sizeZ, int sizeX, int numberOfSectors) {
        this.sizeZ = sizeZ;
        this.sizeX = sizeX;

        // Initialize sectors data
        this.numberOfSectors = numberOfSectors;
        sectorBorder = new List<MazeCoords>[numberOfSectors];
        nextSectorExit = new List<(MazeCoords, MazeDirection)>[numberOfSectors];
        passages = new List<(MazeCoords, MazeDirection)>[numberOfSectors];
        //sectorCellCount = new int[numberOfSectors]; // Default initialization with 0
        sectorCells = new List<MazeCoords>[numberOfSectors];
        for(int i = 0; i < numberOfSectors; i++) {
            sectorBorder[i] = new List<MazeCoords>();
            nextSectorExit[i] = new List<(MazeCoords, MazeDirection)>();
            passages[i] = new List<(MazeCoords, MazeDirection)>();
            sectorCells[i] = new List<MazeCoords>();
        }
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
