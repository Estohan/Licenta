using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDatabase : MonoBehaviour {
    public static ObjectDatabase instance;

    public MazeCellObject _mazeCellObject;
    public GameObject _levelRoot;
    public GameStageObjects commonObjects;
    public GameStageObjects firstStageObjects;

    public GameObject GetArchitecture(int stage, ObjectType objType, int index) {
        // switch by stage
        GameStageObjects currentStageObjects;
        List<GameObject> desiredContainer;

        switch (stage) {
            case 0:
                currentStageObjects = commonObjects;
                break;
            default:
                currentStageObjects = firstStageObjects;
                break;
        }

        switch (objType) {
            case ObjectType.NEWall:
                desiredContainer = currentStageObjects.neWalls;
                break;
            case ObjectType.SWWall:
                desiredContainer = currentStageObjects.swWalls;
                break;
            case ObjectType.TwoFaceCorner:
                desiredContainer = currentStageObjects.twoFacecorners;
                break;
            case ObjectType.OneFaceCorner:
                desiredContainer = currentStageObjects.oneFacecorners;
                break;
            case ObjectType.NoFaceCorner:
                desiredContainer = currentStageObjects.noFacecorners;
                break;
            case ObjectType.InnerPadding:
                desiredContainer = currentStageObjects.innerPadding;
                break;
            case ObjectType.OuterPadding:
                desiredContainer = currentStageObjects.outerPadding;
                break;
            default: // floor
                return currentStageObjects.floors[index];
        }

        if (desiredContainer.Count >= index + 1) {
            return desiredContainer[index];
        } else {
            Debug.Log("ObjectDatabase: BAD INDEX IN GETARTCHITECTURE()");
            return desiredContainer[0];
        }
    }

    public int GetObjectsPoolSize(int stage, ObjectType objType) {
        GameStageObjects currentStageObjects;

        switch (stage) {
            case 0:
                currentStageObjects = commonObjects;
                break;
            default:
                currentStageObjects = firstStageObjects;
                break;
        }

        switch (objType) {
            case ObjectType.NEWall:
                return currentStageObjects.neWalls.Count;
            case ObjectType.SWWall:
                return currentStageObjects.swWalls.Count;
            case ObjectType.TwoFaceCorner:
                return currentStageObjects.twoFacecorners.Count;
            case ObjectType.OneFaceCorner:
                return currentStageObjects.oneFacecorners.Count;
            case ObjectType.NoFaceCorner:
                return currentStageObjects.noFacecorners.Count;
            case ObjectType.InnerPadding:
                return currentStageObjects.innerPadding.Count;
            case ObjectType.OuterPadding:
                return currentStageObjects.outerPadding.Count;
            default: // floor
                return currentStageObjects.floors.Count;
        }
    }

    public List<ObstacleObject> GetObstaclesOfShapeID(int stage, int shapeID) {
        GameStageObjects currentStageObjects = firstStageObjects;

        if(shapeID < currentStageObjects.obstacleObjectsByShape.Count) {
            return currentStageObjects.obstacleObjectsByShape[shapeID].obstacleObjectsOfShapeID;
        } else {
            throw new System.Exception("GetObstaclesOfShapeID: No existing obstacles for shapeID " + shapeID);
        }
    }

    public ObstacleObject GetObstacle(int stage, int shapeID, int obstacleID) {
        GameStageObjects currentStageObjects = firstStageObjects;
        
        if(shapeID < currentStageObjects.obstacleObjectsByShape.Count) {
            if (obstacleID < currentStageObjects.obstacleObjectsByShape[shapeID].obstacleObjectsOfShapeID.Count) {
                return currentStageObjects.obstacleObjectsByShape[shapeID].obstacleObjectsOfShapeID[obstacleID];
            } else {
                throw new System.Exception("GetObstacle: No existing obstacles for obstacleID " + obstacleID);
            }
        } else {
            throw new System.Exception("GetObstacle: No existing obstacles for shapeID " + shapeID);
        }
    }

    public List<RoomObjectCell> GetRoomCells(int stage, RoomData data) {
        int size = data.size;
        int index = data.index;
        MazeDirection rotation = data.rotation;
        GameStageObjects currentStageObjects = firstStageObjects;

        if(size < currentStageObjects.PredefinedRoomsBySize.Count) {
            return currentStageObjects.PredefinedRoomsBySize[size - 1].GetRoom(index, rotation);
        } else {
            throw new System.Exception("GetRoomCells: No rooms of size " + size + ".");
        }
    }

    public RoomObjectCell GetRoomCell(int stage, RoomData data, MazeCoords offset) {
        int size = data.size;
        int index = data.index;
        MazeDirection rotation = data.rotation;
        GameStageObjects currentStageObjects = firstStageObjects;

        if (size <= currentStageObjects.PredefinedRoomsBySize.Count) {
            return currentStageObjects.PredefinedRoomsBySize[size - 1].GetRoomCell(index, rotation, offset);
        } else {
            //throw new System.Exception("GetRoomCells: No rooms of size " + size + ".");
            // Debug.Log("(!) GetRoomCell: No room of index " + index + ".Returning null\n");
            return null;
        }
    }

    public GameObject GetPickUpItem(int stage, int itemID, ItemRarity itemRarity) {
        GameStageObjects currentStageObjects = firstStageObjects;
        return currentStageObjects.pickUpItems.GetPickUpItem(itemID, itemRarity);
    }

    private void Awake() {
        if (instance != null && instance != this) {
            Debug.LogError("Duplicate instance of ObjectDatabase.\n");
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }
}

[System.Serializable]
public class GameStageObjects {
    // Architecture
    public List<GameObject> floors;
    public List<GameObject> neWalls;
    public List<GameObject> swWalls;
    public List<GameObject> twoFacecorners;
    public List<GameObject> oneFacecorners;
    public List<GameObject> noFacecorners;
    public List<GameObject> outerPadding;
    public List<GameObject> innerPadding;
    // Rooms
    public List<RoomsOfSizeN> PredefinedRoomsBySize;
    // Obstacles
    public List<ObstacleObjectsByShape> obstacleObjectsByShape;
    // PickUp Items
    public PickUpsSO pickUpItems;

    public int GetWallsPoolSize(MazeDirection direction) {
        if(direction == MazeDirection.North ||
            direction == MazeDirection.East) {
            return neWalls.Count;
        }
        return swWalls.Count;
    }

    public int GetCornersPoolSize(int nrOfFaces) {
        if (nrOfFaces == 2) {
            return twoFacecorners.Count;
        }
        if (nrOfFaces == 1) {
            return oneFacecorners.Count;
        }
        return noFacecorners.Count;
    }

    public int GetPaddingPoolSize(bool isOuterPadding) {
        if(isOuterPadding) {
            return outerPadding.Count;
        }
        return innerPadding.Count;
    }
}

[System.Serializable]
public class RoomsOfSizeN {
    public List<RoomObject> roomsByIndex;

    public List<RoomObjectCell> GetRoom(int index, MazeDirection rotation) {
        if(roomsByIndex.Count > index) {
            return roomsByIndex[index].GetRotation(rotation);
        } else {
            // throw new System.Exception("GetRoom: Bad room index!");
            // Debug.Log("(!) GetRoomCell: No room of index " + index + ".Returning null\n");
            return null;
        }
    }

    public RoomObjectCell GetRoomCell(int index, MazeDirection rotation, MazeCoords offset) {
        if (roomsByIndex.Count > index) {
            return roomsByIndex[index].GetCellFromRotation(rotation, offset);
        } else {
            // throw new System.Exception("GetRoomCell: Bad room index!");
            // Debug.Log("(!) GetRoomCell: No room of index " + index + ". Returning null\n");
            return null;
        }
    }
}

[System.Serializable]
public class ObstacleObjectsByShape {
    public List<ObstacleObject> obstacleObjectsOfShapeID;
}

public enum ObjectType {
    Floor,
    NEWall, SWWall,
    TwoFaceCorner, OneFaceCorner, NoFaceCorner,
    OuterPadding, InnerPadding,
    Obstacle,
    Item
}
