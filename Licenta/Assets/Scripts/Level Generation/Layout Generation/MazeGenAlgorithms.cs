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

    public static (int[,,], MazeCoords startCellPos, MazeCoords finishCellPos)
        PrimsAlgorithm(int sizeZ, int sizeX, int outerPaddingDiv, int innerPaddingDiv, int nrOfSectors) {
        int[,,] layout = new int[sizeZ, sizeX, 5];
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
        (startCell, finishCell) = ChooseStartAndFinish(layout, outerPaddingValZ, outerPaddingValX, sizeZ, sizeX);
        Debug.Log("Start cell: " + startCell + "\nFinish cell: " + finishCell);

        // TODO
        // Split remaining space into sectors

        // TODO
        // ADD rooms

        // TODO
        // Fill with corridors (maze)
        MazeFill_Prims(layout, startCell, finishCell, sizeZ, sizeX);

        return (layout, startCell, finishCell);
    }

    private static void MazeFill_Prims(int[,,] layout, MazeCoords startCell, MazeCoords finishCell, int sizeZ, int sizeX) {
        // Set all start, finish and all common cells to not visited
        /*for (int z = 0; z < sizeZ; z++) {
            for (int x = 0; x < sizeX; x++) {
                if (layout[z, x, 4] > 10) {
                    visited[z, x] = 0;
                }
            }
        }*/
        // Explored marks the unexplored directions for each cell.
        // Visited keeps track of which cells were reached already. There is
        // a guaranteed path from the start cell to any other visited cell.
        List<MazeDirection>[,] unexplored = new List<MazeDirection>[sizeZ, sizeX];
        bool[,] visited = new bool[sizeZ, sizeX];
        for(int z = 0; z < sizeZ; z++) {
            for(int x = 0; x < sizeX; x++) {
                unexplored[z, x] = new List<MazeDirection>();
                if (layout[z, x, 4] > 10) {
                    visited[z, x] = false;
                    unexplored[z, x].Add(MazeDirection.North);
                    unexplored[z, x].Add(MazeDirection.East);
                    unexplored[z, x].Add(MazeDirection.South);
                    unexplored[z, x].Add(MazeDirection.West);
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
        stack.Push(startCell);
        visited[startCell.z, startCell.x] = true;

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

        string message = "Maze Creation:\n";
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
        Debug.Log(message);
    }

    private static (MazeCoords, MazeCoords) ChooseStartAndFinish(int[,,] layout, int outerPaddingValZ, int outerPaddingValX, int sizeZ, int sizeX) {
        // Spiral search for a cell of common type starting with the
        // outer rim of the space contained by the outer padding.
        int currZ;
        int currX;
        int lapsDone = 0;
        int lapComplete = 1;
        int searchIterations = 0;
        int maxSearchIterations = sizeZ * sizeX;
        MazeCoords startCell = new MazeCoords((int) sizeZ / 2, (int) sizeX / 2);
        MazeCoords finishCell = startCell;
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
        startCell = new MazeCoords(currZ, currX);

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

        return (startCell, finishCell);
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

[System.Serializable]
public enum CellType {
    OuterPadding,
    InnerPadding,
    Start,
    Finish,
    Common,
    Room
}