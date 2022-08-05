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

    // Works only with Vector3.forward, Vector3.back, Vector3.right and Vector3.left
    public static Vector3 ToVector3(this MazeDirection direction) {
        switch (direction) {
            case MazeDirection.North:
                return Vector3.forward;
            case MazeDirection.East:
                return Vector3.right;
            case MazeDirection.South:
                return Vector3.back;
            default: // West
                return Vector3.left;
        }
    }

    /* Returns the direction of the destination cell relative to the source cell*/
    public static MazeDirection DirFromPairOfCoords(MazeCoords source, MazeCoords destination) {
        int z_diff = destination.z - source.z;
        int x_diff = destination.x - source.x;
        /*          (-1,  0)
         * ( 0, -1) ( 0,  0) ( 0,  1)
         *          ( 1,  0)
         * North: (0,  0) - (-1,  0) = ( 1,  0)
         * East:  (0,  0) - ( 0,  1) = ( 0, -1)
         * South: (0,  0) - ( 1,  0) = (-1,  0)
         * West:  (0,  0) - ( 0, -1) = ( 0,  1)
         */
        if (z_diff == 0) {
            if (x_diff == 1) {
                return MazeDirection.West;
            } else {
                return MazeDirection.East;
            }
        } else {
            if (z_diff == -1) {
                return MazeDirection.South;
            }
        }

        return MazeDirection.North;
    }
}
