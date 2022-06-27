using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObstacleShapes {
    //   The below list contains pairs of shapeID's and shapes.
    //   A shape is an association between a connected group of cells and a
    // specific order in which they are traversed.
    //   Shapes are stored as graphs, where each node stores the direction
    // the player chose last and the remaining cells of the shape.
    // Eg.: 
    //     [x]
    //     [S][E] , traversed from top to bottom right, is stored as
    //     
    //     [ -1 ] - [ South ] - [ East ], where -1 means no direction.

    private static readonly Dictionary<int, ObstacleShape> shapes =
        new Dictionary<int, ObstacleShape> {
            // [.]
            {1, new ObstacleShape(-1, new List<ObstacleShape>())},
            // [.]
            // [S]
            {2, new ObstacleShape(-1, new List<ObstacleShape>{ 
                    (new ObstacleShape((int)MazeDirection.South, null))
                })
            },
            // [.][E]
            {3, new ObstacleShape(-1, new List<ObstacleShape>{
                    (new ObstacleShape((int)MazeDirection.East, null))
                }) 
            },
            // [.][E]
            //    [S]
            {4, new ObstacleShape(-1, new List<ObstacleShape>{
                    (new ObstacleShape((int)MazeDirection.East, new List<ObstacleShape> {
                        (new ObstacleShape((int)MazeDirection.South, null))
                    }))
                })
            },
            //    [N]
            // [.][E]
            //    [S]
            {5, new ObstacleShape(-1, new List<ObstacleShape> {
                    (new ObstacleShape((int)MazeDirection.East, new List<ObstacleShape> {
                        (new ObstacleShape((int)MazeDirection.North, null)),
                        (new ObstacleShape((int)MazeDirection.South, null))
                    }))
                }) 
            }
        };

}

public class ObstacleShape {
    // direction is stored as integer to allow the representation of a
    // 'no direction' case as -1.
    int dirRelativeToParent;
    List<ObstacleShape> restOfShape;
    // TODO
    // Pruning list - any failure in mapping this shape will automatically
    // exclude these other shapes
    // List<int> // ID's of shapes to be pruned

    public ObstacleShape(int dir, List<ObstacleShape> restOfShape) {
        this.dirRelativeToParent = dir;
        this.restOfShape = restOfShape;
    }

    public int getDirRelativeToParent() {
        return dirRelativeToParent;
    }

    public List<ObstacleShape> getRestOfShape() {
        return restOfShape;
    }
}
