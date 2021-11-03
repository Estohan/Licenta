using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level {
    private int sizeZ, sizeX;
    private MazeCellData[,] cellsData;
    public MazeCellObject[,] cellsObjects;
    public MazeCoords startCellPos;
    public MazeCoords finishCellPos;

    // Takes in the layout of the maze and puts it into
    // a two dimensional array of MazeCellData
    public Level(int sizeZ, int sizeX, int[,,] layout) {
        this.sizeZ = sizeZ;
        this.sizeX = sizeX;

        cellsData = new MazeCellData[sizeZ, sizeX];
        cellsObjects = new MazeCellObject[sizeZ, sizeX];

        int[] wallsAndType = new int[5];
        for(int z = 0; z < sizeZ; z ++) {
            for(int x = 0; x < sizeX; x ++) {
                wallsAndType[0] = layout[z, x, 0];
                wallsAndType[1] = layout[z, x, 1];
                wallsAndType[2] = layout[z, x, 2];
                wallsAndType[3] = layout[z, x, 3];
                wallsAndType[4] = layout[z, x, 4];
                cellsData[z, x] = new MazeCellData(new MazeCoords(z, x), wallsAndType);
            }
        }
    }

    public Level(int sizeZ, int sizeX, SavedData savedData) {
        this.sizeZ = sizeZ;
        this.sizeX = sizeX;

        cellsData = new MazeCellData[sizeZ, sizeX];
        cellsObjects = new MazeCellObject[sizeZ, sizeX];

        for (int z = 0; z < sizeZ; z++) {
            for (int x = 0; x < sizeX; x++) {
                cellsData[z, x] = savedData.cells[z * sizeX + x];
            }
        }
    }

    public MazeCellData getCellData(int z, int x) {
        return cellsData[z, x];
    }

    /*public MazeCellObject getCellObject(int x, int z) {
        return cellsObjects[x, z];
    }*/

    public (int, int) getDimensions() {
        return (sizeZ, sizeX);
    }
}
