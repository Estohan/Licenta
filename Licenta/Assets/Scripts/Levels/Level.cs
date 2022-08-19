using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level {
    public int stage;
    public int sizeZ, sizeX;
    public MazeCellData[,] cellsData;
    public MazeCellObject[,] cellsObjects;
    public LayoutStats stats;
    public List<(MazeCoords, int, MazeDirection, int, int)> obstaclesList;

    // Takes in the layout of the maze and puts it into
    // a two dimensional array of MazeCellData
    public Level(int sizeZ, int sizeX, int[,,] layout, LayoutStats stats) {
        stage = 1;

        this.sizeZ = sizeZ;
        this.sizeX = sizeX;
        this.stats = stats;

        cellsData = new MazeCellData[sizeZ, sizeX];
        cellsObjects = new MazeCellObject[sizeZ, sizeX];
        obstaclesList = new List<(MazeCoords, int, MazeDirection, int, int)>();

        int[] data = new int[6];
        for(int z = 0; z < sizeZ; z ++) {
            for(int x = 0; x < sizeX; x ++) {
                data[0] = layout[z, x, 0];
                data[1] = layout[z, x, 1];
                data[2] = layout[z, x, 2];
                data[3] = layout[z, x, 3];
                data[4] = layout[z, x, 4];
                data[5] = layout[z, x, 5];
                cellsData[z, x] = new MazeCellData(new MazeCoords(z, x), data);
                cellsData[z, x].cellStats = stats.cellsStats[z, x];
            }
        }
    }

    public Level(int sizeZ, int sizeX, SavedData savedData) {
        this.sizeZ = sizeZ;
        this.sizeX = sizeX;
        this.stats = savedData.stats;

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
