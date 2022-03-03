using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Obstacle SO")]
public class ObstacleSO : ScriptableObject {

    /*    public int mappingSize; // number of cells that the trap covers
        public int mappingIndex; // layout shape index
        public int mappingRotation; // layout shape rotation*/
    public Obstacle obstacle_North;
    public Obstacle obstacle_East;
    public Obstacle obstacle_South;
    public Obstacle obstacle_West;

    [Space]
    public ObstacleType type;
    public int dangerLevel; // 0 - mild, 1 - serious or 2 - severe
    [Space]
    public float damage;
    public ObstacleState state;

    public Obstacle GetObstacle(MazeDirection rotation) {
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
    }

    public enum ObstacleType {
        artificial,
        natural
    }
}

[System.Serializable]
public class ObstacleData {
    // data about the obstacle to be saved when saving the game
}

/*public enum SideEffects {
    Confusion,
    Amnesia,
    Fatigue,
    Dizziness,
    Blindness,
    Poison,
    Bleed,
    Sleep
}
*/