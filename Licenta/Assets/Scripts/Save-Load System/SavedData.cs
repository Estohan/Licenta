using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *      [ WIP ] Does not affect the game.
 *      Part of the data saved to disk when a game level is saved.
 */
[System.Serializable]
public class SavedData {
    public int sizeZ;
    public int sizeX;
    public LayoutStats stats;
    public List<MazeCellData> cells;

    public SavedData(Level currentLevel) {
        if (currentLevel != null) {
            (this.sizeZ, this.sizeX) = currentLevel.getDimensions(); // Redundant, remove this
            this.stats = currentLevel.stats;
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
