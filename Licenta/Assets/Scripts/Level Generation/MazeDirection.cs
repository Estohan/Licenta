using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MazeDirection {
    North,
    East,
    South,
    West
}

public static class MazeDirections {
    public const int Count = 4;

    private static MazeDirection[] oppositeDirection = {
        MazeDirection.South,
        MazeDirection.West,
        MazeDirection.North,
        MazeDirection.East
    };

    private static MazeCoords[] directionVectors = {
        new MazeCoords(-1, 0),
        new MazeCoords(0, 1),
        new MazeCoords(1, 0),
        new MazeCoords(0, -1)
    };

    public static MazeDirection RandomDirection {
        get {
            return (MazeDirection) UnityEngine.Random.Range(0, Count);
        }
    }

    public static MazeDirection GetOppositeDirection(this MazeDirection direction) {
        return oppositeDirection[(int)direction];
    }

    public static MazeCoords ToMazeCoords(this MazeDirection direction) {
        return directionVectors[(int)direction];
    }
}
