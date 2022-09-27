using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *      Obstacle specific component.
 */
public class Obstacle : MonoBehaviour {
    ObstacleData obstacleData;

    public Obstacle(ObstacleData obstacleData) {
        this.obstacleData = obstacleData;
    }
}
