using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

    private Level level;
    private int sizeZ, sizeX;

    // GAMEOBJECTS - TEMPORARY LOCATION
    /*public GameObject _FloorGrey;
    public GameObject _FloorGreen;
    public GameObject _FloorRed;
    public GameObject _FloorWhite;
    public GameObject _Wall;
    public GameObject _Corner2;
    public GameObject _Corner1;
    public GameObject _Corner0;
    public GameObject _Start;
    public GameObject _Finish;
    public GameObject _OuterPadding;
    public GameObject _InnerPadding;*/
    public MazeCellObject _MazeCellObject;
    public GameObject _LevelRootObject;

    // !! This function will need some parameters
    // Uses MazeGenAlgorithms to generate a maze layout and
    // saves that layout into a two dimensional array of MazeCellData
    public Level GenerateLevel(int sizeZ, int sizeX, int outerPaddingDiv, int innerPaddingDiv, int nrOfSectors) {
        // minimum size of 30 ?
        this.sizeZ = sizeZ;
        this.sizeX = sizeX;

        //int[,,] layout = MazeGenAlgorithms.testAlgoritm(sizeZ, sizeX, 1);
        (int[,,] layout, MazeCoords startCellPos, MazeCoords finishCellPos) = 
            MazeGenAlgorithms.PrimsAlgorithm(sizeZ, sizeX, outerPaddingDiv, innerPaddingDiv, nrOfSectors);
        level = new Level(sizeZ, sizeX, layout);
        level.startCellPos = startCellPos;
        level.finishCellPos = finishCellPos;

        // printing layout
        /*string message = "Layout:\n";
        for (int k = 0; k < 5; k++) {
            message += "k = " + k + "\n";

            for (int i = 0; i < sizeX; i++) {
                for (int j = 0; j < sizeZ; j++) {
                    message += (layout[i, j, k] + " ");
                }
                message += "\n";
            }
            message += "\n";
        }
        print(message);*/

        return level;
    }

    public Level GenerateLevelFromSave(SavedData savedData) {
        this.sizeZ = savedData.sizeZ;
        this.sizeX = savedData.sizeX;

        level = new Level(sizeZ, sizeX, savedData);
        level.startCellPos = savedData.startCellPos;
        level.finishCellPos = savedData.finishCellPos;

        return level;
    }

    public void InstantiateLevel() {
        // Create a Level object that will hold all the level contents
        Transform rootParent = Instantiate(_LevelRootObject, this.transform).transform;

        // For each cell create its parent cell object and then
        // instantiate its contents as children
        for (int z = 0; z < sizeZ; z++) {
            for (int x = 0; x < sizeX; x++) {
                // Create cell object
                MazeCellObject newCellObject = Instantiate(_MazeCellObject, rootParent);
                newCellObject.data = level.getCellData(z, x);
                newCellObject.name = "Cell " + z + "-" + x;
                // newCellObject.transform.parent = this.transform;

                // Save newly created cell
                level.cellsObjects[z, x] = newCellObject;

                // Instantiate floors and move cells to their correct positions
                newCellObject.InstantiateFloor();

                // Instantiate walls
                newCellObject.InstantiateWalls();

                // Instantiate corners
                newCellObject.InstantiateCorners();

                // DEBUG
                newCellObject.DebugColor();
            }
        }
        /*Debug.Log("LevelGenerator: Level Instantiated.");*/
    }

    public Level GetLevel() {
        return this.level;
    }

    /*
     * |'5'|''''' ''1'' '''''|'6'|
     * |   |  9  |  10 |  11 |   |
     * | 4 |  12 |  13 |  14 | 2 |
     * |   |  15 |  16 |  17 |   |
     * |'8'|''''' ''3'' '''''|'7'|
     */
    public enum CellSubsections {
        NorthEdge, EastEdge, SouthEdge, WestEdge,
        NWCorner, NECorner, SECorner, SWCorner,
        NWInner, NInner, NEInner,
        WInner, Inner, EInner,
        SWInner, SInner, SEInner
    }
}
