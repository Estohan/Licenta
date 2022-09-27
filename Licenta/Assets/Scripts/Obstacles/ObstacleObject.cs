using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *      Scriptable object that holds the data for all levels of
 *  difficulty (and their rotations) of an obstacle type.
 */
[CreateAssetMenu(menuName = "Obstacle SO")]
public class ObstacleObject : ScriptableObject {
    [Header("Lvl. 0 Difficulty - Tutorial")]
    public GameObject northRotation_0;
    public GameObject eastRotation_0;
    public GameObject southRotation_0;
    public GameObject westRotation_0;

    // 1
    // North
    // East
    // West
    // South
    [Header("Lvl. 1 Difficulty - Easy")]
    public GameObject northRotation_1;
    public GameObject eastRotation_1;
    public GameObject southRotation_1;
    public GameObject westRotation_1;

    // 2
    // North
    // East
    // West
    // South
    [Header("Lvl. 2 Difficulty - Medium")]
    public GameObject northRotation_2;
    public GameObject eastRotation_2;
    public GameObject southRotation_2;
    public GameObject westRotation_2;

    // 3
    // North
    // East
    // West
    // South
    [Header("Lvl. 3 Difficulty - Difficult")]
    public GameObject northRotation_3;
    public GameObject eastRotation_3;
    public GameObject southRotation_3;
    public GameObject westRotation_3;


    /*[Space]
    public int difficulty; // 0 - tutorial, 1 - mild, 2 - serious or 3 - difficult*/
    [Space]
    // list of tuples (offset, object), where object is a subsection to delete.
    // these deleted objects will be replaced by the obstacle
    public List<ObstObjDeletionEntry> deletionListNorth;
    public List<ObstObjDeletionEntry> deletionListEast;
    public List<ObstObjDeletionEntry> deletionListSouth;
    public List<ObstObjDeletionEntry> deletionListWest;

    public List<ObstObjDeletionEntry> getDeletionEntries(MazeDirection rotation) {
        switch(rotation) {
            case MazeDirection.North:
                return deletionListNorth;
            case MazeDirection.East:
                return deletionListEast;
            case MazeDirection.South:
                return deletionListSouth;
            default:
                return deletionListWest;
        }
    }

    public GameObject GetObstacle(MazeDirection rotation, int difficulty) {
        switch (difficulty) {
            case 1:
                if (rotation == MazeDirection.North) return northRotation_1;
                if (rotation == MazeDirection.East) return eastRotation_1;
                if (rotation == MazeDirection.South) return southRotation_1;
                return westRotation_1;
            case 2:
                if (rotation == MazeDirection.North) return northRotation_2;
                if (rotation == MazeDirection.East) return eastRotation_2;
                if (rotation == MazeDirection.South) return southRotation_2;
                return westRotation_2;
            case 3:
                if (rotation == MazeDirection.North) return northRotation_3;
                if (rotation == MazeDirection.East) return eastRotation_3;
                if (rotation == MazeDirection.South) return southRotation_3;
                return westRotation_3;
            default: // 0
                if (rotation == MazeDirection.North) return northRotation_0;
                if (rotation == MazeDirection.East) return eastRotation_0;
                if (rotation == MazeDirection.South) return southRotation_0;
                return westRotation_0;
        }
    }

    [System.Serializable]
    public struct ObstObjDeletionEntry {
        public MazeCoords offset;
        public int subsection;
    }
}

[System.Serializable]
public class ObstacleData {
    /* everything the level generator needs to know about the obstacle
     * to correctly instantiate and restore it to the state it was in
     * when the game was saved */

    // data needed to fetch the obstacle
    int stage;
    int shapeID;
    int obstacleID;
    // int obstacleDifficulty;

    // state of the obstacle
    ObstacleState obstacleState;
}

