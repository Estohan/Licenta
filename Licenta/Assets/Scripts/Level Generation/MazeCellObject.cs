using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCellObject : MonoBehaviour {

    public MazeCellData data;
    // public ObjectReferences objectPrefabs;

    public float cellSize;

    // GAME OBJECTS ?
    /*public GameObject Prf_FloorGrey;
    public GameObject Prf_FloorGreen;
    public GameObject Prf_FloorRed;
    public GameObject Prf_FloorWhite;*/

    public MazeCellObject(MazeCoords coordinates) {
        this.data.coordinates = coordinates;
        // objectPrefabs = GameManager.instance.GetComponent<ObjectReferences>();
    }


    internal void InstantiateFloor() {

        // Create floor as child of the cell
        // GameObject _floorObj = DecideFloorType(data.type); [TODO] Remove this
        if (!data.hasObjectReference[0] && data.type != CellType.Room) {
            return;
        }

        GameObject _floorObj;

        if (data.type != CellType.Room) { // Common type object
            _floorObj = ObjectDatabase.instance.GetArchitecture(data.objectReferences[0].Item1,
                                                                           (ObjectType)data.objectReferences[0].Item2,
                                                                           data.objectReferences[0].Item3);
        } else { // Room type object
            RoomObjectCell roomObjCell = ObjectDatabase.instance.GetRoomCell(data.roomObjStage, data.room, data.offsetToRoomAnchor);
            if(roomObjCell != null) {
                _floorObj = roomObjCell._floor;
            } else { // Default blank floor
                _floorObj = ObjectDatabase.instance.GetArchitecture(0, ObjectType.Floor, 0);
            }
        }

        Instantiate(_floorObj, this.transform);
        // Move cell to correct position
        this.cellSize = this.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size.x;
        this.transform.parent = transform;
        this.transform.localPosition =
            new Vector3(data.coordinates.x * cellSize - cellSize / 2,
                        0f,
                        -data.coordinates.z * cellSize + cellSize / 2);
    }

    public void InstantiateWalls() {
        if (data.type > CellType.InnerPadding) { // not outerPadding or innerPadding
            foreach (MazeDirection direction in Enum.GetValues(typeof(MazeDirection))) {
                if (data.HasWallInDirection(direction) && data.hasObjectReference[(int)direction + 1]) {
                    // Set name and set cell subsection in which to be placed
                    LevelGenerator.CellSubsections subsection = LevelGenerator.CellSubsections.Inner;
                    String name = "UNNAMED";
                    switch (direction) {
                        case MazeDirection.North:
                            subsection = LevelGenerator.CellSubsections.NorthEdge;
                            name = "NorthWall";
                            break;
                        case MazeDirection.East:
                            subsection = LevelGenerator.CellSubsections.EastEdge;
                            name = "EastWall";
                            break;
                        case MazeDirection.South:
                            subsection = LevelGenerator.CellSubsections.SouthEdge;
                            name = "SouthWall";
                            break;
                        case MazeDirection.West:
                            subsection = LevelGenerator.CellSubsections.WestEdge;
                            name = "WestWall";
                            break;
                    }
                    // Create wall as child of its cell
                    GameObject _wallObj;
                    if (data.type != CellType.Room) { // Common type cell
                        _wallObj = ObjectDatabase.instance.GetArchitecture(
                                                data.objectReferences[(int)direction + 1].Item1,
                                                (ObjectType)data.objectReferences[(int)direction + 1].Item2,
                                                data.objectReferences[(int)direction + 1].Item3);
                    } else { // Room type cell
                        RoomObjectCell roomObjCell = ObjectDatabase.instance.GetRoomCell(
                                                                    data.roomObjStage,
                                                                    data.room,
                                                                    data.offsetToRoomAnchor);
                        if(roomObjCell != null) {
                            _wallObj = roomObjCell.GetWall(direction);
                        } else { // default blank object
                            _wallObj = ObjectDatabase.instance.GetArchitecture(0, ObjectType.NEWall, 0);
                        }
                    }
                    GameObject newWall = Instantiate(_wallObj, this.transform);
                    newWall.name = name + data.coordinates.z + "-" + data.coordinates.x;
                    // Set to correct position
                    newWall.transform.localPosition = GetCellSubsectionPos(newWall.transform,
                                                                            cellSize,
                                                                            subsection);
                    // Rotate to correct position
                    newWall.transform.rotation = GetRotationFromSubsection(newWall.transform,
                                                                            subsection);
                }
            }
        }
    }

    public void InstantiateCorners() {
        GameObject cornerType;
        int objStage, objType, objIndex;

        if (data.type > CellType.InnerPadding) {
            for (int k = 0; k < 4; k++) {
                if (data.hasObjectReference[5 + k]) {
                    // Get the corner game object
                    if (data.type != CellType.Room) { // Common cell
                        (objStage, objType, objIndex) = data.objectReferences[5 + k];
                        cornerType = ObjectDatabase.instance.GetArchitecture(objStage, (ObjectType)objType, objIndex);
                    } else { // Room cell
                        RoomObjectCell roomObjCell = ObjectDatabase.instance.GetRoomCell(
                                                                        data.roomObjStage,
                                                                        data.room,
                                                                        data.offsetToRoomAnchor);
                        if(roomObjCell != null) {
                            cornerType = roomObjCell.GetCorner(k);
                        } else { // default blank object
                            cornerType = ObjectDatabase.instance.GetArchitecture(0, ObjectType.NoFaceCorner, 0);
                        }
                    }
                    // Set name and set cell subsection in which to be placed
                    // Also set rotation depending on corner position
                    LevelGenerator.CellSubsections subsection = LevelGenerator.CellSubsections.Inner;
                    String name = "UNNAMED";
                    Quaternion rotation = Quaternion.identity;
                    bool[] wallPlacement = data.walls;
                    switch (k) {
                        case 0: // North-West corner
                            subsection = LevelGenerator.CellSubsections.NWCorner;
                            name = "NWCorner";
                            if (data.cornerFaces[k] == 1) {
                                // 1F corner rotations
                                if (wallPlacement[0]) { // Check existence of North wall
                                    rotation = Quaternion.identity; // Face S
                                } else {
                                    rotation = Quaternion.Euler(0f, 270f, 0f); // Face E
                                }
                            } else {
                                // 2F (and 0F) corner rotation
                                rotation = Quaternion.Euler(0f, 270f, 0f); // Face SE
                            }
                            break;
                        case 1: // North-East corner
                            subsection = LevelGenerator.CellSubsections.NECorner;
                            name = "NECorner";
                            if (data.cornerFaces[k] == 1) {
                                // 1F corner rotations
                                if (wallPlacement[0]) { // Check existence of North wall
                                    rotation = Quaternion.identity; // Face S
                                } else {
                                    rotation = Quaternion.Euler(0f, 90f, 0f); // Face W
                                }
                            } else {
                                // 2F (and 0F) corner rotation
                                rotation = Quaternion.identity; // Face SW
                            }
                            break;
                        case 2: // South-East corner
                            subsection = LevelGenerator.CellSubsections.SECorner;
                            name = "SECorner";
                            if (data.cornerFaces[k] == 1) {
                                // 1F corner rotations
                                if (wallPlacement[2]) { // Check existence of South wall
                                    rotation = Quaternion.Euler(0f, 180f, 0f); // Face N
                                } else {
                                    rotation = Quaternion.Euler(0f, 90f, 0f); // Face W
                                }
                            } else {
                                // 2F (and 0F) corner rotation
                                rotation = Quaternion.Euler(0f, 90f, 0f); // Face NW
                            }
                            break;
                        case 3: // South-West corner
                            subsection = LevelGenerator.CellSubsections.SWCorner;
                            name = "SWCorner";
                            if (data.cornerFaces[k] == 1) {
                                // 1F corner rotations
                                if (wallPlacement[2]) { // Check existence of South wall
                                    rotation = Quaternion.Euler(0f, 180f, 0f); // Face N
                                } else {
                                    rotation = Quaternion.Euler(0f, 270f, 0f); // Face E
                                }
                            } else {
                                // 2F (and 0F) corner rotation
                                rotation = Quaternion.Euler(0f, 180f, 0f); // Face NE
                            }
                            break;
                    }
                    // Create wall as child of its cell
                    GameObject newCorner = Instantiate(cornerType, this.transform);
                    newCorner.name = name + data.coordinates.z + "-" + data.coordinates.x;
                    // Set to correct position
                    newCorner.transform.localPosition = GetCellSubsectionPos(newCorner.transform,
                                                                            cellSize,
                                                                            subsection);
                    newCorner.transform.rotation = rotation;
                }
            }
        }
    }

    public void DebugColor() {
        /*float r = 0.1f;
        float g = 0.1f;
        float b = 0.6f;
        Color color = new Color(r, g, b, 1.0f);*/
        List<Color> colors = new List<Color>();
        colors.Add(new Color(0.1f, 0.1f, 0.5f, 1.0f));
        colors.Add(new Color(0.1f, 0.5f, 0.1f, 1.0f));
        colors.Add(new Color(0.5f, 0.1f, 0.1f, 1.0f));

        colors.Add(new Color(0.1f, 0.5f, 0.5f, 1.0f));
        colors.Add(new Color(0.5f, 0.1f, 0.5f, 1.0f));
        colors.Add(new Color(1.0f, 0.1f, 0.1f, 1.0f));

        colors.Add(new Color(1.0f, 0.5f, 0.1f, 1.0f)); // rooms

        Transform minimapFloor = null;

        if (data.hasObjectReference[0]) {
            minimapFloor = this.transform.GetChild(0).transform.Find("MinimapFloor"); ;
        }
        // Color sector
        if(minimapFloor != null && this.data.sector > 0) {
            if(this.data.type == CellType.Room) {
                if(this.data.sector == 6) {
                    minimapFloor.GetComponent<Renderer>().material.color = colors[this.data.sector - 1];
                } else {
                    minimapFloor.GetComponent<Renderer>().material.color = colors[colors.Count - 1];
                }
            } else {
                minimapFloor.GetComponent<Renderer>().material.color = colors[this.data.sector - 1];
            }
        }
        // this.GetComponentInChildren<Renderer>().material.color = color;
    }

    // REMOVE THIS LATER
    private GameObject DecideFloorType(CellType type) {
        switch (type) {
            case CellType.OuterPadding:
                return  ObjectReferences.instance._OuterPadding;
            case CellType.InnerPadding:
                return ObjectReferences.instance._InnerPadding;
            case CellType.Start:
                return ObjectReferences.instance._Start;
            case CellType.Finish:
                return ObjectReferences.instance._Finish;
            case CellType.Common:
                return ObjectReferences.instance._FloorWhite;
            case CellType.Room:
                return ObjectReferences.instance._FloorWhite;
        }
        return ObjectReferences.instance._FloorWhite;
    }

    // Returns the position of a cell subsection given the
    // position of the parent cell
    private Vector3 GetCellSubsectionPos(Transform transform, float cellSize, LevelGenerator.CellSubsections subsection) {
        float z, x;
        // offset of z or x, so that it is places inside the cell
        float depthOffset = (transform.GetComponent<Renderer>().bounds.size.z) / 2;
        // offset of y so that the object is placed "on the floor"
        float y = (transform.GetComponent<Renderer>().bounds.size.y) / 2;
        switch (subsection) {
            case LevelGenerator.CellSubsections.NorthEdge:
                z = transform.localPosition.z + cellSize / 2 - depthOffset;
                return new Vector3(0f, y, z);
            case LevelGenerator.CellSubsections.EastEdge:
                x = transform.localPosition.x + cellSize / 2 - depthOffset;
                return new Vector3(x, y, 0f);
            case LevelGenerator.CellSubsections.SouthEdge:
                z = transform.localPosition.z - cellSize / 2 + depthOffset;
                return new Vector3(0f, y, z);
            case LevelGenerator.CellSubsections.WestEdge:
                x = transform.localPosition.x - cellSize / 2 + depthOffset;
                return new Vector3(x, y, 0f);
            case LevelGenerator.CellSubsections.NWCorner:
                z = transform.localPosition.z + cellSize / 2 - depthOffset;
                x = transform.localPosition.x - cellSize / 2 + depthOffset;
                return new Vector3(x, y, z);
            case LevelGenerator.CellSubsections.NECorner:
                z = transform.localPosition.z + cellSize / 2 - depthOffset;
                x = transform.localPosition.x + cellSize / 2 - depthOffset;
                return new Vector3(x, y, z);
            case LevelGenerator.CellSubsections.SECorner:
                z = transform.localPosition.z - cellSize / 2 + depthOffset;
                x = transform.localPosition.x + cellSize / 2 - depthOffset;
                return new Vector3(x, y, z);
            case LevelGenerator.CellSubsections.SWCorner:
                z = transform.localPosition.z - cellSize / 2 + depthOffset;
                x = transform.localPosition.x - cellSize / 2 + depthOffset;
                return new Vector3(x, y, z);
            case LevelGenerator.CellSubsections.NWInner:
                break;
            case LevelGenerator.CellSubsections.NInner:
                break;
            case LevelGenerator.CellSubsections.NEInner:
                break;
            case LevelGenerator.CellSubsections.WInner:
                break;
            case LevelGenerator.CellSubsections.Inner:
                break;
            case LevelGenerator.CellSubsections.EInner:
                break;
            case LevelGenerator.CellSubsections.SWInner:
                break;
            case LevelGenerator.CellSubsections.SInner:
                break;
            case LevelGenerator.CellSubsections.SEInner:
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
    private Quaternion GetRotationFromSubsection(Transform transform, LevelGenerator.CellSubsections subsection,
                                                 bool cornered = false, Vector3 targetPos = default) {
        switch (subsection) {
            case LevelGenerator.CellSubsections.NorthEdge:
                return Quaternion.identity;
            case LevelGenerator.CellSubsections.EastEdge:
                return Quaternion.Euler(0f, 90f, 0f);
            case LevelGenerator.CellSubsections.SouthEdge:
                return Quaternion.Euler(0f, 180f, 0f);
            case LevelGenerator.CellSubsections.WestEdge:
                return Quaternion.Euler(0f, 270f, 0f);
            case LevelGenerator.CellSubsections.NWCorner:
                break;
            case LevelGenerator.CellSubsections.NECorner:
                break;
            case LevelGenerator.CellSubsections.SECorner:
                break;
            case LevelGenerator.CellSubsections.SWCorner:
                break;
            case LevelGenerator.CellSubsections.NWInner:
                break;
            case LevelGenerator.CellSubsections.NInner:
                break;
            case LevelGenerator.CellSubsections.NEInner:
                break;
            case LevelGenerator.CellSubsections.WInner:
                break;
            case LevelGenerator.CellSubsections.Inner:
                break;
            case LevelGenerator.CellSubsections.EInner:
                break;
            case LevelGenerator.CellSubsections.SWInner:
                break;
            case LevelGenerator.CellSubsections.SInner:
                break;
            case LevelGenerator.CellSubsections.SEInner:
                break;
            default:
                throw new Exception("Unrecognized subsection received in GetCellSubsectionPos().");
        }
        return Quaternion.identity;
    }
}


/*public void InstantiateCorners() {
    GameObject cornerType = ObjectReferences.instance._Corner0;
    if (data.type > CellType.InnerPadding) {
        for (int k = 0; k < 4; k++) {
            // Choose corner type to be instantiated [TODO] Remove this
            switch (data.cornerFaces[k]) {
                case 0:
                    cornerType = ObjectReferences.instance._Corner0;
                    break;
                case 1:
                    cornerType = ObjectReferences.instance._Corner1;
                    break;
                case 2:
                    cornerType = ObjectReferences.instance._Corner2;
                    break;
            }
            // Set name and set cell subsection in which to be placed
            // Also set rotation depending on corner position
            LevelGenerator.CellSubsections subsection = LevelGenerator.CellSubsections.Inner;
            String name = "UNNAMED";
            Quaternion rotation = Quaternion.identity;
            bool[] wallPlacement = data.walls;
            switch (k) {
                case 0:
                    subsection = LevelGenerator.CellSubsections.NWCorner;
                    name = "NWCorner";
                    if (cornerType != ObjectReferences.instance._Corner2) {
                        if (wallPlacement[0]) rotation = Quaternion.identity; // Face S
                        if (wallPlacement[3]) rotation = Quaternion.Euler(0f, 270f, 0f); // Face E
                    } else {
                        rotation = Quaternion.Euler(0f, 270f, 0f); // Face SE
                    }
                    break;
                case 1:
                    subsection = LevelGenerator.CellSubsections.NECorner;
                    name = "NECorner";
                    if (cornerType != ObjectReferences.instance._Corner2) {
                        if (wallPlacement[0]) rotation = Quaternion.identity; // Face S
                        if (wallPlacement[1]) rotation = Quaternion.Euler(0f, 90f, 0f); // Face W
                    } else {
                        rotation = Quaternion.identity; // Face SW
                    }
                    break;
                case 2:
                    subsection = LevelGenerator.CellSubsections.SECorner;
                    name = "SECorner";
                    if (cornerType != ObjectReferences.instance._Corner2) {
                        if (wallPlacement[1]) rotation = Quaternion.Euler(0f, 90f, 0f); // Face W
                        if (wallPlacement[2]) rotation = Quaternion.Euler(0f, 180f, 0f); // Face N
                    } else {
                        rotation = Quaternion.Euler(0f, 90f, 0f); // Face NW
                    }
                    break;
                case 3:
                    subsection = LevelGenerator.CellSubsections.SWCorner;
                    name = "SWCorner";
                    if (cornerType != ObjectReferences.instance._Corner2) {
                        if (wallPlacement[2]) rotation = Quaternion.Euler(0f, 180f, 0f); // Face N
                        if (wallPlacement[3]) rotation = Quaternion.Euler(0f, 270f, 0f); // Face E
                    } else {
                        rotation = Quaternion.Euler(0f, 180f, 0f); // Face NE
                    }
                    break;
            }
            // Create wall as child of its cell
            GameObject newCorner = Instantiate(cornerType, this.transform);
            newCorner.name = name + data.coordinates.z + "-" + data.coordinates.x;
            // Set to correct position
            newCorner.transform.localPosition = GetCellSubsectionPos(newCorner.transform,
                                                                    cellSize,
                                                                    subsection);
            // Additional rotation of two-faced corners
            if (cornerType == ObjectReferences.instance._Corner2) {

            }
            newCorner.transform.rotation = rotation;
        }
    }
}*/