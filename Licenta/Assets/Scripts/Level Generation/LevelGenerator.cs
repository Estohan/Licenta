using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

    private Level level;
    private int sizeZ, sizeX;

    // GAMEOBJECTS - TEMPORARY LOCATION
    public GameObject _FloorGrey;
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
    public GameObject _InnerPadding;
    public MazeCellObject _MazeCellObject;

    // !! This function will need some parameters
    // Uses MazeGenAlgorithms to generate a maze layout and
    // saves that layout into a two dimensional array of MazeCellData
    public Level GenerateLevel(int sizeZ, int sizeX, int outerPaddingDiv, int innerPaddingDiv) {
        // minimum size of 30 ?
        this.sizeZ = sizeZ;
        this.sizeX = sizeZ;

        //int[,,] layout = MazeGenAlgorithms.testAlgoritm(sizeZ, sizeX, 1);
        int[,,] layout = MazeGenAlgorithms.PrimsAlgorithm(sizeZ, sizeX, outerPaddingDiv, innerPaddingDiv, 3);
        level = new Level(sizeZ, sizeX, layout);

        // printing layout
        string message = "\n";
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
        print(message);

        return level;
    }

    public void InstantiateLevel() {
        InstantiateCells();
        InstantiateWalls();
        // InstantiateCorners();
    }

    private void InstantiateCorners() {
        GameObject cornerType = _Corner0;
        for(int z = 0; z < sizeZ; z ++) {
            for (int x = 0; x < sizeX; x++) {
                if (level.getCellData(z, x).type == CellType.Common) {
                    for (int k = 0; k < 4; k++) {
                        // Choose corner type to be instantiated
                        switch (level.getCellData(z, x).cornerFaces[k]) {
                            case 0:
                                cornerType = _Corner0;
                                break;
                            case 1:
                                cornerType = _Corner1;
                                break;
                            case 2:
                                cornerType = _Corner2;
                                break;
                        }
                        // Set name and set cell subsection in which to be placed
                        // Also set rotation depending on corner position
                        CellSubsections subsection = CellSubsections.Inner;
                        String name = "UNNAMED";
                        Quaternion rotation = Quaternion.identity;
                        bool[] wallPlacement = level.getCellData(z, x).walls;
                        switch (k) {
                            case 0:
                                subsection = CellSubsections.NWCorner;
                                name = "NWCorner";
                                if (cornerType != _Corner2) {
                                    if (wallPlacement[0]) rotation = Quaternion.identity; // Face S
                                    if (wallPlacement[3]) rotation = Quaternion.Euler(0f, 270f, 0f); // Face E
                                } else {
                                    rotation = Quaternion.Euler(0f, 270f, 0f); // Face SE
                                }
                                break;
                            case 1:
                                subsection = CellSubsections.NECorner;
                                name = "NECorner";
                                if (cornerType != _Corner2) {
                                    if (wallPlacement[0]) rotation = Quaternion.identity; // Face S
                                    if (wallPlacement[1]) rotation = Quaternion.Euler(0f, 90f, 0f); // Face W
                                } else {
                                    rotation = Quaternion.identity; // Face SW
                                }
                                break;
                            case 2:
                                subsection = CellSubsections.SECorner;
                                name = "SECorner";
                                if (cornerType != _Corner2) {
                                    if (wallPlacement[1]) rotation = Quaternion.Euler(0f, 90f, 0f); // Face W
                                    if (wallPlacement[2]) rotation = Quaternion.Euler(0f, 180f, 0f); // Face N
                                } else {
                                    rotation = Quaternion.Euler(0f, 90f, 0f); // Face NW
                                }
                                break;
                            case 3:
                                subsection = CellSubsections.SWCorner;
                                name = "SWCorner";
                                if (cornerType != _Corner2) {
                                    if (wallPlacement[2]) rotation = Quaternion.Euler(0f, 180f, 0f); // Face N
                                    if (wallPlacement[3]) rotation = Quaternion.Euler(0f, 270f, 0f); // Face E
                                } else {
                                    rotation = Quaternion.Euler(0f, 180f, 0f); // Face NE
                                }
                                break;
                        }
                        // Create wall as child of its cell
                        GameObject newCorner = Instantiate(cornerType, level.cellsObjects[z, x].transform);
                        newCorner.name = name + z + "-" + x;
                        // Set to correct position
                        newCorner.transform.localPosition = GetCellSubsectionPos(newCorner.transform,
                                                                                level.cellsObjects[z, x].size,
                                                                                subsection);
                        // Additional rotation of two-faced corners
                        if (cornerType == _Corner2) {

                        }
                        newCorner.transform.rotation = rotation;
                    }
                }
            }
        }
    }

    private void InstantiateWalls() {
        for (int z = 0; z < sizeZ; z++) {
            for (int x = 0; x < sizeX; x++) {
                if (level.getCellData(z, x).type == CellType.Common) {
                    foreach (MazeDirection direction in Enum.GetValues(typeof(MazeDirection))) {
                        if (level.getCellData(z, x).HasWallInDirection(direction)) {
                            // Set name and set cell subsection in which to be placed
                            CellSubsections subsection = CellSubsections.Inner;
                            String name = "UNNAMED";
                            switch (direction) {
                                case MazeDirection.North:
                                    subsection = CellSubsections.NorthEdge;
                                    name = "NorthWall";
                                    break;
                                case MazeDirection.East:
                                    subsection = CellSubsections.EastEdge;
                                    name = "EastWall";
                                    break;
                                case MazeDirection.South:
                                    subsection = CellSubsections.SouthEdge;
                                    name = "SouthWall";
                                    break;
                                case MazeDirection.West:
                                    subsection = CellSubsections.WestEdge;
                                    name = "WestWall";
                                    break;
                            }
                            // Create wall as child of its cell
                            GameObject newWall = Instantiate(_Wall, level.cellsObjects[z, x].transform);
                            newWall.name = name + z + "-" + x;
                            // Set to correct position
                            newWall.transform.localPosition = GetCellSubsectionPos(newWall.transform,
                                                                                   level.cellsObjects[z, x].size,
                                                                                   subsection);
                            // Rotate to correct position
                            newWall.transform.rotation = GetRotationFromSubsection(newWall.transform,
                                                                                   subsection);
                        }
                    }
                }
            }
        }
    }

    private void InstantiateCells() {
        // (int sizeZ, int sizeX) = level.getDimensions();
        float cellSize;
        for(int z = 0; z < sizeZ; z ++) {
            for(int x = 0; x < sizeX; x ++) {
                // Create cell object
                MazeCellObject newCellObject = Instantiate(_MazeCellObject) as MazeCellObject;
                newCellObject.data = level.getCellData(z, x);
                newCellObject.name = "Cell " + z + "-" + x;
                newCellObject.transform.parent = this.transform;
                // Create floor as child of the cell
                GameObject _floorObj = DecideFloorType(level.getCellData(z, x).type);
                Instantiate(_floorObj, newCellObject.transform);
                // Move cell to correct position
                newCellObject.size = newCellObject.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.x;
                cellSize = newCellObject.size;
                newCellObject.transform.parent = transform;
                newCellObject.transform.localPosition =
                    new Vector3(x*cellSize - cellSize/2,
                                0f,
                                -z*cellSize + cellSize/2);
                // Save newly created cell
                level.cellsObjects[z, x] = newCellObject;
            }
        }
    }

    // REMOVE THIS LATER
    private GameObject DecideFloorType(CellType type) {
        switch(type) {
            case CellType.OuterPadding:
                return _OuterPadding;
            case CellType.InnerPadding:
                return _InnerPadding;
            case CellType.Start:
                return _Start;
            case CellType.Finish:
                return _Finish;
            case CellType.Common:
                return _FloorWhite;
            case CellType.Room:
                return _FloorRed;
        }
        return _FloorWhite;
    }

    // Returns the position of a cell subsection given the
    // position of the parent cell
    private Vector3 GetCellSubsectionPos(Transform transform, float cellSize, CellSubsections subsection) {
        float z, x;
        // offset of z or x, so that it is places inside the cell
        float depthOffset = (transform.GetComponent<Renderer>().bounds.size.z) / 2;
        // offset of y so that the object is placed "on the floor"
        float y = (transform.GetComponent<Renderer>().bounds.size.y) / 2;
        switch (subsection) {
            case CellSubsections.NorthEdge:
                z = transform.localPosition.z + cellSize / 2 - depthOffset;
                return new Vector3(0f, y, z);
            case CellSubsections.EastEdge:
                x = transform.localPosition.x + cellSize / 2 - depthOffset;
                return new Vector3(x, y, 0f);
            case CellSubsections.SouthEdge:
                z = transform.localPosition.z - cellSize / 2 + depthOffset;
                return new Vector3(0f, y, z);
            case CellSubsections.WestEdge:
                x = transform.localPosition.x - cellSize / 2 + depthOffset;
                return new Vector3(x, y, 0f);
            case CellSubsections.NWCorner:
                z = transform.localPosition.z + cellSize / 2 - depthOffset;
                x = transform.localPosition.x - cellSize / 2 + depthOffset;
                return new Vector3(x, y, z);
            case CellSubsections.NECorner:
                z = transform.localPosition.z + cellSize / 2 - depthOffset;
                x = transform.localPosition.x + cellSize / 2 - depthOffset;
                return new Vector3(x, y, z);
            case CellSubsections.SECorner:
                z = transform.localPosition.z - cellSize / 2 + depthOffset;
                x = transform.localPosition.x + cellSize / 2 - depthOffset;
                return new Vector3(x, y, z);
            case CellSubsections.SWCorner:
                z = transform.localPosition.z - cellSize / 2 + depthOffset;
                x = transform.localPosition.x - cellSize / 2 + depthOffset;
                return new Vector3(x, y, z);
            case CellSubsections.NWInner:
                break;
            case CellSubsections.NInner:
                break;
            case CellSubsections.NEInner:
                break;
            case CellSubsections.WInner:
                break;
            case CellSubsections.Inner:
                break;
            case CellSubsections.EInner:
                break;
            case CellSubsections.SWInner:
                break;
            case CellSubsections.SInner:
                break;
            case CellSubsections.SEInner:
                break;
            default:
                throw new Exception("Unrecognized subsection received in GetCellSubsectionPos().");
        }
        return new Vector3(0f, 0f, 0f);
    }

    /*
     *  cornerWallLeft
     * |
     * |__
     * |__|____ cornerWallRight
     */
    /*
     * Returns rotation quaternion depending on the subsection an object is placed in.
     */
    private Quaternion GetRotationFromSubsection(Transform transform, CellSubsections subsection,
                                                 bool cornered= false, Vector3 targetPos = default) {
        switch (subsection) {
            case CellSubsections.NorthEdge:
                return Quaternion.identity;
            case CellSubsections.EastEdge:
                return Quaternion.Euler(0f, 90f, 0f);
            case CellSubsections.SouthEdge:
                return Quaternion.Euler(0f, 180f, 0f);
            case CellSubsections.WestEdge:
                return Quaternion.Euler(0f, 270f, 0f);
            case CellSubsections.NWCorner:
                break;
            case CellSubsections.NECorner:
                break;
            case CellSubsections.SECorner:
                break;
            case CellSubsections.SWCorner:
                break;
            case CellSubsections.NWInner:
                break;
            case CellSubsections.NInner:
                break;
            case CellSubsections.NEInner:
                break;
            case CellSubsections.WInner:
                break;
            case CellSubsections.Inner:
                break;
            case CellSubsections.EInner:
                break;
            case CellSubsections.SWInner:
                break;
            case CellSubsections.SInner:
                break;
            case CellSubsections.SEInner:
                break;
            default:
                throw new Exception("Unrecognized subsection received in GetCellSubsectionPos().");
        }
        return Quaternion.identity;
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
