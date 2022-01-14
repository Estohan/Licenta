using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(menuName = "Obstacle Object")]
public class ObstacleObject : MonoBehaviour {

    public int mappingSize; // number of cells that the trap covers
    public int mappingIndex; // layout shape index
    public int mappingRotation; // layout shape rotation
    [Space]
    public ObstacleType type;
    public int dangerLevel; // 0 - mild, 1 - serious or 2 - severe

    // public GameObject _prefab;
    /*public List<ObstacleTrigger> obstacleTriggers;
    public List<Obstacle> obstacles;*/

    public enum ObstacleType {
        artificial,
        natural
    }
}
