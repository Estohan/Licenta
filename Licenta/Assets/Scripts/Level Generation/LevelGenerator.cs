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
    /*public MazeCellObject _MazeCellObject;
    public GameObject _LevelRootObject;*/

    // !! This function will need some parameters
    // Uses MazeGenAlgorithms to generate a maze layout and
    // saves that layout into a two dimensional array of MazeCellData
    public Level GenerateLevel(int sizeZ, int sizeX, int outerPaddingPerc, int innerPaddingPerc, int nrOfSectors) {
        // minimum size of 30 ?
        this.sizeZ = sizeZ;
        this.sizeX = sizeX;

        //int[,,] layout = MazeGenAlgorithms.testAlgoritm(sizeZ, sizeX, 1);
        (int[,,] layout, LayoutStats stats) = 
            MazeGenAlgorithms.GenerateLayout(sizeZ, sizeX, outerPaddingPerc, innerPaddingPerc, nrOfSectors);
        level = new Level(sizeZ, sizeX, layout, stats);

        AssignRoomsData(stats.rooms, nrOfSectors);
        AssignWallsAndFloors();

        // printing layout
        /*string message = "Layout:\n";
        for (int k = 0; k < 6; k++) {
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

    private void AssignRoomsData(List<RoomData>[] rooms, int nrOfSectors) {
        MazeCoords anchor, cellCoords;
        MazeDirection rotation;
        int size;
        int index;
        List<(int, int)> roomCellsOffsets;

        for(int sector = 1; sector <= nrOfSectors; sector ++) {
            foreach(RoomData room in rooms[sector - 1]) {
                anchor = room.anchor;
                size = room.size;
                index = room.index;
                rotation = room.rotation;
                roomCellsOffsets = RoomLayouts.rooms[size - 1][index].GetRotation(rotation);
                foreach((int z, int x) in roomCellsOffsets) {
                    cellCoords = anchor + (z, x);
                    level.cellsData[cellCoords.z, cellCoords.x].offsetToRoomAnchor = new MazeCoords(z, x);
                    level.cellsData[cellCoords.z, cellCoords.x].room = room;
                    level.cellsData[cellCoords.z, cellCoords.x].roomObjStage = level.stage;
                }
            }
        }
    }

    private void AssignWallsAndFloors() {
        int objStage = level.stage;
        int objIndex;
        for(int z = 0; z < sizeZ; z ++) {
            for(int x = 0; x < sizeX; x ++) {
                // [TODO] Insert some logic to decide between multiple objects
                // of the same type
                switch(level.cellsData[z, x].type) {
                    case CellType.OuterPadding:
                        // Floor
                        level.cellsData[z, x].objectReferences[0] = (objStage, (int)ObjectType.OuterPadding, 0);
                        level.cellsData[z, x].hasObjectReference[0] = true;
                        break;
                    case CellType.InnerPadding:
                        // Floor
                        level.cellsData[z, x].objectReferences[0] = (objStage, (int)ObjectType.InnerPadding, 0);
                        level.cellsData[z, x].hasObjectReference[0] = true;
                        break;
                    case CellType.Room:
                        for(int i = 0; i < 18; i ++) {
                            level.cellsData[z, x].hasObjectReference[i] = true;
                        }
                        break;
                    case CellType.Start:
                    case CellType.Finish:
                    case CellType.Common:
                        // Walls
                        // [TODO] Choose 4 walls?
                        level.cellsData[z, x].objectReferences[1] = (objStage, (int)ObjectType.NEWall, 0);
                        level.cellsData[z, x].hasObjectReference[1] = true;
                        level.cellsData[z, x].objectReferences[2] = (objStage, (int)ObjectType.NEWall, 0);
                        level.cellsData[z, x].hasObjectReference[2] = true;
                        level.cellsData[z, x].objectReferences[3] = (objStage, (int)ObjectType.SWWall, 0);
                        level.cellsData[z, x].hasObjectReference[3] = true;
                        level.cellsData[z, x].objectReferences[4] = (objStage, (int)ObjectType.SWWall, 0);
                        level.cellsData[z, x].hasObjectReference[4] = true;
                        // Corners
                        for (int i = 0; i < 4; i++) {
                            switch (level.cellsData[z, x].cornerFaces[i]) {
                                case 1:
                                    level.cellsData[z, x].objectReferences[5 + i] = (objStage, (int)ObjectType.OneFaceCorner, 0);
                                    level.cellsData[z, x].hasObjectReference[5 + i] = true;
                                    break;
                                case 2:
                                    level.cellsData[z, x].objectReferences[5 + i] = (objStage, (int)ObjectType.TwoFaceCorner, 0);
                                    level.cellsData[z, x].hasObjectReference[5 + i] = true;
                                    break;
                                default:
                                    level.cellsData[z, x].objectReferences[5 + i] = (objStage, (int)ObjectType.NoFaceCorner, 0);
                                    level.cellsData[z, x].hasObjectReference[5 + i] = true;
                                    break;
                            }
                        }
                        // Floor
                        level.cellsData[z, x].objectReferences[0] = (objStage, (int)ObjectType.Floor, 0);
                        level.cellsData[z, x].hasObjectReference[0] = true;
                        break;
                }

                // [TODO] Replace this (custom floor for start and finish cells)
                if(level.cellsData[z, x].type == CellType.Start) {
                    level.cellsData[z, x].objectReferences[0] = (objStage, (int)ObjectType.Floor, 1);
                    level.cellsData[z, x].hasObjectReference[0] = true;
                }
                if (level.cellsData[z, x].type == CellType.Finish) {
                    level.cellsData[z, x].objectReferences[0] = (objStage, (int)ObjectType.Floor, 2);
                    level.cellsData[z, x].hasObjectReference[0] = true;
                }
            }
        }
    }

    public Level GenerateLevelFromSave(SavedData savedData) {
        this.sizeZ = savedData.sizeZ;
        this.sizeX = savedData.sizeX;

        level = new Level(sizeZ, sizeX, savedData);
        // ----------------------------------------------------------- Add stats in savedData
        // ----------------------------------------------------------- Stats are empty here!
        /*level.stats = new LayoutStats();
        level.stats.startCell = savedData.startCellPos;
        level.stats.finishCell = savedData.finishCellPos;*/

        return level;
    }

    public void InstantiateLevel() {
        // Create a Level object that will hold all the level contents
        Transform rootParent = Instantiate(ObjectDatabase.instance._levelRoot, this.transform).transform;

        // For each cell create its parent cell object and then
        // instantiate its contents as children
        for (int z = 0; z < sizeZ; z++) {
            for (int x = 0; x < sizeX; x++) {
                // Create cell object
                MazeCellObject newCellObject = Instantiate(ObjectDatabase.instance._mazeCellObject, rootParent);
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
    [System.Serializable]
    public enum CellSubsections {
        NorthEdge, EastEdge, SouthEdge, WestEdge,
        NWCorner, NECorner, SECorner, SWCorner,
        NWInner, NInner, NEInner,
        WInner, Inner, EInner,
        SWInner, SInner, SEInner
    }
}
