using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {
    ObstacleData obstacleData;

    public Obstacle(ObstacleData obstacleData) {
        this.obstacleData = obstacleData;
    }
}
