using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class MazeGenAlgorithms {

    // the layout returned is a three dimensional array of
    // x = sizeX
    // y = sizeZ
    // z = 6 (0 - North wall, 1 - East wall, 2 - South wall, 3 - West wall,
    //          4 - cell type, 5 - cell sector)
    // Types: 0 - outer padding, 1 - inner padding, 2 - start cell,
    //        3 - finish cell, 4 - common cell, 5 - special room cell.

    public static (int[,,], LayoutStats stats)
        GenerateLayout(int sizeZ, int sizeX, int outerPaddingDiv, int innerPaddingDiv, int nrOfSectors) {

        int[,,] layout = new int[sizeZ, sizeX, 6];
        LayoutStats stats = new LayoutStats(sizeZ, sizeX, nrOfSectors);
        MazeCoords startCell;
        MazeCoords finishCell;

        // 6(OPl) + 3(IPl) + 12(M) + 3(IPr) + 6(OPr) = 12 + 6 + 12 = 30
        int outerPaddingValZ = (int)(sizeZ / outerPaddingDiv);
        int outerPaddingValX = (int)(sizeX / outerPaddingDiv);
        int innerPaddingValZ = (int)(sizeZ / innerPaddingDiv);
        int innerPaddingValX = (int)(sizeX / innerPaddingDiv);
        int remainingCellsZ = sizeZ - outerPaddingValZ * 2 - innerPaddingValZ * 2; // DO I NEED THIS?
        int remainingCellsX = sizeX - outerPaddingValX * 2 - innerPaddingValX * 2; // DO I NEED THIS?
        int totalInnerPadding = ((sizeZ - outerPaddingValZ * 2) * (sizeX - outerPaddingValX * 2)) - 
                                (remainingCellsZ * remainingCellsX); // DO I NEED THIS?
        stats.InitializePadding(outerPaddingValZ, outerPaddingValX, innerPaddingValZ, innerPaddingValX);

        // visisted array initialization
        int[,] visited = new int[sizeZ, sizeX];
        for(int z = 0; z < sizeZ; z ++) {
            for(int x = 0; x < sizeX; x ++) {
                visited[z, x] = 0;
            }
        }

        // layout initialization
        /*for(int z = 0; z < sizeZ; z ++) {
            for(int x = 0; x < sizeX; x ++) {
                for(int k = 0; k < 5; k ++) {
                    layout[z, x, k] = 0;
                }
            }
        }*/
        int[,,] testLayout = { 
            { { 1, 1, 1, 1, 1}, 
              { 0, 0, 1, 0, 1}, 
              { 1, 1, 0, 0, 0}, 
              { 0, 0, 0, 0, 0}, 
              { 1, 1, 0, 0, 1} },

            { { 0, 0, 1, 0, 1},
              { 0, 1, 0, 1, 1},
              { 0, 0, 0, 1, 1},
              { 1, 0, 0, 1, 1},
              { 1, 1, 0, 0, 1} },

            { { 0, 0, 1, 0, 1},
              { 1, 1, 0, 0, 0},
              { 0, 0, 0, 0, 0},
              { 1, 0, 0, 0, 1},
              { 1, 1, 1, 1, 1} },

            { { 1, 0, 0, 1, 0},
              { 1, 0, 1, 0, 1},
              { 1, 0, 0, 0, 1},
              { 1, 0, 0, 0, 1},
              { 1, 1, 1, 0, 0} }, 

            { { 1, 0, 0, 0, 0},
              { 0, 0, 0, 0, 0},
              { 0, 0, 0, 0, 0},
              { 0, 0, 0, 0, 0},
              { 0, 0, 0, 0, 0} },

            { { 0, 0, 0, 0, 0},
              { 0, 0, 0, 0, 0},
              { 0, 0, 0, 0, 0},
              { 0, 0, 0, 0, 0},
              { 0, 0, 0, 0, 0} },};

        // Create outer padding
        CreateOuterPadding(layout, stats, visited);

        // Create inner padding
        CreateInnerPadding(layout, stats, visited);

        // Choose start cell and end cell
        (startCell, finishCell) = ChooseStartAndFinish(layout, stats);

        // Split remaining space into sectors
        DivideIntoSectors(layout, stats);

        // Add rooms 
        AddSpecialSections(layout, stats);

        // Fill with corridors (maze)
        MazeFill_GrowingTreeAlg(layout, stats);

        // DEBUG
        _RemoveRoomWalls(layout, stats);

        // Analyze layout
        AnalyzeLayout(layout, stats);

        Debug.Log("Here are some stats:");
        Debug.Log("Cells total: " + stats.sizeZ * stats.sizeX + " = " + stats.totalOuterPadding + " + " + stats.totalInnerPadding + " + " + stats.totalCore + " + " +
          (stats.totalOuterPadding + stats.totalInnerPadding + stats.totalCore));
        String message = "Sectors data: \n";
        for(int sector = 1; sector <= nrOfSectors; sector ++) {
            message += "Sector " + sector + ": ";
            message += stats.sectorCells[sector - 1].Count + " cells, of which ";
            message += stats.sectorBorder[sector - 1].Count + " are on border. \n";
            message += "\tExits on:";
            foreach((MazeCoords cell, MazeDirection dir) in stats.nextSectorExit[sector - 1]) {
                message += "[" + cell + " " + dir + "] ";
            }
            message += "\n";
            message += "\tPassages on:";
            foreach ((MazeCoords cell, MazeDirection dir) in stats.passages[sector - 1]) {
                message += "[" + cell + " " + dir + "] ";
                // layout[cell.z, cell.x, 5] = 6;
            }
            message += "\n";
            // COLOR STUFF (pre-sec/type)
            /*foreach(MazeCoords cell in stats.sectorBorder[sector - 1]) {
                DEBUGG_type = (layout[cell.z, cell.x, 4] / 10) * 10;
                if (sector == 2 || sector == 4) {
                    layout[cell.z, cell.x, 4] = DEBUGG_type + 6;
                }
            }*/
        }
        Debug.Log(message);

        message = "Room data:\n";
        for(int sector = 1; sector < nrOfSectors; sector ++) {
            message += "Sector " + sector + ":\n";
            foreach(RoomData room in stats.rooms[sector - 1]) {
                message += "\t Room " + room.index + " of size " + room.size + " and rot. " + room.rotation + ", at " + room.anchor + "\n";
            }
        }
        Debug.Log(message);


        return (layout, stats);
    }

    private static void AnalyzeLayout(int[,,] layout, LayoutStats stats) {
        // main solution
        // partial solutions? (sector solutions)
        // dead ends
        MazeCoords startCell = stats.startCell;
        MazeCoords finishCell = stats.finishCell;
        MazeCoords currentCell;
        Queue<MazeCoords> bfsQueue = new Queue<MazeCoords>();
        List<MazeCoords> neighbours;
        bool[,] visited = new bool[stats.sizeZ, stats.sizeX];

        // Calculate distance to start cell
        bfsQueue.Enqueue(startCell);
        stats.cellsStats[startCell.z, startCell.x].distanceToStart = 0;
        while (bfsQueue.Count > 0) {
            currentCell = bfsQueue.Dequeue();
            visited[currentCell.z, currentCell.x] = true;
            neighbours = GetAccessibleNeighbours_Unsafe(currentCell, layout);
            foreach(MazeCoords neighbour in neighbours) {
                if(!visited[neighbour.z, neighbour.x]) {
                    bfsQueue.Enqueue(neighbour);
                    stats.cellsStats[neighbour.z, neighbour.x].distanceToStart = 
                        stats.cellsStats[currentCell.z, currentCell.x].distanceToStart + 1;
                }
            }
        }

        // Reinitialize array "visited" for a new BFS
        for(int z = 0; z < stats.sizeZ; z ++) {
            for(int x = 0; x < stats.sizeX; x ++) {
                visited[z, x] = false;
            }
        }

        // Calculate distance to finish cell (and find solution)
        int distanceToStartCounter = stats.cellsStats[finishCell.z, finishCell.x].distanceToStart + 1;
        bfsQueue.Enqueue(finishCell);
        stats.cellsStats[finishCell.z, finishCell.x].distanceToEnd = 0;
        while (bfsQueue.Count > 0) {
            currentCell = bfsQueue.Dequeue();
            visited[currentCell.z, currentCell.x] = true;
            neighbours = GetAccessibleNeighbours_Unsafe(currentCell, layout);
            foreach (MazeCoords neighbour in neighbours) {
                if (!visited[neighbour.z, neighbour.x]) {
                    bfsQueue.Enqueue(neighbour);
                    stats.cellsStats[neighbour.z, neighbour.x].distanceToEnd =
                        stats.cellsStats[currentCell.z, currentCell.x].distanceToEnd + 1;
                }
            }
            // also check if current cell is part of the solution path
            if(stats.cellsStats[currentCell.z, currentCell.x].distanceToStart == distanceToStartCounter - 1) {
                stats.cellsStats[currentCell.z, currentCell.x].isInSolution = true;
                distanceToStartCounter--;
            }
        }
    }

    private static void _RemoveRoomWalls(int[,,] layout, LayoutStats stats) {
        List<(int, int)> roomLayoutRotation;
        List<MazeCoords> roomCells = new List<MazeCoords>();
        MazeCoords aux;
        bool oneOfUs;
        for (int sector = 1; sector <= stats.numberOfSectors; sector++) {
            foreach (RoomData room in stats.rooms[sector - 1]) {
                roomLayoutRotation = RoomLayouts.rooms[room.size - 1][room.index].GetRotation(room.rotation);
                roomCells.Clear();
                foreach ((int z, int x) in roomLayoutRotation) {
                    aux = room.anchor + (z, x);
                    roomCells.Add(aux);
                }

                foreach (MazeCoords cell in roomCells) {
                    oneOfUs = false;
                    aux = cell + MazeDirection.North.ToMazeCoords();
                    foreach (MazeCoords cell2 in roomCells) {
                        if (aux.z == cell2.z && aux.x == cell2.x) { 
                            oneOfUs = true; 
                            break; 
                        } 
                    }
                    if (oneOfUs) layout[cell.z, cell.x, (int)MazeDirection.North] = 0;

                    oneOfUs = false;
                    aux = cell + MazeDirection.East.ToMazeCoords();
                    foreach (MazeCoords cell2 in roomCells) { 
                        if (aux.z == cell2.z && aux.x == cell2.x) { 
                            oneOfUs = true; 
                            break; 
                        } 
                    }
                    if (oneOfUs) layout[cell.z, cell.x, (int)MazeDirection.East] = 0;

                    oneOfUs = false;
                    aux = cell + MazeDirection.South.ToMazeCoords();
                    foreach (MazeCoords cell2 in roomCells) { 
                        if (aux.z == cell2.z && aux.x == cell2.x) { 
                            oneOfUs = true; 
                            break; 
                        } 
                    }
                    if (oneOfUs) layout[cell.z, cell.x, (int)MazeDirection.South] = 0;

                    oneOfUs = false;
                    aux = cell + MazeDirection.West.ToMazeCoords();
                    foreach (MazeCoords cell2 in roomCells) { 
                        if (aux.z == cell2.z && aux.x == cell2.x) { 
                            oneOfUs = true; 
                            break; 
                        } 
                    }
                    if (oneOfUs) layout[cell.z, cell.x, (int)MazeDirection.West] = 0;
                }
            }
        }
    }

    private static void AddSpecialSections(int[,,] layout, LayoutStats stats) {
        // Step 1: Decide max number and complexity of sections/rooms per sector
        // [DEBUG][TRY] A section shouldn't represent more than 10% of its sector
        // [DEBUG][TRY] All sections in a sector shouldn't represent more than 20% of the sector

        // Sector specific section size constraints
        int[] initialMaxSectionSize = new int[stats.numberOfSectors];
        int[] initialMaxSectionTotal = new int[stats.numberOfSectors];
        int percentOfSectorSize;
        for(int sector = 1; sector <= stats.numberOfSectors; sector++) {
            percentOfSectorSize = stats.sectorCells[sector - 1].Count / 10; // 10% of sector size
            // [TRY] Insert some randomness here
            initialMaxSectionSize[sector - 1] = percentOfSectorSize; // MAX SIZE OF THE ROOMS
            initialMaxSectionTotal[sector - 1] = percentOfSectorSize * 2;
            // If there are no rooms this large, the limit will be the largest existing type of room
            if(percentOfSectorSize >= RoomLayouts.rooms.Length) {
                initialMaxSectionSize[sector - 1] = RoomLayouts.rooms.Length;
            }
        }

        // Step 2: Place sections/rooms
        bool largestOnePlaced;
        bool roomPlaced;
        int currentMaxSectionSize;
        int currentMaxSectionTotal;
        int roomSize, roomIndex;
        int triesLeftAnchor;
        int triesLeftRoom;
        MazeCoords anchor;
        MazeCoords aux;
        MazeDirection rotation;
        List<(int, int)>[] room;

        for(int sector = 1; sector <= stats.numberOfSectors; sector++) {
            // Initialize placed rooms list
            largestOnePlaced = false;
            currentMaxSectionSize = initialMaxSectionSize[sector - 1];
            currentMaxSectionTotal = initialMaxSectionTotal[sector - 1];
            // [DEBUG] Debug.Log("Placing rooms in sector " + sector + ", currMSS = " + currentMaxSectionSize + ", currMST = " + currentMaxSectionTotal);
            triesLeftRoom = 100;
            // While there is space for more rooms
            while(currentMaxSectionTotal > 0 && triesLeftRoom > 0) {
                // If the current maximum room size is bigger than the 
                // current unallocated room space
                // Ex.: can't get a size 5 room anymore if I only have 4 cells of space left
                if(currentMaxSectionTotal < currentMaxSectionSize) {
                    currentMaxSectionSize = currentMaxSectionTotal;
                }
                // Choose a room
                // [DEBUG] Debug.Log("[OutOfBoundException]: " + currentMaxSectionSize);
                if (largestOnePlaced) {
                    (room, roomSize, roomIndex) = RoomLayouts.GetRandomRoomOfMaxSizeN(currentMaxSectionSize);
                } else {
                    (room, roomSize, roomIndex) = RoomLayouts.GetRandomRoomOfSizeN(currentMaxSectionSize);
                    largestOnePlaced = true;
                }
                // [DEBUG] Debug.Log("\tTrying to place room " + roomIndex + " of size " + roomSize);
                // Select a random cell from the current sector as anchor and check if the room can
                // be placed there. If not, choose another cell, and so on.
                roomPlaced = false;
                triesLeftAnchor = stats.sectorCells[sector - 1].Count;
                while(!roomPlaced && triesLeftAnchor > 0) {
                    // Choose anchor - [OPTIMIZATION]: Avoid picking the same anchor twice
                    anchor = stats.sectorCells[sector - 1][UnityEngine.Random.Range(0, stats.sectorCells[sector - 1].Count)];
                    // [DEBUG] Debug.Log("\t\tTrying to place at " + anchor);
                    // Test rotations placement
                    (roomPlaced, rotation, _) = CheckRoomPlacement(layout, anchor, sector, room, stats);
                    if(roomPlaced) {
                        // [DEBUG] Debug.Log("\t\t\tPlacement of rotation " + rotation);
                        // Place room on layout
                        foreach ((int z, int x) in room[(int)rotation]) {
                            aux = anchor + (z, x);
                            layout[aux.z, aux.x, 4] = (int)CellType.Room;
                            // North wall
                            layout[aux.z, aux.x, 0] = 1;
                            // East wall
                            layout[aux.z, aux.x, 1] = 1;
                            // South wall
                            layout[aux.z, aux.x, 2] = 1;
                            // West wall
                            layout[aux.z, aux.x, 3] = 1;
                        }
                        // Access to room - remove entrance wall
                        layout[anchor.z, anchor.x, (int)rotation] = 0;
                        // Store room's data
                        stats.rooms[sector - 1].Add(new RoomData(anchor, rotation, roomSize, roomIndex));
                        stats._accessibleCellsCount[sector - 1] -= roomSize - 1;
                        // Subtract the room's size from the sector's allocated space for rooms
                        currentMaxSectionTotal -= roomSize;
                    } else {
                        triesLeftAnchor--;
                    }
                }
                triesLeftRoom--;
            }
        }
    }
    // Checks if any of the room rotations of the "room" parameter can be safely placed on the layout.
    // [DEBUG] : err
    // err = 0: Success
    // err = 1: All rotation overlap non-common type cells
    // err = 2: Room cannot be accessed
    // err = 3: Room blocks another room, sector exit or sector section
    private static (bool roomPlaced, MazeDirection rotation, int err) 
        CheckRoomPlacement(int[,,] layout, MazeCoords anchor, int sector, List<(int, int)>[] room, LayoutStats stats) {
        bool roomPlaced = false;
        int rotationIndex;
        int cellSector;
        int cellType;
        int nrOfRotations = room.Count();
        int currNrOfRotations = nrOfRotations;
        MazeCoords aux;
        List<int> uncheckedRotations = new List<int>();
        int uncheckedRotationIndex;
        List<(int, int)> roomRotation;

        // Check if the given anchor is on top of a room cell.
        // If so, there is no point in doing anything else - the room cannot be
        // placed here.
        if(layout[anchor.z, anchor.x, 4] == (int)CellType.Room) {
            roomPlaced = false;
            // [DEBUG] Debug.Log("\t\tFAIL! Anchor is part of another room.");
            return (roomPlaced, MazeDirection.North, 1);
        }

        // uncheckedRotations contains the indexes of the rotations left
        // unchecked in room
        for(int i = 0; i < nrOfRotations; i ++) {
            uncheckedRotations.Add(i);
        }

        // Randomly select rotations from the list instead of simply interatig through
        // them to avoid the first rotations being "chosen" too frequently
        while(currNrOfRotations > 0) {
            uncheckedRotationIndex = UnityEngine.Random.Range(0, uncheckedRotations.Count);
            rotationIndex = uncheckedRotations[uncheckedRotationIndex];
            uncheckedRotations.RemoveAt(uncheckedRotationIndex);

            roomRotation = room[rotationIndex];
            roomPlaced = true;
            // Check overlapping for each offset in rotation
            foreach((int z, int x) in roomRotation) {
                aux = anchor + (z, x); // room cell with offset (z, x)
                cellSector = layout[aux.z, aux.x, 5];
                cellType = layout[aux.z, aux.x, 4];
                if(cellSector != sector || cellType != (int)CellType.Common) {
                    roomPlaced = false;
                    // [DEBUG] Debug.Log("\t\tInvalid rotation on " + aux);
                    break;
                }
            }
            currNrOfRotations--;
            // If the room does not overlap with anything:
            if(roomPlaced) {
                // a) Check if it can be accessed
                aux = anchor + ((MazeDirection)rotationIndex).ToMazeCoords();
                if(layout[aux.z, aux.x, 4] != (int)CellType.Common || layout[aux.z, aux.x, 5] != sector) {
                    roomPlaced = false;
                    // [DEBUG] Debug.Log("\t\tFAIL! Can't be accessed!");
                    return (roomPlaced, (MazeDirection)rotationIndex, 2);
                }

                // b) Check if it blocks any sector exit
                // temporary layout that contains the cell types
                int[,] tempLayout = new int[stats.sizeZ, stats.sizeX];
                for (int z = 0; z < stats.sizeZ; z++) {
                    for (int x = 0; x < stats.sizeX; x++) {
                        tempLayout[z, x] = layout[z, x, 4];
                    }
                }
                // Place room on temporary layout
                foreach ((int z, int x) in roomRotation) {
                    aux = anchor + (z, x); // room cell with offset (z, x)
                    tempLayout[aux.z, aux.x] = (int)CellType.Room;
                }
                foreach((MazeCoords passage, MazeDirection dir) in stats.passages[sector - 1]) {
                    if(tempLayout[passage.z, passage.x] == (int)CellType.Room) {
                        roomPlaced = false;
                        // [DEBUG] Debug.Log("\t\tFAIL! Blocks sector exit!");
                        return (roomPlaced, (MazeDirection)rotationIndex, 3);
                    }
                }

                // c) Check if it blocks another room's entrance or a sector section
                int currAccessibleCellsCount = 0;
                int roomInaccessibleCells = roomRotation.Count;
                MazeCoords startCell;
                Queue<MazeCoords> bfsQueue = new Queue<MazeCoords>();
                bool[,] visited = new bool[stats.sizeZ, stats.sizeX];
                // Mark room's cells as visited
                foreach((int z, int x) in roomRotation) {
                    visited[anchor.z + z, anchor.x + x] = true;
                }
                // Start from the cell that is directly in front of the room's entrance a BFS for all explorable
                // cells. If the final count is less than stats._accessibleCellsCount then it means there is one
                // or more cells blocked by this placement
                startCell = anchor + ((MazeDirection)rotationIndex).ToMazeCoords();

                bfsQueue.Enqueue(startCell);
                visited[startCell.z, startCell.x] = true;
                while (bfsQueue.Count > 0) {
                    aux = bfsQueue.Dequeue();
                    currAccessibleCellsCount++;
                    // For each neighbour there are 5 conditions to be satisfied before adding them to the queue:
                    // - the neighbour is not visited
                    // - the neighbour is in this sector
                    // - the neighbour is not padding  
                    // - neighbour has no wall in this direction
                    // - this cell has no wall in the neighbour's direction
                    // North neighbour
                    if (!visited[aux.z - 1, aux.x] &&
                        layout[aux.z - 1, aux.x, 5] == sector && // their sector
                        //tempLayout[aux.z - 1, aux.x] > (int)CellType.InnerPadding && // their type
                        layout[aux.z - 1, aux.x, (int)MazeDirection.South] == 0 && // their wall
                        layout[aux.z, aux.x, (int)MazeDirection.North] == 0) { // my wall
                        bfsQueue.Enqueue(aux + (-1, 0));
                        visited[aux.z - 1, aux.x] = true;
                    }
                    // East neighbour
                    if (!visited[aux.z, aux.x + 1] &&
                        layout[aux.z, aux.x + 1, 5] == sector && // their sector
                        //tempLayout[aux.z, aux.x + 1] > (int)CellType.InnerPadding && // their type
                        layout[aux.z, aux.x + 1, (int)MazeDirection.West] == 0 && // their wall
                        layout[aux.z, aux.x, (int)MazeDirection.East] == 0) { // my wall
                        bfsQueue.Enqueue(aux + (0, 1));
                        visited[aux.z, aux.x + 1] = true;
                    }
                    // South neighbour
                    if (!visited[aux.z + 1, aux.x] &&
                        layout[aux.z + 1, aux.x, 5] == sector && // their sector
                        //tempLayout[aux.z + 1, aux.x] > (int)CellType.InnerPadding && // their type
                        layout[aux.z + 1, aux.x, (int)MazeDirection.North] == 0 && // their wall
                        layout[aux.z, aux.x, (int)MazeDirection.South] == 0) { // my wall
                        bfsQueue.Enqueue(aux + (1, 0));
                        visited[aux.z + 1, aux.x] = true;
                    }
                    // West neighbour
                    if (!visited[aux.z, aux.x - 1] &&
                        layout[aux.z, aux.x - 1, 5] == sector && // their sector
                        //tempLayout[aux.z, aux.x - 1] > (int)CellType.InnerPadding && // their type
                        layout[aux.z, aux.x - 1, (int)MazeDirection.East] == 0 && // their wall
                        layout[aux.z, aux.x, (int)MazeDirection.West] == 0) { // my wall
                        bfsQueue.Enqueue(aux + (0, -1));
                        visited[aux.z, aux.x - 1] = true;
                    }
                }
                // See if the search reached all the accessible cells.
                if (currAccessibleCellsCount != stats._accessibleCellsCount[sector - 1] - roomInaccessibleCells) {
                    roomPlaced = false;
                    // [DEBUG] Debug.Log("\t\tFAIL! Blocks something! " + currAccessibleCellsCount + "==" + stats._accessibleCellsCount[sector - 1] +
                    // [DEBUG] " (Sec. total of " + stats.sectorCells[sector - 1].Count + ")");
                    return (roomPlaced, (MazeDirection)rotationIndex, 3);
                } else {
                    // [DEBUG] Debug.Log("\t\tSUCCESS! Not blocking. " + anchor + " " + currAccessibleCellsCount + "==" + stats._accessibleCellsCount[sector - 1] +
                    // [DEBUG] " (Sec. total of " + stats.sectorCells[sector - 1].Count + ")");
                }
                // Successful placement
                return (roomPlaced, (MazeDirection)rotationIndex, 0);
            }
        }
        // Unable to place room
        return (roomPlaced, MazeDirection.South, 1);
    }

    private static void DivideIntoSectors(int[,,] layout, LayoutStats stats) {
        // Step 1: Place seeds
        List<MazeCoords> sectorSeeds = new List<MazeCoords>();
        MazeCoords center = new MazeCoords(stats.sizeZ / 2, stats.sizeX / 2);

        // Only allow 1, 2, 3, 4 or 5 sectors
        if(stats.numberOfSectors < 1 || stats.numberOfSectors > 5) {
            stats.numberOfSectors = 1;
        }

        switch(stats.numberOfSectors) {
            case 2:
                //  1..
                //  .x.
                //  ..2
                sectorSeeds.Add(new MazeCoords(center.z - 1, center.x - 1));
                sectorSeeds.Add(new MazeCoords(center.z + 1, center.x + 1));
                break;
            case 3:
                //  ..1..
                //  .....
                //  ..x..
                //  .....
                //  3...2
                sectorSeeds.Add(new MazeCoords(center.z - 2, center.x));
                sectorSeeds.Add(new MazeCoords(center.z + 2, center.x + 2));
                sectorSeeds.Add(new MazeCoords(center.z + 2, center.x - 2));
                break;
            case 4:
                //  1.2
                //  .x.
                //  4.3
                sectorSeeds.Add(new MazeCoords(center.z - 1, center.x - 1));
                sectorSeeds.Add(new MazeCoords(center.z - 1, center.x + 1));
                sectorSeeds.Add(new MazeCoords(center.z + 1, center.x + 1));
                sectorSeeds.Add(new MazeCoords(center.z + 1, center.x - 1));
                break;
            case 5:
                //  ..1..
                //  .....
                //  5.x.2
                //  .....
                //  .4.3.
                sectorSeeds.Add(new MazeCoords(center.z - 2, center.x));
                sectorSeeds.Add(new MazeCoords(center.z, center.x + 2));
                sectorSeeds.Add(new MazeCoords(center.z + 2, center.x + 1));
                sectorSeeds.Add(new MazeCoords(center.z + 2, center.x - 1));
                sectorSeeds.Add(new MazeCoords(center.z, center.x - 2));
                break;
            default:
                //  ...
                //  .1.
                //  ...
                sectorSeeds.Add(new MazeCoords(center.z, center.x));
                break;
        }

        // Step 2: Start in parallel from each seed a sector that is spreading in a BFS fashion.
        // This ends when each non-padding cell belongs to a sector.
        MazeCoords cellCandidate;
        List<Queue<MazeCoords>> bfsQueues = new List<Queue<MazeCoords>>();
        List<MazeCoords> neighbours;
        bool[,] visited = new bool[stats.sizeZ, stats.sizeX];
        bool noSeedsLeft = false;
        bool noCellAssignedYet = true;

        // Initialize visited array
        for (int z = 0; z < stats.sizeZ; z++) {
            for (int x = 0; x < stats.sizeX; x++) {
                visited[z, x] = false;
            }
        }

        // Initialize queues
        for(int sector = 1; sector <= stats.numberOfSectors; sector ++) {
            bfsQueues.Add(new Queue<MazeCoords>()); //  Create new queue
            bfsQueues[sector - 1].Enqueue(sectorSeeds[sector - 1]); // Put inside the corresponding seed
        }

        // Assign sectors to cells
        while(!noSeedsLeft) {
            // For each sector
            for(int sector = 1; sector <= stats.numberOfSectors; sector ++) {
                noCellAssignedYet = true;
                // Assign a cell (if possible)
                while(noCellAssignedYet && bfsQueues[sector - 1].Count > 0) {
                    cellCandidate = bfsQueues[sector - 1].Dequeue();
                    // If the candidate is not visited and is not padding
                    if(!visited[cellCandidate.z, cellCandidate.x] &&
                        layout[cellCandidate.z, cellCandidate.x, 4] >= (int)CellType.Start) {
                        // Claim this cell
                        layout[cellCandidate.z, cellCandidate.x, 5] = sector;
                        visited[cellCandidate.z, cellCandidate.x] = true;
                        noCellAssignedYet = false;
                        // Record its assignation into stats
                        stats.sectorCells[sector - 1].Add(cellCandidate);
                        stats._accessibleCellsCount[sector - 1]++;
                        // Transform its neighbours into candidates for this sector
                        neighbours = GetNeighboursOfCell_Unsafe(cellCandidate, false, true);
                        foreach(MazeCoords neigh in neighbours) {
                            bfsQueues[sector - 1].Enqueue(neigh);
                        }
                    }
                }
            }

            // See if there are any seeds left
            noSeedsLeft = true;
            foreach(Queue<MazeCoords> queue in bfsQueues) {
                if(queue.Count > 0) {
                    noSeedsLeft = false;
                    break;
                }
            }
        }

        // Step 3: Create walls between sectors
        MazeCoords neighbour, thisCell;
        int thisCellSector, neighbourSector;
        // Initialize unexplored edges list and borders list
        List<MazeDirection>[,] unexplored = new List<MazeDirection>[stats.sizeZ, stats.sizeX];
        List<(MazeCoords, MazeDirection)>[] borderWithNextSector = new List<(MazeCoords, MazeDirection)>[stats.numberOfSectors];
        // Initialize visited array again, and use it to ensure a cell
        // is added to a border in "stats" only once
        for (int z = 0; z < stats.sizeZ; z++) {
            for (int x = 0; x < stats.sizeX; x++) {
                visited[z, x] = false;
            }
        }

        for (int z = stats.outerPaddingValZ; z < stats.sizeZ - stats.outerPaddingValZ; z++) {
            for (int x = stats.outerPaddingValX; x < stats.sizeX - stats.outerPaddingValX; x++) {
                // If cell is not padding
                if(layout[z, x, 4] >= (int)CellType.Start) {
                    unexplored[z, x] = new List<MazeDirection>();
                    unexplored[z, x].Add(MazeDirection.North);
                    unexplored[z, x].Add(MazeDirection.East);
                    unexplored[z, x].Add(MazeDirection.South);
                    unexplored[z, x].Add(MazeDirection.West);
                }
            }
        }
        for(int i = 0; i < stats.numberOfSectors; i ++) {
            borderWithNextSector[i] = new List<(MazeCoords, MazeDirection)>();
        }

        // Surround sectors with walls
        for (int z = stats.outerPaddingValZ; z < stats.sizeZ - stats.outerPaddingValZ; z++) {
            for (int x = stats.outerPaddingValX; x < stats.sizeX - stats.outerPaddingValX; x++) {
                // If cell is not padding
                if (layout[z, x, 4] >= (int)CellType.Start) {
                    thisCell = new MazeCoords(z, x);
                    thisCellSector = layout[thisCell.z, thisCell.x, 5];
                    foreach(MazeDirection direction in unexplored[z, x]) {
                        neighbour = thisCell + direction.ToMazeCoords();
                        neighbourSector = layout[neighbour.z, neighbour.x, 5];
                        // If thisCell and its neighbour belong to different sectors then put a wall
                        // between thisCell and its neighbour... in this cell
                        if (thisCellSector != neighbourSector) {
                            // Debug.Log("Found border betweeen " + thisCell + " and " + neighbour + " (" + thisCellSector + " and " + neighbourSector + ")");
                            layout[thisCell.z, thisCell.x, (int)direction] = 1;
                            // ... and in the neighbour's cell
                            if(layout[neighbour.z, neighbour.x, 4] >= (int)CellType.Start) {
                                layout[neighbour.z, neighbour.x, (int)direction.GetOppositeDirection()] = 1;
                                unexplored[neighbour.z, neighbour.x].Remove(direction.GetOppositeDirection());
                                // Record the border of the neighbour into stats
                                if (!visited[neighbour.z, neighbour.x])  {
                                    stats.sectorBorder[neighbourSector - 1].Add(neighbour);
                                    visited[neighbour.z, neighbour.x] = true;
                                }
                            }
                            // Record this border into stats
                            if(!visited[thisCell.z, thisCell.x]) {
                                stats.sectorBorder[thisCellSector - 1].Add(thisCell);
                                visited[thisCell.z, thisCell.x] = true;
                            }
                            // If one cell is in sector n and the other in sector n+1, save the edge for
                            // later, when passages will be chosen
                            if(thisCellSector == neighbourSector - 1) {
                                // Debug.Log("Added border between " + thisCellSector + " and " + neighbourSector);
                                borderWithNextSector[thisCellSector - 1].Add((thisCell, direction));
                            }
                            if(thisCellSector == neighbourSector + 1 && neighbourSector > 0) {
                                // Debug.Log("Added border between " + thisCellSector + " and " + neighbourSector);
                                borderWithNextSector[neighbourSector - 1].Add((neighbour, direction.GetOppositeDirection()));
                            }
                        }
                    }
                }
            }
        }

        // Step 4: Choose passages between sectors (1-2, 2-3, etc.)
        int randomIndex;
        MazeDirection passageDirection;
        MazeCoords passageCell;
        List<(MazeCoords, MazeDirection)> auxPassages = new List<(MazeCoords, MazeDirection)>();
        for (int sector = 1; sector <= stats.numberOfSectors; sector++) {
            // Randomly choose a cell on this sector's border with the next sector
            randomIndex = UnityEngine.Random.Range(0, borderWithNextSector[sector - 1].Count);
            if (borderWithNextSector[sector - 1].Count > 0) {
                (passageCell, passageDirection) = borderWithNextSector[sector - 1][randomIndex];
                // Used when finding the player's route through the passages
                auxPassages.Add((passageCell, passageDirection));
                auxPassages.Add((passageCell + passageDirection.ToMazeCoords(), passageDirection.GetOppositeDirection()));
                // Store passage location
                stats.passages[sector - 1].Add((passageCell, passageDirection));
                stats.passages[sector].Add((passageCell + passageDirection.ToMazeCoords(), passageDirection.GetOppositeDirection()));
                // Remove the wall with the next sector
                neighbour = passageCell + passageDirection.ToMazeCoords();
                layout[passageCell.z, passageCell.x, (int)passageDirection] = 0;
                layout[neighbour.z, neighbour.x, (int)passageDirection.GetOppositeDirection()] = 0;
            }
        }

        // DEBUG
        /*String message = "";
        foreach((MazeCoords celll, MazeDirection dirr) in passages) {
            message += "[" + celll + " " + dirr + "]\n";
        }
        Debug.Log("---" + message);*/

        // Save the passages in the order the player will visit them
        MazeCoords cell;
        MazeDirection dir;
        int currentSector = layout[stats.startCell.z, stats.startCell.x, 5];
        int toBeRemovedIndex;
        bool sectorDone = false; // All its exits are discovered
        bool passageDiscovered;
        Queue<int> nextSector = new Queue<int>();

        nextSector.Enqueue(currentSector);
        while(auxPassages.Count > 0) {
            // If all exits are discovered for this sector
            //Debug.Log("Sector " + currentSector + " " + nextSector.Count + " " + passages.Count);
            if (sectorDone) {
                currentSector = nextSector.Dequeue();
                sectorDone = false;
            }
            // Look for passages in this sector
            passageDiscovered = false;
            toBeRemovedIndex = -1;
            for(int i = 0; i < auxPassages.Count; i++) {
                cell = auxPassages[i].Item1;
                dir = auxPassages[i].Item2;
                if(layout[cell.z, cell.x, 5] == currentSector) {
                    //Debug.Log("Found passage: ");
                    //Debug.Log("[" + cell + " " + dir + " " + layout[(cell + dir.ToMazeCoords()).z, (cell + dir.ToMazeCoords()).x, 4] % 10 + "]");
                    stats.nextSectorExit[currentSector - 1].Add(auxPassages[i]);
                    nextSector.Enqueue(layout[(cell + dir.ToMazeCoords()).z, (cell + dir.ToMazeCoords()).x, 5]);
                    toBeRemovedIndex = i;
                    passageDiscovered = true;
                    break;
                }
            }
            // DEBUG
            //message = "";
            // Remove the passage found from list passages
            if(toBeRemovedIndex > -1) {
                //message += "Removing: ";
               // message += passages[toBeRemovedIndex];
                auxPassages.RemoveAt(toBeRemovedIndex);
                // Also remove the next/previous element, which is the same passage but in reverse
                if (toBeRemovedIndex % 2 == 0) {
                    //message += "and ";
                    //message += passages[toBeRemovedIndex];
                    auxPassages.RemoveAt(toBeRemovedIndex);
                } else {
                    //message += "and ";
                    //message += passages[toBeRemovedIndex - 1];
                    auxPassages.RemoveAt(toBeRemovedIndex - 1);
                }
            }
            //Debug.Log(message);

            if(!passageDiscovered) {
                sectorDone = true;
            }
        }

        /*for (int i = 1; i <= stats.numberOfSectors; i ++) {
            Debug.Log(borderWithNextSector[i - 1].Count + " on border between " + i + " and " + (i + 1));
        }*/
    }

    private static void MazeFill_GrowingTreeAlg(int[,,] layout, LayoutStats stats) {
        // Explored marks the unexplored directions for each cell.
        // Visited keeps track of which cells were reached already. There is
        // a guaranteed path from the start cell to any other visited cell.
        List<MazeDirection>[,] unexplored = new List<MazeDirection>[stats.sizeZ, stats.sizeX];
        bool[,] visited = new bool[stats.sizeZ, stats.sizeX];
        for(int z = 0; z < stats.sizeZ; z++) {
            for(int x = 0; x < stats.sizeX; x++) {
                unexplored[z, x] = new List<MazeDirection>();
                if (layout[z, x, 4] > (int)CellType.InnerPadding) {
                    visited[z, x] = false;
                    if (layout[z, x, (int)MazeDirection.North] == 0) unexplored[z, x].Add(MazeDirection.North);
                    if (layout[z, x, (int)MazeDirection.East] == 0) unexplored[z, x].Add(MazeDirection.East);
                    if (layout[z, x, (int)MazeDirection.South] == 0) unexplored[z, x].Add(MazeDirection.South);
                    if (layout[z, x, (int)MazeDirection.West] == 0) unexplored[z, x].Add(MazeDirection.West);
                } else {
                    visited[z, x] = true;
                }
            }
        }


        MazeCoords currentCell;
        MazeCoords neighbour;
        MazeDirection unexploredDirection;
        int unexploredDirectionIndex;
        // Cell exploration will be done in a LIFO manner, using a stack
        Stack<MazeCoords> stack = new Stack<MazeCoords>();
        stack.Push(stats.startCell);
        visited[stats.startCell.z, stats.startCell.x] = true;

        // Whiler there are cell with unexplored directions
        while(stack.Count > 0) {
            currentCell = stack.Peek();

            // If the current cell is fully explored
            if(unexplored[currentCell.z, currentCell.x].Count == 0) {
                stack.Pop();
                continue;
            } else {
                // Choose a random direction from the list of unexplored ones and ther remove
                // that direction from the list
                unexploredDirectionIndex = UnityEngine.Random.Range(0, unexplored[currentCell.z, currentCell.x].Count);
                unexploredDirection = unexplored[currentCell.z, currentCell.x][unexploredDirectionIndex];
                unexplored[currentCell.z, currentCell.x].RemoveAt(unexploredDirectionIndex);

                neighbour = currentCell + unexploredDirection.ToMazeCoords();
                /*Debug.Log("Current cell: " + currentCell.ToString());
                //Debug.Log("Current cell + " + unexploredDirection + " = " + (currentCell + unexploredDirection.ToMazeCoords()).ToString());
                Debug.Log("Neighbour cell: " + neighbour.ToString());*/
                // If the neighbour was already visited
                if (visited[neighbour.z, neighbour.x]) {
                    // Then there is already out there a path between the starting cell
                    // and this neighbour

                    // Remove "this direction" from the neighbour's list of unexplored directions
                    unexplored[neighbour.z, neighbour.x].Remove(unexploredDirection.GetOppositeDirection());
                    // Place a wall between the current cell and this neighbour and between
                    // the neighbour and the current cell.
                    layout[currentCell.z, currentCell.x, (int)unexploredDirection] = 1;
                    if (layout[neighbour.z, neighbour.x, 4] > (int)CellType.InnerPadding) {
                        layout[neighbour.z, neighbour.x, (int)unexploredDirection.GetOppositeDirection()] = 1;
                    }
                } else {
                    // If there is at least a wall between the current cell and the neighbour, make sure there are
                    // two and then move on.
                    if (layout[currentCell.z, currentCell.x, (int)unexploredDirection] == 1 ||
                        layout[neighbour.z, neighbour.x, (int)unexploredDirection.GetOppositeDirection()] == 1) {
                        layout[currentCell.z, currentCell.x, (int)unexploredDirection] = 1;
                            layout[neighbour.z, neighbour.x, (int)unexploredDirection.GetOppositeDirection()] = 1;
                    } else {
                        // Mark the neighbour as visited, there is now a path to it.
                        visited[neighbour.z, neighbour.x] = true;
                        // Remove "this direction" from the neighbour's list of unexplored directions
                        unexplored[neighbour.z, neighbour.x].Remove(unexploredDirection.GetOppositeDirection());
                        // Add it to the stack
                        stack.Push(new MazeCoords(neighbour.z, neighbour.x));
                    }
                }
            }
        }

        /*string message = "Maze Creation:\n";
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
        Debug.Log(message);*/
    }

    private static (MazeCoords, MazeCoords) ChooseStartAndFinish(int[,,] layout, LayoutStats stats) {
        // Spiral search for a cell of common type starting with the
        // outer rim of the space contained by the outer padding.
        int currZ;
        int currX;
        int lapsDone = 0;
        int lapComplete = 1;
        int searchIterations = 0;
        int maxSearchIterations = stats.sizeZ * stats.sizeX;
        MazeCoords startCell = new MazeCoords((int)stats.sizeZ / 2, (int)stats.sizeX / 2);
        MazeCoords finishCell = startCell;
        // Start the search from a random direction
        MazeDirection movingDirection = (MazeDirection) UnityEngine.Random.Range(0, 4);
        MazeDirection startCellDirection = movingDirection;
        // Depending on the direction, set the search starting cell
        switch (movingDirection) {
            // start from the top-right corner
            case MazeDirection.North:
                currZ = stats.sizeZ - stats.outerPaddingValZ - 1;
                currX = stats.sizeX - stats.outerPaddingValX - 1;
                startCellDirection = MazeDirection.East;
                break;
            // start from the top-left corner
            case MazeDirection.West:
                currZ = stats.outerPaddingValZ;
                currX = stats.sizeX - stats.outerPaddingValX - 1;
                startCellDirection = MazeDirection.North;
                break;
            // start from the bottom-left corner
            case MazeDirection.South:
                currZ = stats.outerPaddingValZ;
                currX = stats.outerPaddingValX;
                startCellDirection = MazeDirection.West;
                break;
            // start from the bottom-right corner
            default: // MazeDirecton.East
                currZ = stats.sizeZ - stats.outerPaddingValZ - 1;
                currX = stats.outerPaddingValX;
                startCellDirection = MazeDirection.South;
                break;
        }

        /*Debug.Log("Starting the search in " + movingDirection + " at (" + currZ + "-" + currX + ")");*/
        while (layout[currZ, currX, 4] != (int)CellType.Common) {
            switch (movingDirection) {
                case MazeDirection.North:
                    if (currZ > stats.outerPaddingValZ + lapsDone) {
                        currZ--;
                    } else {
                        /*Debug.Log("Change from North to West at " + currZ + "-" + currX);*/
                        movingDirection = MazeDirection.West;
                        startCellDirection = MazeDirection.North;
                        lapComplete++;
                    }
                    break;
                case MazeDirection.West:
                    if (currX > stats.outerPaddingValX + lapsDone) {
                        currX--;
                    } else {
                        /*Debug.Log("Change from West to South at " + currZ + "-" + currX);*/
                        movingDirection = MazeDirection.South;
                        startCellDirection = MazeDirection.West;
                        lapComplete++;
                    }
                    break;
                case MazeDirection.South:
                    if (currZ < stats.sizeZ - stats.outerPaddingValZ - lapsDone - 1) {
                        currZ++;
                    } else {
                        /*Debug.Log("Change from South to East at " + currZ + "-" + currX);*/
                        movingDirection = MazeDirection.East;
                        startCellDirection = MazeDirection.South;
                        lapComplete++;
                    }
                    break;
                default: // MazeDirection.East:
                    if (currX < stats.sizeX - stats.outerPaddingValX - lapsDone - 1) {
                        currX++;
                    } else {
                        /*Debug.Log("Change from East to North at " + currZ + "-" + currX);*/
                        movingDirection = MazeDirection.North;
                        startCellDirection = MazeDirection.East;
                        lapComplete++;
                    }
                    break;
            }

            if(lapComplete == 4) {
                lapComplete = 0;
                lapsDone++;
            }

            searchIterations++;
            if(searchIterations == maxSearchIterations) {
                throw new Exception("Max. iterations of spiral search reached. Could not choose a start cell.");
            }
        }

        // Start point is chosen
        layout[currZ, currX, 4] = (int)CellType.Start;
        startCell = new MazeCoords(currZ, currX);

        // Depending on the start cell direction, a new row-by-row or column-by-column
        // search will be started in the opposite direction in order to find a free cell
        // for the finish point
        int InnPadUpperBorder = stats.outerPaddingValZ;
        int InnPadLowerBorder = stats.sizeZ - stats.outerPaddingValZ - 1;
        int InnPadLeftBorder = stats.outerPaddingValX;
        int InnPadRightBorder = stats.sizeX - stats.outerPaddingValX - 1;
        switch (startCellDirection) {
            case MazeDirection.North:
                // start a row-by-row search in South
                for(int z = InnPadLowerBorder; z > InnPadUpperBorder; z--) {
                    for(int x = InnPadLeftBorder; x < InnPadRightBorder; x++) {
                        if(layout[z, x, 4] == (int)CellType.Common) {
                            layout[z, x, 4] = (int)CellType.Finish; // Finish point is chosen
                            finishCell = new MazeCoords(z, x);
                            z = InnPadUpperBorder - 1;
                            break;
                        }
                    }
                }
                break;
            case MazeDirection.East:
                // start a column-by-column search in West
                for (int x = InnPadLeftBorder; x < InnPadRightBorder; x++) {
                    for (int z = InnPadUpperBorder; z < InnPadLowerBorder; z++) {
                        if (layout[z, x, 4] == (int)CellType.Common) {
                            layout[z, x, 4] = (int)CellType.Finish; // Finish point is chosen
                            finishCell = new MazeCoords(z, x);
                            x = InnPadRightBorder + 1;
                            break;
                        }
                    }
                }
                break;
            case MazeDirection.South:
                // start a row-by-row search in North
                for (int z = InnPadUpperBorder; z < InnPadLowerBorder; z++) {
                    for (int x = InnPadLeftBorder; x < InnPadRightBorder; x++) {
                        if (layout[z, x, 4] == (int)CellType.Common) {
                            layout[z, x, 4] = (int)CellType.Finish; // Finish point is chosen
                            finishCell = new MazeCoords(z, x);
                            z = InnPadLowerBorder + 1;
                            break;
                        }
                    }
                }
                break;
            default: // MazeDirection.West:
                // start a column-by-column search in East
                for (int x = InnPadRightBorder; x > InnPadLeftBorder; x--) {
                    for (int z = InnPadUpperBorder; z < InnPadLowerBorder; z++) {
                        if (layout[z, x, 4] == (int)CellType.Common) {
                            layout[z, x, 4] = (int)CellType.Finish; // Finish point is chosen
                            finishCell = new MazeCoords(z, x);
                            x = InnPadLeftBorder - 1;
                            break;
                        }
                    }
                }
                break;
        }

        stats.SetStartAndFinish(startCell, finishCell);
        return (startCell, finishCell);
    }

    private static void CreateInnerPadding(int[,,] layout, LayoutStats stats, int[,] visited) {
        int remainingCellsZ = stats.sizeZ - stats.outerPaddingValZ * 2 - stats.innerPaddingValZ * 2;
        int remainingCellsX = stats.sizeX - stats.outerPaddingValX * 2 - stats.innerPaddingValX * 2;
        int desiredInnerPadding = ((stats.sizeZ - stats.outerPaddingValZ * 2) * (stats.sizeX - stats.outerPaddingValX * 2)) -
                                (remainingCellsZ * remainingCellsX);

        /*int DBG_TOTAL_CORE = remainingCellsZ * remainingCellsX;
        int DBG_TOTAL_OUTER_PADDING = outerPaddingValZ * sizeX * 2 + outerPaddingValX * (sizeZ - 2 * outerPaddingValZ) * 2;
        Debug.Log("Total core: " + DBG_TOTAL_CORE);
        Debug.Log("Total inner padding: " + totalInnerPadding);
        Debug.Log("Total outer padding: " + DBG_TOTAL_OUTER_PADDING);
        Debug.Log("Total cells: " + (DBG_TOTAL_CORE + totalInnerPadding + DBG_TOTAL_OUTER_PADDING));
        Debug.Log("Real total cells: " + sizeZ * sizeX);*/

        // "frame" that will generate the inner padding
        List<MazeCoords> roots = new List<MazeCoords>();
        // array to mark already added roots so they are not added twice
        bool[,] alreadyRoot = new bool[stats.sizeZ, stats.sizeX];
        for(int z = 0; z < stats.sizeZ; z ++) {
            for(int x = 0; x < stats.sizeX; x ++) {
                alreadyRoot[z, x] = false;
            }
        }

        // initialization
        int InnPadUpperBorder = stats.outerPaddingValZ;
        int InnPadLowerBorder = stats.sizeZ - stats.outerPaddingValZ - 1;
        int InnPadLeftBorder = stats.outerPaddingValX;
        int InnPadRightBorder = stats.sizeX - stats.outerPaddingValX - 1;
        for(int x = InnPadLeftBorder; x <= InnPadRightBorder; x ++) {
            roots.Add(new MazeCoords(InnPadUpperBorder, x)); // upper edge of the frame
            roots.Add(new MazeCoords(InnPadLowerBorder, x)); // lower edge of the frame
            alreadyRoot[InnPadUpperBorder, x] = true;
            alreadyRoot[InnPadLowerBorder, x] = true;
        }
        for(int z = InnPadUpperBorder + 1; z < InnPadLowerBorder; z ++) {
            roots.Add(new MazeCoords(z, InnPadLeftBorder)); // left edge of the frame
            roots.Add(new MazeCoords(z, InnPadRightBorder)); // right edge of the frame
            alreadyRoot[z, InnPadLeftBorder] = true;
            alreadyRoot[z, InnPadRightBorder] = true;
        }

        // inner padding cells selection
        MazeCoords selectedCell;
        int selectedCellIndex;
        int totalPaddingNeighbours;
        int totalInnerPadding = 0;
        List<MazeCoords> toBeRemovedRoots = new List<MazeCoords>();
        while (totalInnerPadding < desiredInnerPadding && roots.Count != 0) {
            // randomly select new inner padding cell and remove it from roots
            selectedCellIndex = UnityEngine.Random.Range(0, roots.Count);
            selectedCell = roots[selectedCellIndex];
            // Debug.Log("Chosen (" + selectedCell.z + "-" + selectedCell.x + "). Cell " + selectedCells + " from " + totalInnerPadding + ".");
            roots.RemoveAt(selectedCellIndex);
            layout[selectedCell.z, selectedCell.x, 4] = (int)CellType.InnerPadding; // new inner padding cell
            // also mark the cell as visited for future removing of holes in the padding
            visited[selectedCell.z, selectedCell.x] = 1;
            // also increase the count of inner padding cells
            totalInnerPadding++;

            // add viable neighbours to the roots list
            if (layout[selectedCell.z - 1, selectedCell.x, 4] > (int)CellType.InnerPadding && alreadyRoot[selectedCell.z - 1, selectedCell.x] == false) {
                roots.Add(new MazeCoords(selectedCell.z - 1, selectedCell.x));
                alreadyRoot[selectedCell.z - 1, selectedCell.x] = true;
            }
            if (layout[selectedCell.z + 1, selectedCell.x, 4] > (int)CellType.InnerPadding && alreadyRoot[selectedCell.z + 1, selectedCell.x] == false) {
                roots.Add(new MazeCoords(selectedCell.z + 1, selectedCell.x));
                alreadyRoot[selectedCell.z + 1, selectedCell.x] = true;
            }
            if (layout[selectedCell.z, selectedCell.x - 1, 4] > (int)CellType.InnerPadding && alreadyRoot[selectedCell.z, selectedCell.x - 1] == false) {
                roots.Add(new MazeCoords(selectedCell.z, selectedCell.x - 1));
                alreadyRoot[selectedCell.z, selectedCell.x - 1] = true;
            }
            if (layout[selectedCell.z, selectedCell.x + 1, 4] > (int)CellType.InnerPadding && alreadyRoot[selectedCell.z, selectedCell.x + 1] == false) {
                roots.Add(new MazeCoords(selectedCell.z, selectedCell.x + 1));
                alreadyRoot[selectedCell.z, selectedCell.x + 1] = true;
            }

            // If a cell is surrounded by inner padding it must become itself part
            // of the inner padding since it is inaccessible
            foreach (MazeCoords root in roots) {
                totalPaddingNeighbours = 0;

                if (layout[root.z - 1, root.x, 4] < (int)CellType.Start) totalPaddingNeighbours++;
                if (layout[root.z + 1, root.x, 4] < (int)CellType.Start) totalPaddingNeighbours++;
                if (layout[root.z, root.x - 1, 4] < (int)CellType.Start) totalPaddingNeighbours++;
                if (layout[root.z, root.x + 1, 4] < (int)CellType.Start) totalPaddingNeighbours++;

                if (totalPaddingNeighbours == 4) {
                    layout[root.z, root.x, 4] = (int)CellType.InnerPadding; // new inner padding cell
                    visited[root.z, root.x] = 1;
                    totalInnerPadding++;
                    toBeRemovedRoots.Add(root);
                }
            }

            foreach (MazeCoords root in toBeRemovedRoots) {
                roots.Remove(root);
            }
            toBeRemovedRoots.Clear();
        }

        // Cleaning up: fill up any padding holes that may have been formed
        // Start a BFS from the center of the grid and visit all accessible cells
        // Any cell left unvisited is a hole and becomes padding
        MazeCoords startCell = new MazeCoords((int)(stats.sizeZ / 2), (int)(stats.sizeX / 2));
        MazeCoords currentCell;
        List<MazeCoords> neighbours;
        Queue<MazeCoords> q = new Queue<MazeCoords>();
        /*int DBG_HOLES_FILLED = 0;*/
        q.Enqueue(startCell);
        visited[startCell.z, startCell.x] = 1;
        while (q.Count > 0) {
            currentCell = q.Dequeue();

            neighbours = GetNeighboursOfCell_Unsafe(currentCell);
            foreach (MazeCoords neighbour in neighbours) {
                if (visited[neighbour.z, neighbour.x] == 0) {
                    visited[neighbour.z, neighbour.x] = 1;
                    q.Enqueue(neighbour);
                }
            }
        }

        for (int z = stats.outerPaddingValZ; z < stats.sizeZ - stats.outerPaddingValZ; z ++) {
            for (int x = stats.outerPaddingValX; x < stats.sizeX - stats.outerPaddingValX; x ++) {
                if (visited[z, x] == 0) { // unvisited cell, hole
                    layout[z, x, 4] = (int)CellType.InnerPadding; // new inner padding cell
                    visited[z, x] = 1;
                    totalInnerPadding++;
                    /*DBG_HOLES_FILLED++;*/
                }
            }
        }

        stats.totalInnerPadding = totalInnerPadding;
        stats.totalCore = (stats.sizeZ * stats.sizeX) - stats.totalOuterPadding - stats.totalInnerPadding;
        /*Debug.Log("Filled " + DBG_HOLES_FILLED + " holes.");
        Debug.Log("Inner padding done with a total of  " + selectedCells + " cells.");*/
    }

    // Returns the four neighbours of a cell while making sure they are within the bounds
    // of the array
    // If extended is true, it also returns the diagonal neighbours
    private static List<MazeCoords> GetNeighboursOfCell_Safe(MazeCoords currentCell, int sizeZ, int sizeX, bool extended = false, bool shuffled = false) {
        List<MazeCoords> neighbours = new List<MazeCoords>();

        bool safe_u = false; // up
        bool safe_d = false; // down
        bool safe_l = false; // left
        bool safe_r = false; // right

        if (currentCell.z - 1 >= 0) safe_u = true;
        if (currentCell.z + 1 < sizeZ) safe_d = true;
        if (currentCell.x - 1 >= 0) safe_l = true;
        if (currentCell.x + 1 < sizeX) safe_r = true;

        if (safe_u) neighbours.Add(new MazeCoords(currentCell.z - 1, currentCell.x));
        if (safe_r) neighbours.Add(new MazeCoords(currentCell.z, currentCell.x + 1));
        if (safe_d) neighbours.Add(new MazeCoords(currentCell.z + 1, currentCell.x));
        if (safe_l) neighbours.Add(new MazeCoords(currentCell.z, currentCell.x - 1));

        if(extended) {
            if (safe_u && safe_l) neighbours.Add(new MazeCoords(currentCell.z - 1, currentCell.x - 1));
            if (safe_u && safe_r) neighbours.Add(new MazeCoords(currentCell.z - 1, currentCell.x + 1));
            if (safe_d && safe_r) neighbours.Add(new MazeCoords(currentCell.z + 1, currentCell.x + 1));
            if (safe_d && safe_l) neighbours.Add(new MazeCoords(currentCell.z + 1, currentCell.x - 1));
        }

        if(shuffled) {
            List<MazeCoords> shuffledNeighbours = new List<MazeCoords>();
            int index;
            while(neighbours.Count > 0) {
                index = UnityEngine.Random.Range(0, neighbours.Count);
                shuffledNeighbours.Add(neighbours[index]);
                neighbours.RemoveAt(index);
            }
            return shuffledNeighbours;
        }

        return neighbours;
    }

    // Returns the four neighbours of a cell without checking the bounds of the array
    // If extended is true, it also returns the diagonal neighbours
    // It is important for the creation of sectors process that the returned list contains
    // first the up/down/left/right neighbours and then the diagonal ones
    private static List<MazeCoords> GetNeighboursOfCell_Unsafe(MazeCoords currentCell, bool extended = false, bool shuffled = false) {
        List<MazeCoords> neighbours = new List<MazeCoords>();

        neighbours.Add(new MazeCoords(currentCell.z - 1, currentCell.x));
        neighbours.Add(new MazeCoords(currentCell.z, currentCell.x + 1));
        neighbours.Add(new MazeCoords(currentCell.z + 1, currentCell.x));
        neighbours.Add(new MazeCoords(currentCell.z, currentCell.x - 1));

        if(extended) {
            neighbours.Add(new MazeCoords(currentCell.z - 1, currentCell.x - 1));
            neighbours.Add(new MazeCoords(currentCell.z - 1, currentCell.x + 1));
            neighbours.Add(new MazeCoords(currentCell.z + 1, currentCell.x + 1));
            neighbours.Add(new MazeCoords(currentCell.z + 1, currentCell.x - 1));
        }

        if (shuffled) {
            List<MazeCoords> shuffledNeighbours = new List<MazeCoords>();
            int index;
            while (neighbours.Count > 0) {
                index = UnityEngine.Random.Range(0, neighbours.Count);
                shuffledNeighbours.Add(neighbours[index]);
                neighbours.RemoveAt(index);
            }
            return shuffledNeighbours;
        }

        return neighbours;
    }

    private static List<MazeCoords> GetAccessibleNeighbours_Unsafe(MazeCoords currentCell, int[,,] layout) {
        List<MazeCoords> accessibleNeighbours = new List<MazeCoords>();
        // North
        if(layout[currentCell.z - 1, currentCell.x, 2] == 0 &&
            layout[currentCell.z, currentCell.x, 0] == 0 &&
            layout[currentCell.z - 1, currentCell.x, 4] >= (int)CellType.Start &&
            layout[currentCell.z - 1, currentCell.x, 4] <= (int)CellType.Common) { 
                accessibleNeighbours.Add(new MazeCoords(currentCell.z - 1, currentCell.x));
        }
        // East
        if (layout[currentCell.z, currentCell.x + 1, 3] == 0 &&
            layout[currentCell.z, currentCell.x, 1] == 0 &&
            layout[currentCell.z, currentCell.x + 1, 4] >= (int)CellType.Start &&
            layout[currentCell.z, currentCell.x + 1, 4] <= (int)CellType.Common) {
            accessibleNeighbours.Add(new MazeCoords(currentCell.z, currentCell.x + 1));
        }
        // South
        if (layout[currentCell.z + 1, currentCell.x, 0] == 0 &&
            layout[currentCell.z, currentCell.x, 2] == 0 &&
            layout[currentCell.z + 1, currentCell.x, 4] >= (int)CellType.Start &&
            layout[currentCell.z + 1, currentCell.x, 4] <= (int)CellType.Common) {
            accessibleNeighbours.Add(new MazeCoords(currentCell.z + 1, currentCell.x));
        }
        // West
        if (layout[currentCell.z, currentCell.x - 1, 1] == 0 &&
            layout[currentCell.z, currentCell.x, 3] == 0 &&
            layout[currentCell.z, currentCell.x - 1, 4] >= (int)CellType.Start &&
            layout[currentCell.z, currentCell.x - 1, 4] <= (int)CellType.Common) {
            accessibleNeighbours.Add(new MazeCoords(currentCell.z, currentCell.x - 1));
        }
        return accessibleNeighbours;
    }

    private static void CreateOuterPadding(int[,,] layout, LayoutStats stats, int[,] visited) {
        for (int z = 0; z < stats.sizeZ; z++) {
            for (int x = 0; x < stats.sizeX; x++) {
                if (z < stats.outerPaddingValZ || z > stats.sizeZ - stats.outerPaddingValZ - 1 ||
                   x < stats.outerPaddingValX || x > stats.sizeX - stats.outerPaddingValX - 1) {
                    layout[z, x, 4] = (int)CellType.OuterPadding;
                    visited[z, x] = 1;
                } else {
                    // set every other cell's type to common (4)
                    layout[z, x, 4] = (int)CellType.Common;
                }
            }
        }
    }

    public static int[,,] testAlgoritm(int sizeZ, int sizeX) {
        
        int[,,] layout = new int[sizeZ, sizeX, 5];

        /*for(int i = 0; i < sizeX; i ++) {
            for(int j = 0; j < sizeZ; j ++) {
                for(int k = 0; k < 4; k ++) {
                    layout[i, j, k] = k;
                }
            }
        }*/
        // North
        layout[0, 0, 0] = 1;
        layout[0, 1, 0] = 1;
        layout[0, 2, 0] = 1;
        layout[1, 0, 0] = 0;
        layout[1, 1, 0] = 1;
        layout[1, 2, 0] = 0;
        layout[2, 0, 0] = 0;
        layout[2, 1, 0] = 0;
        layout[2, 2, 0] = 0;

        // East
        layout[0, 0, 1] = 0;
        layout[0, 1, 1] = 1;
        layout[0, 2, 1] = 1;
        layout[1, 0, 1] = 1;
        layout[1, 1, 1] = 0;
        layout[1, 2, 1] = 1;
        layout[2, 0, 1] = 0;
        layout[2, 1, 1] = 1;
        layout[2, 2, 1] = 1;

        // South
        layout[0, 0, 2] = 0;
        layout[0, 1, 2] = 1;
        layout[0, 2, 2] = 0;
        layout[1, 0, 2] = 0;
        layout[1, 1, 2] = 0;
        layout[1, 2, 2] = 0;
        layout[2, 0, 2] = 1;
        layout[2, 1, 2] = 1;
        layout[2, 2, 2] = 1;

        // West
        layout[0, 0, 3] = 1;
        layout[0, 1, 3] = 0;
        layout[0, 2, 3] = 1;
        layout[1, 0, 3] = 1;
        layout[1, 1, 3] = 1;
        layout[1, 2, 3] = 0;
        layout[2, 0, 3] = 1;
        layout[2, 1, 3] = 0;
        layout[2, 2, 3] = 1;

        // Type
        layout[0, 0, 4] = 40;
        layout[0, 1, 4] = 20;
        layout[0, 2, 4] = 40;
        layout[1, 0, 4] = 40;
        layout[1, 1, 4] = 40;
        layout[1, 2, 4] = 40;
        layout[2, 0, 4] = 40;
        layout[2, 1, 4] = 40;
        layout[2, 2, 4] = 30;

        return layout;
    }
}

[System.Serializable]
public enum CellType {
    OuterPadding,
    InnerPadding,
    Start,
    Finish,
    Common,
    Room
}