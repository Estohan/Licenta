using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Obstacle SO")]
public class ObstacleObject : ScriptableObject {

    // TODO
    /*public Obstacle obstacle_North;
    public Obstacle obstacle_East;
    public Obstacle obstacle_South;
    public Obstacle obstacle_West;*/
    public GameObject obstacle;

    // TODO
    // difficulty objects
    // 0
    // North
    // East
    // West
    // South

    // 1
    // North
    // East
    // West
    // South

    // 2
    // North
    // East
    // West
    // South

    // 3
    // North
    // East
    // West
    // South


    [Space]
    public int difficulty; // 0 - tutorial, 1 - mild, 2 - serious or 3 - difficult
    [Space]
    public float damage;
    public ObstacleState state;
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

    // TODO
    /*public Obstacle GetObstacle(MazeDirection rotation) {
        switch(rotation) {
            case MazeDirection.North:
                return obstacle_North;
            case MazeDirection.East:
                return obstacle_East;
            case MazeDirection.South:
                return obstacle_South;
            default:
                return obstacle_West;
        }
    }*/

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

