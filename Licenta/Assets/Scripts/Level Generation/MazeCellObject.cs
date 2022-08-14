using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCellObject : MonoBehaviour {

    [SerializeField]
    private CellConcealer cellConcealer;
    private bool visited;

    public MazeCellData data;

    // GAME OBJECTS ?
    /*public GameObject Prf_FloorGrey;
    public GameObject Prf_FloorGreen;
    public GameObject Prf_FloorRed;
    public GameObject Prf_FloorWhite;*/

    private void Start() {
        visited = false;
    }

    public MazeCellObject(MazeCoords coordinates) {
        this.data.coordinates = coordinates;
        // objectPrefabs = GameManager.instance.GetComponent<ObjectReferences>();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.transform.CompareTag("Player")) {
            GameEventSystem.instance.PlayerMoveedToAnotherCell(data);
            if (!visited) {
                cellConcealer.RevealNeighbours();
                visited = true;
            }
        }
    }


    /*
     ==========================================================================
     CELL INSTANTIATION
     ==========================================================================     
     */


    internal void InstantiateFloor() {

        // Create floor as child of the cell
        // GameObject _floorObj = DecideFloorType(data.type); [TODO] Remove this
        if (!data.hasObjectReference[0]) { // && data.type != CellType.Room) {
            //Debug.LogError("Cell " + data.coordinates + " cannot instantiate floor (no object reference).");
            return;
        }

        GameObject _floorObj;

        if (data.type != CellType.Room) { // Common type object
            _floorObj = ObjectDatabase.instance.GetArchitecture(data.objectReferences[0].Item1,
                                                                (ObjectType)data.objectReferences[0].Item2,
                                                                data.objectReferences[0].Item3);
        } else { // Room type object
            RoomObjectCell roomObjCell = ObjectDatabase.instance.GetRoomCell(data.roomObjStage, data.room, data.offsetToRoomAnchor);
            if(roomObjCell != null && roomObjCell.HasFloor()) {
                _floorObj = roomObjCell._floor;
            } else { // Default blank floor
                // _floorObj = ObjectDatabase.instance.GetArchitecture(0, ObjectType.Floor, 0);
                _floorObj = null;
            }
        }

        if (_floorObj != null) {
            Instantiate(_floorObj, this.transform);
        }
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
                        if(roomObjCell != null && roomObjCell.HasWall(direction)) {
                            _wallObj = roomObjCell.GetWall(direction);
                        } else { // default blank object
                            // _wallObj = ObjectDatabase.instance.GetArchitecture(0, ObjectType.NEWall, 0);
                            _wallObj = null;
                        }
                    }

                    if (_wallObj != null) {
                        GameObject newWall = Instantiate(_wallObj, this.transform);
                        newWall.name = name + data.coordinates.z + "-" + data.coordinates.x;
                        // Set to correct position
                        newWall.transform.localPosition = GetCellSubsectionPos(newWall.transform,
                                                                                Constants.cellSize,
                                                                                subsection);
                        // Rotate to correct position
                        newWall.transform.rotation = GetRotationFromSubsection(newWall.transform,
                                                                                subsection);
                    }
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
                        if(roomObjCell != null && roomObjCell.HasCorner(k)) {
                            cornerType = roomObjCell.GetCorner(k);
                        } else { // default blank object
                            // cornerType = ObjectDatabase.instance.GetArchitecture(0, ObjectType.NoFaceCorner, 0);
                            cornerType = null;
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

                    if (cornerType != null) {
                        // Create corner as child of its cell
                        GameObject newCorner = Instantiate(cornerType, this.transform);
                        newCorner.name = name + data.coordinates.z + "-" + data.coordinates.x;
                        // Set to correct position
                        newCorner.transform.localPosition = GetCellSubsectionPos(newCorner.transform,
                                                                                Constants.cellSize,
                                                                                subsection);
                        newCorner.transform.rotation = rotation;
                    }
                }
            }
        }
    }

    public void InstantiateContent() {
        int stage = 1;
        Quaternion rotation = Quaternion.identity;
        // Test
        if(data.type == CellType.Obstacle && data.obst_anchor == true) {
            GameObject obstacle = Instantiate(ObjectDatabase.instance.GetObstacle(stage, data.obst_shapeID, data.obst_obstacleID).
                                                                       GetObstacle(data.obst_rotation, data.obst_difficulty),
                                                this.transform);
            /*switch (data.rotation) {
                case MazeDirection.East:
                    rotation = Quaternion.Euler(0f, 90f, 0f);
                    break;
                case MazeDirection.South:
                    rotation = Quaternion.Euler(0f, 180f, 0f);
                    break;
                case MazeDirection.West:
                    rotation = Quaternion.Euler(0f, 270f, 0f);
                    break;
                default: // North
                    break;
             }
            obstacle.transform.rotation = rotation;*/
        }
        if (data.type == CellType.Room && data.offsetToRoomAnchor.z == 0 && data.offsetToRoomAnchor.x == 0) {
            if (ObjectDatabase.instance.GetRoomCell(stage, data.room, data.offsetToRoomAnchor).HasInterior()) {
                GameObject interior = Instantiate(ObjectDatabase.instance.GetRoomCell(stage, data.room, data.offsetToRoomAnchor).
                                                    GetInterior(),
                                                    this.transform);
            }
        }
    }

    // Returns the position of a cell subsection given the
    // position of the parent cell
    private Vector3 GetCellSubsectionPos(Transform transform, float cellSize, LevelGenerator.CellSubsections subsection) {
        float z, x;
        // offset of z or x, so that it is places inside the cell
        // float depthOffset = (transform.GetComponent<Renderer>().bounds.size.z) / 2;
        // float depthOffset = Constants.cellSize / Constants.cellSegments / 2; //  half the size of a subsesction
        // offset of y so that the object is placed "on the floor" - only needed if the object's origin is set to geometry
        float y = 0f; // (transform.GetComponent<Renderer>().bounds.size.y) / 2;
        switch (subsection) {
            case LevelGenerator.CellSubsections.NorthEdge:
                z = transform.localPosition.z + cellSize / 2 - (Constants.wallWidth / 2);
                return new Vector3(0f, y, z);
            case LevelGenerator.CellSubsections.EastEdge:
                x = transform.localPosition.x + cellSize / 2 - (Constants.wallWidth / 2);
                return new Vector3(x, y, 0f);
            case LevelGenerator.CellSubsections.SouthEdge:
                z = transform.localPosition.z - cellSize / 2 + (Constants.wallWidth / 2);
                return new Vector3(0f, y, z);
            case LevelGenerator.CellSubsections.WestEdge:
                x = transform.localPosition.x - cellSize / 2 + (Constants.wallWidth / 2);
                return new Vector3(x, y, 0f);
            case LevelGenerator.CellSubsections.NWCorner:
                z = transform.localPosition.z + cellSize / 2 - (Constants.wallWidth / 2);
                x = transform.localPosition.x - cellSize / 2 + (Constants.wallWidth / 2);
                return new Vector3(x, y, z);
            case LevelGenerator.CellSubsections.NECorner:
                z = transform.localPosition.z + cellSize / 2 - (Constants.wallWidth / 2);
                x = transform.localPosition.x + cellSize / 2 - (Constants.wallWidth / 2);
                return new Vector3(x, y, z);
            case LevelGenerator.CellSubsections.SECorner:
                z = transform.localPosition.z - cellSize / 2 + (Constants.wallWidth / 2);
                x = transform.localPosition.x + cellSize / 2 - (Constants.wallWidth / 2);
                return new Vector3(x, y, z);
            case LevelGenerator.CellSubsections.SWCorner:
                z = transform.localPosition.z - cellSize / 2 + (Constants.wallWidth / 2);
                x = transform.localPosition.x - cellSize / 2 + (Constants.wallWidth / 2);
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

        colors.Add(new Color(1.0f, 1.0f, 1.0f, 1.0f)); // solution: count -2 
        colors.Add(new Color(1.0f, 0.5f, 0.1f, 1.0f)); // rooms: count - 1

        Transform minimapFloor = null;

        // Check if there is a minimap icon associated
        if (data.hasObjectReference[0]) {
            minimapFloor = this.transform.GetChild(0).transform.Find("MinimapFloor"); ;
        }
        // Color sector
        if (minimapFloor != null && this.data.sector > 0) {
            if (this.data.type == CellType.Room) {
                if (this.data.sector == 6) {
                    minimapFloor.GetComponent<Renderer>().material.color = colors[this.data.sector - 1];
                } else {
                    minimapFloor.GetComponent<Renderer>().material.color = colors[colors.Count - 1];
                }
            } else {
                if(data.cellStats.isInSolution) {
                    minimapFloor.GetComponent<Renderer>().material.color = colors[colors.Count - 2];
                } else {
                    minimapFloor.GetComponent<Renderer>().material.color = colors[this.data.sector - 1];
                }
            }
        }
        // this.GetComponentInChildren<Renderer>().material.color = color;
    }

    public void DebugColor2() {
        float r = 0.3f;
        float g = 0.1f;
        float b = 0.1f;
        Color color = new Color(r, g, b, 1.0f);

        Transform minimapFloor = null;

        // Check if there is a minimap icon associated
        if (data.hasObjectReference[0]) {
            minimapFloor = this.transform.GetChild(0).transform.Find("MinimapFloor"); ;
        }
        // Color sector
        if (minimapFloor != null && this.data.sector > 0) {
            if (this.data.type == CellType.Room) {
            } else {
                minimapFloor.GetComponent<Renderer>().material.color = color + new Color(0.0f, 
                                                                                           0.0f,
                                                                                           this.data.cellStats.distanceToStart / 100f,
                                                                                           0.0f);
            }
        }
        // this.GetComponentInChildren<Renderer>().material.color = color;
    }

    public CellConcealer GetOccluder() {
        return cellConcealer;
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