using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class MazeGenAlgorithms {

    // the layout returned is a three dimensional array of
    // x = sizeX
    // y = sizeZ
    // z = 5 (walls (N E S W) and type/sector)
    // The type is an integer n, where n / 10 represents the cell type
    // and n % 10 represents the sector (max. of 10)
    // Types: 0 - outer padding, 1 - inner padding, 2 - start cell,
    //        3 - finish cell, 4 - common cell, 5 - special room cell.

    public static (int[,,], MazeCoords startCellPos, MazeCoords finishCellPos)
        PrimsAlgorithm(int sizeZ, int sizeX, int outerPaddingDiv, int innerPaddingDiv, int nrOfSectors) {

        int[,,] layout = new int[sizeZ, sizeX, 5];
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

        // Create outer padding
        CreateOuterPadding(layout, stats, visited);

        // Create inner padding
        CreateInnerPadding(layout, stats, visited);

        // Choose start cell and end cell
        (startCell, finishCell) = ChooseStartAndFinish(layout, stats);
        /*Debug.Log("New layout: Start cell - " + startCell + " Finish cell - " + finishCell);*/

        // TODO
        // Split remaining space into sectors
        // DivideIntoSectors_Attempt_1(layout, startCell, 4, sizeZ, sizeX);
        // DivideIntoSectors_Attempt_2(layout, startCell, 2, sizeZ, sizeX);
        DivideIntoSectors_Attempt_3(layout, stats);

        // TODO
        // AddSpecialSections(layout, startCell, finishCell, nrOfSectors, sizeZ, sizeX);

        // Fill with corridors (maze)
        // MazeFill_Prims(layout, stats);

        // DEBUG

        Debug.Log("Here are some stats:");
        Debug.Log("Cells total: " + stats.sizeZ * stats.sizeX + " = " + stats.totalOuterPadding + " + " + stats.totalInnerPadding + " + " + stats.totalCore + " + " +
          (stats.totalOuterPadding + stats.totalInnerPadding + stats.totalCore));
        String message = "Sectors data: \n";
        int DEBUGG_type;
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
                DEBUGG_type = (layout[cell.z, cell.x, 4] / 10) * 10;
                layout[cell.z, cell.x, 4] = DEBUGG_type + 6;
            }
            message += "\n";
            // COLOR STUFF
            /*foreach(MazeCoords cell in stats.sectorBorder[sector - 1]) {
                DEBUGG_type = (layout[cell.z, cell.x, 4] / 10) * 10;
                if (sector == 2 || sector == 4) {
                    layout[cell.z, cell.x, 4] = DEBUGG_type + 6;
                }
            }*/
        }
        Debug.Log(message);

        return (layout, startCell, finishCell);
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
            // If there are no rooms this large, the limit will be the largest existing type of room
            if(percentOfSectorSize >= RoomLayouts.rooms.Length) {
                percentOfSectorSize = RoomLayouts.rooms.Length - 1;
            }
            initialMaxSectionSize[sector - 1] = percentOfSectorSize;
            initialMaxSectionTotal[sector - 1] = percentOfSectorSize * 2;
        }

        // Step 2: Place sections/rooms
        bool largestOnePlaced;
        bool roomPlaced;
        int currentMaxSectionSize;
        int currentMaxSectionTotal;
        MazeCoords anchor;
        MazeDirection rotation;
        List<(int, int)>[] room;
        List<(MazeCoords, MazeDirection)>[] placedRooms = new List<(MazeCoords, MazeDirection)>[stats.numberOfSectors];
        // CREATE TEMPORARY LAYOUT?

        for(int sector = 1; sector <= stats.numberOfSectors; sector++) {
            // Initialize placed rooms list
            placedRooms[sector - 1] = new List<(MazeCoords, MazeDirection)>();
            largestOnePlaced = false;
            currentMaxSectionSize = initialMaxSectionSize[sector - 1];
            currentMaxSectionTotal = initialMaxSectionTotal[sector - 1];
            // While there is space for more rooms
            while(currentMaxSectionTotal > 0) {
                // If the current maximum room size is bigger than the 
                // current unallocated room space
                // Ex.: can't get a size 5 room anymore if I only have 4 cells of space left
                if(currentMaxSectionSize <= currentMaxSectionTotal) {
                    currentMaxSectionSize = currentMaxSectionTotal;
                }
                // Choose a room
                if(largestOnePlaced) {
                    room = RoomLayouts.GetRandomRoomOfMaxSizeN(currentMaxSectionSize);
                } else {
                    room = RoomLayouts.GetRandomRoomOfSizeN(currentMaxSectionSize);
                    largestOnePlaced = true;
                }
                // Find a place for it
                roomPlaced = false;
                while(!roomPlaced) {
                    // DEBUG - Utils.GetRandomFrom(container)
                    // Choose anchor
                    anchor = stats.sectorCells[sector - 1][UnityEngine.Random.Range(0, stats.sectorCells[sector - 1].Count)];
                    // Test rotations placement
                    (roomPlaced, rotation) = CheckRoomPlacement(layout, anchor, sector, room);
                    if(roomPlaced) {

                    }

                }
                // 1. Randomly choose an anchor
                // 2. Test chosen room on it. In case of failure return to [1].
                // 3. Check room exit accessibility. In case of failure return to [1].
                // 4. Check for blockages. In case of failure return to [1].
                // 5. Mark layout and save room. Exit [1] loop.
            }
        }
        
        throw new NotImplementedException();
    }

    private static (bool roomPlaced, MazeDirection rotation) 
        CheckRoomPlacement(int[,,] layout, MazeCoords anchor, int sector, List<(int, int)>[] room) {
        bool roomPlaced = false;
        int rotation = -1;
        int cellSector;
        int cellType;
        MazeCoords aux;

        foreach(List<(int, int)> roomRotation in room) {
            rotation++;
            roomPlaced = true;
            // Check overlapping
            foreach((int z, int x) in roomRotation) {
                cellSector = layout[z, x, 4] % 10;
                cellType = layout[z, x, 4] / 10;
                if(cellSector != sector || cellType != (int)CellType.Common) {
                    roomPlaced = false;
                    break;
                }
            }
            // It the room does not overlap with anything,
            // a) check if it can be accessed
            // b) check if it blocks another room's entrance
            // c) check if it blocks a sector exit or sector section
            if(roomPlaced) {
                // a)
                aux = anchor + ((MazeDirection)rotation).ToMazeCoords();
                if(layout[aux.z, aux.x, 4] / 10 != (int)CellType.Common) {
                    roomPlaced = false;
                    return (roomPlaced, (MazeDirection)rotation);
                }
                return (roomPlaced, (MazeDirection)rotation);
            }
        }
        // Unable to place room
        return (roomPlaced, MazeDirection.South);
    }

    private static void DivideIntoSectors_Attempt_3(int[,,] layout, LayoutStats stats) {
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
                        layout[cellCandidate.z, cellCandidate.x, 4] >= ((int)CellType.Start) * 10) {
                        // Claim this cell
                        layout[cellCandidate.z, cellCandidate.x, 4] += sector;
                        visited[cellCandidate.z, cellCandidate.x] = true;
                        noCellAssignedYet = false;
                        // Record its assignation into stats
                        stats.sectorCells[sector - 1].Add(cellCandidate);
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
                if(layout[z, x, 4] >= ((int)CellType.Start) * 10) {
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
                if (layout[z, x, 4] >= ((int)CellType.Start) * 10) {
                    thisCell = new MazeCoords(z, x);
                    thisCellSector = layout[thisCell.z, thisCell.x, 4] % 10;
                    foreach(MazeDirection direction in unexplored[z, x]) {
                        neighbour = thisCell + direction.ToMazeCoords();
                        neighbourSector = layout[neighbour.z, neighbour.x, 4] % 10;
                        // If thisCell and its neighbour belong to different sectors then put a wall
                        // between thisCell and its neighbour... in this cell
                        if (thisCellSector != neighbourSector) {
                            // Debug.Log("Found border betweeen " + thisCell + " and " + neighbour + " (" + thisCellSector + " and " + neighbourSector + ")");
                            layout[thisCell.z, thisCell.x, (int)direction] = 1;
                            // ... and in the neighbour's cell
                            if(layout[neighbour.z, neighbour.x, 4] >= ((int)CellType.Start) * 10) {
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
        int currentSector = layout[stats.startCell.z, stats.startCell.x, 4] % 10;
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
                if(layout[cell.z, cell.x, 4] % 10 == currentSector) {
                    //Debug.Log("Found passage: ");
                    //Debug.Log("[" + cell + " " + dir + " " + layout[(cell + dir.ToMazeCoords()).z, (cell + dir.ToMazeCoords()).x, 4] % 10 + "]");
                    stats.nextSectorExit[currentSector - 1].Add(auxPassages[i]);
                    nextSector.Enqueue(layout[(cell + dir.ToMazeCoords()).z, (cell + dir.ToMazeCoords()).x, 4] % 10);
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

        for (int i = 1; i <= stats.numberOfSectors; i ++) {
            Debug.Log(borderWithNextSector[i - 1].Count + " on border between " + i + " and " + (i + 1));
        }
    }

    private static void DivideIntoSectors_Attempt_2(int[,,] layout, MazeCoords startCell, int sectorsNumber, int sizeZ, int sizeX) {
        // Place "Seed square" in the center of the level
        List<MazeCoords> border = new List<MazeCoords>();
        MazeCoords center = new MazeCoords(sizeZ / 2, sizeX / 2);
        border.Add(new MazeCoords(center.z - 1, center.x - 1));
        border.Add(new MazeCoords(center.z - 1, center.x));
        border.Add(new MazeCoords(center.z - 1, center.x + 1));
        border.Add(new MazeCoords(center.z, center.x + 1));
        border.Add(new MazeCoords(center.z + 1, center.x + 1));
        border.Add(new MazeCoords(center.z + 1, center.x));
        border.Add(new MazeCoords(center.z + 1, center.x - 1));
        border.Add(new MazeCoords(center.z, center.x - 1));

        foreach(MazeCoords cell in border) {
            layout[cell.z, cell.x, 4] += 1;
        }

        // Place seeds
        List<MazeCoords> sectorSeeds = new List<MazeCoords>();
        int spaceBetweenSeeds = border.Count / sectorsNumber;
        int currIndex = 0;

        Debug.Log(border.Count);
        for (int sector = 2; sector < sectorsNumber + 2; sector++) {
            Debug.Log(currIndex);
            sectorSeeds.Add(border[currIndex]);
            layout[border[currIndex].z, border[currIndex].x, 4] += 1;
            currIndex += spaceBetweenSeeds;
        }

        bool[,] visited = new bool[sizeZ, sizeX];

        for (int z = 0; z < sizeZ; z++) {
            for (int x = 0; x < sizeX; x++) {
                visited[z, x] = false;
            }
        }
    }

    private static void DivideIntoSectors_Attempt_1(int[,,] layout, MazeCoords startCell, int sectorsNumber, int sizeZ, int sizeX) {
        // Step 1: Get the border of the maze
        MazeCoords borderStart = startCell;
        MazeCoords currentBorderCell = startCell;
        List<MazeCoords> neighbours;
        List<MazeCoords> neighboursOfNeighbour;
        List<MazeCoords> border = new List<MazeCoords>();
        Queue<MazeCoords> borderCandidates = new Queue<MazeCoords>();
        bool isOnBorder = false;
        bool[,] visited = new bool[sizeZ, sizeX];

        /*int DEBUG_LIMIT = 100000;
        int DEBUG_COUNTER = 0;*/

        for(int z = 0; z < sizeZ; z ++) {
            for(int x = 0; x < sizeX; x ++) {
                visited[z, x] = false;
            }
        }

        borderCandidates.Enqueue(borderStart);
        visited[currentBorderCell.z, currentBorderCell.x] = true;
        while(borderCandidates.Count > 0) {
            // Search for the next border cell
            currentBorderCell = borderCandidates.Dequeue();
            neighbours = GetNeighboursOfCell_Unsafe(currentBorderCell);
            // Get each neighbour that is adjacent to a padding cell
            foreach(MazeCoords neigh in neighbours) {
                if (!visited[neigh.z, neigh.x] && layout[neigh.z, neigh.x, 4] >= ((int)CellType.Start) * 10) {
                    neighboursOfNeighbour = GetNeighboursOfCell_Unsafe(neigh, true);
                    foreach (MazeCoords secondNeigh in neighboursOfNeighbour) {
                        if (layout[secondNeigh.z, secondNeigh.x, 4] < ((int)CellType.Start) * 10) {
                            isOnBorder = true;
                            break;
                        }
                    }
                    if (isOnBorder) {
                        borderCandidates.Enqueue(neigh);
                        border.Add(currentBorderCell);
                        visited[neigh.z, neigh.x] = true;
                        layout[neigh.z, neigh.x, 4] += 1;
                        isOnBorder = false;
                    }
                }
            }
            /*DEBUG_COUNTER++;
            if(DEBUG_COUNTER > DEBUG_LIMIT) {
                Debug.Log("MAX ITERATIONS LIMIT REACHED");
                break;
            }*/
        }

        /*String DEBUG_MESSAGE = "";
        for (int z = 0; z < sizeZ; z++) {
            for (int x = 0; x < sizeX; x++) {
                DEBUG_MESSAGE += layout[z, x, 4] + " ";
            }
            DEBUG_MESSAGE += "\n";
        }
        Debug.Log(DEBUG_MESSAGE);*/

        // Step 2: Choose n starting points for n sectors
        List<MazeCoords> sectorSeeds = new List<MazeCoords>();
        int spaceBetweenSeeds = border.Count / sectorsNumber;
        int currIndex = 0;

        Debug.Log(border.Count);
        for(int sector = 2; sector < sectorsNumber + 2; sector ++) {
            Debug.Log(currIndex);
            sectorSeeds.Add(border[currIndex]);
            layout[border[currIndex].z, border[currIndex].x, 4] += 1;
            currIndex += spaceBetweenSeeds;
        }
    }

    private static void MazeFill_Prims(int[,,] layout, LayoutStats stats) {
        // Explored marks the unexplored directions for each cell.
        // Visited keeps track of which cells were reached already. There is
        // a guaranteed path from the start cell to any other visited cell.
        List<MazeDirection>[,] unexplored = new List<MazeDirection>[stats.sizeZ, stats.sizeX];
        bool[,] visited = new bool[stats.sizeZ, stats.sizeX];
        for(int z = 0; z < stats.sizeZ; z++) {
            for(int x = 0; x < stats.sizeX; x++) {
                unexplored[z, x] = new List<MazeDirection>();
                if (layout[z, x, 4] > 10) {
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
                    if (layout[neighbour.z, neighbour.x, 4] > 10) {
                        layout[neighbour.z, neighbour.x, (int)unexploredDirection.GetOppositeDirection()] = 1;
                    }
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
        while (layout[currZ, currX, 4] != 40) {
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
        layout[currZ, currX, 4] = 20;
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
                        if(layout[z, x, 4] == 40) {
                            layout[z, x, 4] = 30; // Finish point is chosen
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
                        if (layout[z, x, 4] == 40) {
                            layout[z, x, 4] = 30; // Finish point is chosen
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
                        if (layout[z, x, 4] == 40) {
                            layout[z, x, 4] = 30; // Finish point is chosen
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
                        if (layout[z, x, 4] == 40) {
                            layout[z, x, 4] = 30; // Finish point is chosen
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
            layout[selectedCell.z, selectedCell.x, 4] = 10; // new inner padding cell
            // also mark the cell as visited for future removing of holes in the padding
            visited[selectedCell.z, selectedCell.x] = 1;
            // also increase the count of inner padding cells
            totalInnerPadding++;

            // add viable neighbours to the roots list
            if (layout[selectedCell.z - 1, selectedCell.x, 4] > 10 && alreadyRoot[selectedCell.z - 1, selectedCell.x] == false) {
                roots.Add(new MazeCoords(selectedCell.z - 1, selectedCell.x));
                alreadyRoot[selectedCell.z - 1, selectedCell.x] = true;
            }
            if (layout[selectedCell.z + 1, selectedCell.x, 4] > 10 && alreadyRoot[selectedCell.z + 1, selectedCell.x] == false) {
                roots.Add(new MazeCoords(selectedCell.z + 1, selectedCell.x));
                alreadyRoot[selectedCell.z + 1, selectedCell.x] = true;
            }
            if (layout[selectedCell.z, selectedCell.x - 1, 4] > 10 && alreadyRoot[selectedCell.z, selectedCell.x - 1] == false) {
                roots.Add(new MazeCoords(selectedCell.z, selectedCell.x - 1));
                alreadyRoot[selectedCell.z, selectedCell.x - 1] = true;
            }
            if (layout[selectedCell.z, selectedCell.x + 1, 4] > 10 && alreadyRoot[selectedCell.z, selectedCell.x + 1] == false) {
                roots.Add(new MazeCoords(selectedCell.z, selectedCell.x + 1));
                alreadyRoot[selectedCell.z, selectedCell.x + 1] = true;
            }

            // If a cell is surrounded by inner padding it must become itself part
            // of the inner padding since it is inaccessible
            foreach (MazeCoords root in roots) {
                totalPaddingNeighbours = 0;

                if (layout[root.z - 1, root.x, 4] < 20) totalPaddingNeighbours++;
                if (layout[root.z + 1, root.x, 4] < 20) totalPaddingNeighbours++;
                if (layout[root.z, root.x - 1, 4] < 20) totalPaddingNeighbours++;
                if (layout[root.z, root.x + 1, 4] < 20) totalPaddingNeighbours++;

                if (totalPaddingNeighbours == 4) {
                    layout[root.z, root.x, 4] = 10; // new inner padding cell
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
                    layout[z, x, 4] = 10;
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
    private static List<MazeCoords> GetNeighboursOfCell_Safe(MazeCoords currentCell, bool extended = false, bool shuffled = false) {
        List<MazeCoords> neighbours = new List<MazeCoords>();

        bool safe_u = false; // up
        bool safe_d = false; // down
        bool safe_l = false; // left
        bool safe_r = false; // right

        if (currentCell.z - 1 >= 0) safe_u = true;
        if (currentCell.z + 1 >= 0) safe_d = true;
        if (currentCell.x - 1 >= 0) safe_l = true;
        if (currentCell.x + 1 >= 0) safe_r = true;

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

    private static void CreateOuterPadding(int[,,] layout, LayoutStats stats, int[,] visited) {
        for (int z = 0; z < stats.sizeZ; z++) {
            for (int x = 0; x < stats.sizeX; x++) {
                if (z < stats.outerPaddingValZ || z > stats.sizeZ - stats.outerPaddingValZ - 1 ||
                   x < stats.outerPaddingValX || x > stats.sizeX - stats.outerPaddingValX - 1) {
                    layout[z, x, 4] = 0;
                    visited[z, x] = 1;
                } else {
                    // set every other cell's type to common (40)
                    layout[z, x, 4] = 40;
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