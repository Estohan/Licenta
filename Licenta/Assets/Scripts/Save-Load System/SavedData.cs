using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedData {
    public int sizeZ;
    public int sizeX;
    public MazeCoords startCellPos;
    public MazeCoords finishCellPos;
    public List<MazeCellData> cells;

    public SavedData(Level currentLevel) {
        if (currentLevel != null) {
            (this.sizeZ, this.sizeX) = currentLevel.getDimensions();
            this.startCellPos = currentLevel.startCellPos;
            this.finishCellPos = currentLevel.finishCellPos;
            this.cells = new List<MazeCellData>();

            for (int z = 0; z < sizeZ; z++) {
                for (int x = 0; x < sizeX; x++) {
                    this.cells.Add(currentLevel.getCellData(z, x));
                }
            }
        } else {
            throw new System.Exception("SaveData: Unable to create SavedData package (Null currentLevel)");
        }
    }
}
