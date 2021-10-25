using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenAlgorithms {

    // the layout returned is a three dimensional array of
    // x = sizeX
    // y = sizeZ
    // z = 5 (walls (N E S W) and type/sector (T))
    // The type is an integer n, where n / 10 represents the cell type
    // and n % 10 represents the sector (max. of 10)
    // Types: 0 - outer padding, 1 - inner padding, 2 - start cell,
    //        3 - finish cell, 4 - common cell, 5 - special room cell.

    public static int[,,] PrimsAlgorithm(int sizeZ, int sizeX, int outerPaddingDiv, int innerPaddingDiv, int nrOfSectors) {
        int[,,] layout = new int[sizeZ, sizeX, 5];

        // 6(OPl) + 3(IPl) + 12(M) + 3(IPr) + 6(OPr) = 12 + 6 + 12 = 30
        int outerPaddingValZ = (int)(sizeZ / outerPaddingDiv);
        int outerPaddingValX = (int)(sizeX / outerPaddingDiv);
        int innerPaddingValZ = (int)(sizeZ / innerPaddingDiv);
        int innerPaddingValX = (int)(sizeX / innerPaddingDiv);
        int remainingCellsZ = sizeZ - outerPaddingValZ * 2 - innerPaddingValZ * 2; // DO I NEED THIS?
        int remainingCellsX = sizeX - outerPaddingValX * 2 - innerPaddingValX * 2; // DO I NEED THIS?
        int totalInnerPadding = ((sizeZ - outerPaddingValZ * 2) * (sizeX - outerPaddingValX * 2)) - 
                                (remainingCellsZ * remainingCellsX); // DO I NEED THIS?

        // visisted array initialization
        int[,] visited = new int[sizeZ, sizeX];
        for(int z = 0; z < sizeZ; z ++) {
            for(int x = 0; x < sizeX; x ++) {
                visited[z, x] = 0;
            }
        }

        // Create outer padding
        CreateOuterPadding(layout, visited, outerPaddingValZ, outerPaddingValX, sizeZ, sizeX);

        // Create inner padding
        CreateInnerPadding(layout, visited, outerPaddingValZ, outerPaddingValX, innerPaddingValZ, innerPaddingValX, sizeZ, sizeX);

        /*string message = "\n";
        for (int z = 0; z < sizeZ; z++) {
            for (int x = 0; x < sizeX; x++) {
                message += visited[z, x] + " ";
            }
            message += "\n";
        }
        Debug.Log(message);*/

        // Choose start cell and end cell
        ChooseStartAndFinish(layout, outerPaddingValZ, outerPaddingValX, sizeZ, sizeX);

        // TODO
        // Split remaining space into sectors

        // TODO
        // ADD rooms

        // TODO
        // Fill with corridors (maze)
        // mazeFill_Prims(layout, visited, sizeZ, sizeX);

        return layout;
    }

    private static void mazeFill_Prims(int[,,] layout, int[,] visited, int sizeZ, int sizeX) {
        // Set all start, finish and all common cells to not visited
        for (int z = 0; z < sizeZ; z++) {
            for (int x = 0; x < sizeX; x++) {
                if (layout[z, x, 4] > 10) {
                    visited[z, x] = 0;
                }
            }
        }

        //
    }

    private static void ChooseStartAndFinish(int[,,] layout, int outerPaddingValZ, int outerPaddingValX, int sizeZ, int sizeX) {
        // Spiral search for a cell of common type starting with the
        // outer rim of the space contained by the outer padding.
        int currZ;
        int currX;
        int lapsDone = 0;
        int lapComplete = 1;
        int searchIterations = 0;
        int maxSearchIterations = sizeZ * sizeX;
        // Start the search from a random direction
        MazeDirection movingDirection = (MazeDirection) UnityEngine.Random.Range(0, 4);
        MazeDirection startCellDirection = movingDirection;
        // Depending of the direction, set the search starting cell
        switch (movingDirection) {
            // start from the top-right corner
            case MazeDirection.North:
                currZ = sizeZ - outerPaddingValZ - 1;
                currX = sizeX - outerPaddingValX - 1;
                startCellDirection = MazeDirection.East;
                break;
            // start from the top-left corner
            case MazeDirection.West:
                currZ = outerPaddingValZ;
                currX = sizeX - outerPaddingValX - 1;
                startCellDirection = MazeDirection.North;
                break;
            // start from the bottom-left corner
            case MazeDirection.South:
                currZ = outerPaddingValZ;
                currX = outerPaddingValX;
                startCellDirection = MazeDirection.West;
                break;
            // start from the bottom-right corner
            default: // MazeDirecton.East
                currZ = sizeZ - outerPaddingValZ - 1;
                currX = outerPaddingValX;
                startCellDirection = MazeDirection.South;
                break;
        }

        /*Debug.Log("Starting the search in " + movingDirection + " at (" + currZ + "-" + currX + ")");*/
        while (layout[currZ, currX, 4] != 40) {
            switch (movingDirection) {
                case MazeDirection.North:
                    if (currZ > outerPaddingValZ + lapsDone) {
                        currZ--;
                    } else {
                        /*Debug.Log("Change from North to West at " + currZ + "-" + currX);*/
                        movingDirection = MazeDirection.West;
                        startCellDirection = MazeDirection.North;
                        lapComplete++;
                    }
                    break;
                case MazeDirection.West:
                    if (currX > outerPaddingValX + lapsDone) {
                        currX--;
                    } else {
                        /*Debug.Log("Change from West to South at " + currZ + "-" + currX);*/
                        movingDirection = MazeDirection.South;
                        startCellDirection = MazeDirection.West;
                        lapComplete++;
                    }
                    break;
                case MazeDirection.South:
                    if (currZ < sizeZ - outerPaddingValZ - lapsDone - 1) {
                        currZ++;
                    } else {
                        /*Debug.Log("Change from South to East at " + currZ + "-" + currX);*/
                        movingDirection = MazeDirection.East;
                        startCellDirection = MazeDirection.South;
                        lapComplete++;
                    }
                    break;
                default: // MazeDirection.East:
                    if (currX < sizeX - outerPaddingValX - lapsDone - 1) {
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

        // Depending on the start cell direction, a new row-by-row or column-by-column
        // search will be started in the opposite direction in order to find a free cell
        // for the finish point
        int InnPadUpperBorder = outerPaddingValZ;
        int InnPadLowerBorder = sizeZ - outerPaddingValZ - 1;
        int InnPadLeftBorder = outerPaddingValX;
        int InnPadRightBorder = sizeX - outerPaddingValX - 1;
        switch (startCellDirection) {
            case MazeDirection.North:
                // start a row-by-row search in South
                for(int z = InnPadLowerBorder; z > InnPadUpperBorder; z--) {
                    for(int x = InnPadLeftBorder; x < InnPadRightBorder; x++) {
                        if(layout[z, x, 4] == 40) {
                            layout[z, x, 4] = 30; // Finish point is chosen
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
                            z = InnPadLowerBorder + 1;
                            break;
                        }
                    }
                }
                break;
            case MazeDirection.West:
                // start a column-by-column search in East
                for (int x = InnPadRightBorder; x > InnPadLeftBorder; x--) {
                    for (int z = InnPadUpperBorder; z > InnPadLowerBorder; z++) {
                        if (layout[z, x, 4] == 40) {
                            layout[z, x, 4] = 30; // Finish point is chosen
                            x = InnPadLeftBorder - 1;
                            break;
                        }
                    }
                }
                break;
        }
    }

    private static void CreateInnerPadding(int[,,] layout, int[,] visited, int outerPaddingValZ, int outerPaddingValX, int innerPaddingValZ, int innerPaddingValX, int sizeZ, int sizeX) {
        int remainingCellsZ = sizeZ - outerPaddingValZ * 2 - innerPaddingValZ * 2;
        int remainingCellsX = sizeX - outerPaddingValX * 2 - innerPaddingValX * 2;
        int totalInnerPadding = ((sizeZ - outerPaddingValZ * 2) * (sizeX - outerPaddingValX * 2)) -
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
        bool[,] alreadyRoot = new bool[sizeZ, sizeX];
        for(int z = 0; z < sizeZ; z ++) {
            for(int x = 0; x < sizeX; x ++) {
                alreadyRoot[z, x] = false;
            }
        }

        // initialization
        int InnPadUpperBorder = outerPaddingValZ;
        int InnPadLowerBorder = sizeZ - outerPaddingValZ - 1;
        int InnPadLeftBorder = outerPaddingValX;
        int InnPadRightBorder = sizeX - outerPaddingValX - 1;
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
        int selectedCells = 0;
        List<MazeCoords> toBeRemovedRoots = new List<MazeCoords>();
        while (selectedCells < totalInnerPadding && roots.Count != 0) {
            // randomly select new inner padding cell and remove it from roots
            selectedCellIndex = UnityEngine.Random.Range(0, roots.Count);
            selectedCell = roots[selectedCellIndex];
            // Debug.Log("Chosen (" + selectedCell.z + "-" + selectedCell.x + "). Cell " + selectedCells + " from " + totalInnerPadding + ".");
            roots.RemoveAt(selectedCellIndex);
            layout[selectedCell.z, selectedCell.x, 4] = 10; // new inner padding cell
            selectedCells++;
            // also mark the cell as visited for future removing of holes in the padding
            visited[selectedCell.z, selectedCell.x] = 1;

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
                    toBeRemovedRoots.Add(root);
                    selectedCells++;
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
        MazeCoords startCell = new MazeCoords((int)(sizeZ / 2), (int)(sizeX / 2));
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

        for (int z = outerPaddingValZ; z < sizeZ - outerPaddingValZ; z ++) {
            for (int x = outerPaddingValX; x < sizeX - outerPaddingValX; x ++) {
                if (visited[z, x] == 0) { // unvisited cell, hole
                    layout[z, x, 4] = 10;
                    selectedCells++;
                    /*DBG_HOLES_FILLED++;*/
                    visited[z, x] = 1;
                }
            }
        }

        /*Debug.Log("Filled " + DBG_HOLES_FILLED + " holes.");
        Debug.Log("Inner padding done with a total of  " + selectedCells + " cells.");*/
    }

    // Returns the four neighbours of a cell while making sure they are within the bounds
    // of the array
    private static List<MazeCoords> GetNeighboursOfCell_Safe(MazeCoords currentCell, int sizeZ, int sizeX) {
        List<MazeCoords> neighbours = new List<MazeCoords>();
        
        if (currentCell.z - 1 >= 0) neighbours.Add(new MazeCoords(currentCell.z - 1, currentCell.x));
        if (currentCell.z + 1 >= 0) neighbours.Add(new MazeCoords(currentCell.z + 1, currentCell.x));
        if (currentCell.x - 1 >= 0) neighbours.Add(new MazeCoords(currentCell.z, currentCell.x - 1));
        if (currentCell.x + 1 >= 0) neighbours.Add(new MazeCoords(currentCell.z, currentCell.x + 1));

        return neighbours;
    }

    // Returns the four neighbours of a cell without checking the bounds of the array
    private static List<MazeCoords> GetNeighboursOfCell_Unsafe(MazeCoords currentCell) {
        List<MazeCoords> neighbours = new List<MazeCoords>();

        neighbours.Add(new MazeCoords(currentCell.z - 1, currentCell.x));
        neighbours.Add(new MazeCoords(currentCell.z + 1, currentCell.x));
        neighbours.Add(new MazeCoords(currentCell.z, currentCell.x - 1));
        neighbours.Add(new MazeCoords(currentCell.z, currentCell.x + 1));

        return neighbours;
    }

    private static void CreateOuterPadding(int[,,] layout, int[,] visited, int outerPaddingValZ, int outerPaddingValX, int sizeZ, int sizeX) {
        for (int z = 0; z < sizeZ; z++) {
            for (int x = 0; x < sizeX; x++) {
                if (z < outerPaddingValZ || z > sizeZ - outerPaddingValZ - 1 ||
                   x < outerPaddingValX || x > sizeX - outerPaddingValX - 1) {
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




public enum CellType {
    OuterPadding,
    InnerPadding,
    Start,
    Finish,
    Common,
    Room
}