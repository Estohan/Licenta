using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

    private Level level;
    private int sizeZ, sizeX;

    // !! This function will need some parameters
    // Uses MazeGenAlgorithms to generate a maze layout and
    // saves that layout into a two dimensional array of MazeCellData
    public Level GenerateLevel(LevelConfiguration levelConfiguration) {
        this.sizeZ = levelConfiguration.sizeZ;
        this.sizeX = levelConfiguration.sizeX;

        (int[,,] layout, LayoutStats stats) = 
            MazeGenAlgorithms.GenerateLayout(levelConfiguration);
        level = new Level(sizeZ, sizeX, layout, stats);

        SelectObstacles();
        AssignRoomsData(stats.rooms, levelConfiguration.nrOfSectors);
        AssignWallsAndFloors();
        ObstacleArchitectureDeletions();

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

    private void ObstacleArchitectureDeletions() {
        foreach((MazeCoords anchor, int shapeID, MazeDirection rotation, int obstacleID, int difficulty) in level.obstaclesList) {
            List<ObstacleObject.ObstObjDeletionEntry> deletionEntries = 
                   ObjectDatabase.instance.GetObstacle(level.stage, shapeID, obstacleID).getDeletionEntries(rotation);
            foreach(ObstacleObject.ObstObjDeletionEntry entry in deletionEntries) {
                // Debug.Log("Deletion:  " + anchor + ", " + shapeID + ", " + rotation + ", " + obstacleID + ", " + entry.offset + ", " + entry.subsection);
                level.cellsData[anchor.z + entry.offset.z, anchor.x + entry.offset.x].hasObjectReference[entry.subsection] = false;
            }
        }
    }

    private void SelectObstacles() {
        int stage = 1;
        int randIndex;
        int selectedDifficulty;
        int selectedObjectID;
        MazeDirection selectedRotation;
        List<ObstacleObject> obstaclesList;
        List<(int obstacleID, MazeDirection rotation, int difficulty)> viableObstacles = 
            new List<(int obstacleID, MazeDirection rotation, int difficulty)>();

        foreach((MazeCoords coords, int shapeID, MazeDirection rotation, int rec_diff) in level.stats.obstacles) {
            // Fetch from ObstacleDatabase a list of all obstacles of shape 'shapeID' in stage 'stage'
            obstaclesList = ObjectDatabase.instance.GetObstaclesOfShapeID(stage, shapeID);
            viableObstacles.Clear();
            if (obstaclesList != null) {
                // Select from the list above all obstacles that support the necessary rotation
                for(int i = 0; i < obstaclesList.Count; i ++) {
                    if (obstaclesList[i].GetObstacle(rotation, rec_diff) != null) {
                        viableObstacles.Add((i, rotation, rec_diff));
                    }
                }
                if(viableObstacles.Count == 0) {
                        /*Debug.LogError("No available obstacles for (" + stage + ", " +
                                                                        shapeID + ", " +
                                                                        rotation + ", " +
                                                                        rec_diff + ").");*/
                        return;
                }

                // Randomly pick one of them
                randIndex = UnityEngine.Random.Range(0, viableObstacles.Count - 1);
                   (selectedObjectID, selectedRotation, selectedDifficulty) = viableObstacles[randIndex];
                // [TODO] See if trap has floor/walls attached (bridge case)
                // level.cellsData[coords.z, coords.x].objectReferences[(int)CellSubsections.Inner] = (1, (int)ObjectType.Obstacle, )
                level.cellsData[coords.z, coords.x].obst_anchor = true;
                level.cellsData[coords.z, coords.x].obst_shapeID = shapeID;
                level.cellsData[coords.z, coords.x].obst_obstacleID = selectedObjectID; // The actual selection
                level.cellsData[coords.z, coords.x].obst_rotation = selectedRotation;
                level.cellsData[coords.z, coords.x].obst_difficulty = selectedDifficulty;
                level.obstaclesList.Add((coords, shapeID, selectedRotation, selectedObjectID, selectedDifficulty));
            }
        }
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
        int outerPaddingPoolSize = ObjectDatabase.instance.GetObjectsPoolSize(objStage, ObjectType.OuterPadding);
        int innerPaddingPoolSize = ObjectDatabase.instance.GetObjectsPoolSize(objStage, ObjectType.InnerPadding);
        int objIndex;
        for(int z = 0; z < sizeZ; z ++) {
            for(int x = 0; x < sizeX; x ++) {
                // [TODO] Insert some logic to decide between multiple objects
                // of the same type
                switch(level.cellsData[z, x].type) {
                    case CellType.OuterPadding:
                        // Floor
                        // Randomly choose an outer padding object
                        objIndex = UnityEngine.Random.Range(0, outerPaddingPoolSize);
                        level.cellsData[z, x].objectReferences[0] = (objStage, (int)ObjectType.OuterPadding, objIndex);
                        level.cellsData[z, x].hasObjectReference[0] = true;
                        break;
                    case CellType.InnerPadding:
                        // Floor
                        // Randomly choose an inner padding object
                        objIndex = UnityEngine.Random.Range(0, innerPaddingPoolSize);
                        level.cellsData[z, x].objectReferences[0] = (objStage, (int)ObjectType.InnerPadding, objIndex);
                        level.cellsData[z, x].hasObjectReference[0] = true;
                        break;
                    case CellType.Room:
                        RoomObjectCell roomObjCell = ObjectDatabase.instance.GetRoomCell(
                                                                        level.cellsData[z, x].roomObjStage,
                                                                        level.cellsData[z, x].room,
                                                                        level.cellsData[z, x].offsetToRoomAnchor);
                        if (roomObjCell.HasFloor()) {
                            level.cellsData[z, x].hasObjectReference[0] = true;
                        }
                        for(int i = 0; i < 5; i ++) {
                            if (roomObjCell.HasWall((MazeDirection)i)) {
                                level.cellsData[z, x].hasObjectReference[i+1] = true;
                            }
                            if (roomObjCell.HasCorner(i)) {
                                level.cellsData[z, x].hasObjectReference[i + 5] = true;
                            }
                        }
                        break;
                    case CellType.Start:
                    case CellType.Finish:
                    case CellType.Obstacle:
                    //case CellType.Loot:
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
                // Move cell to correct position
                // this.cellSize = this.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.x;

                //newCellObject.transform.parent = transform;
                newCellObject.transform.localPosition =
                    new Vector3(newCellObject.data.coordinates.x * Constants.cellSize - Constants.cellSize / 2,
                                0f,
                                -newCellObject.data.coordinates.z * Constants.cellSize + Constants.cellSize / 2);

                // Save newly created cell
                level.cellsObjects[z, x] = newCellObject;

                // Instantiate floors and move cells to their correct positions
                newCellObject.InstantiateFloor();

                // Instantiate walls
                newCellObject.InstantiateWalls();

                // Instantiate corners
                newCellObject.InstantiateCorners();

                // Instantiate cell contents
                newCellObject.InstantiateContent();

                // Gather and hide all map icons present in this cell's hierarchy
                newCellObject.CollectAndHideMapIcons();

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
        Floor, // 0
        NorthEdge, EastEdge, SouthEdge, WestEdge,
        NWCorner, NECorner, SECorner, SWCorner,
        NWInner, NInner, NEInner,
        WInner, Inner, EInner,
        SWInner, SInner, SEInner
    }
}
