using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Obstacle SO")]
public class ObstacleObject : ScriptableObject {

    /*    public int mappingSize; // number of cells that the trap covers
        public int mappingIndex; // layout shape index
        public int mappingRotation; // layout shape rotation*/
    /*public Obstacle obstacle_North;
    public Obstacle obstacle_East;
    public Obstacle obstacle_South;
    public Obstacle obstacle_West;*/
    public GameObject obstacle;

    [Space]
    public ObstacleType type;
    public int dangerLevel; // 0 - mild, 1 - serious or 2 - severe
    [Space]
    public float damage;
    public ObstacleState state;
    [Space]
    // list of tuples (offset, object) where object is a subsection to delete
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

    public enum ObstacleType {
        artificial,
        natural
    }

    [System.Serializable]
    public struct ObstObjDeletionEntry {
        public MazeCoords offset;
        public int subsection;
    }
}

[System.Serializable]
public class ObstacleData {
    // data about the obstacle to be saved when saving the game
}

