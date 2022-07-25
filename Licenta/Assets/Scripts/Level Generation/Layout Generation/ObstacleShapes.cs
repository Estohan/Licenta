using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObstacleShapes {

    public static readonly Dictionary<int, ObstacleShape> shapes =
        new Dictionary<int, ObstacleShape> {
            // 0: x
            {0, new ObstacleShape(
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {0, 0, 0, 0})
                   }, // North rotation
                   null, // East rotation (None)
                   null, // South rotation (None)
                   null, // West rotation (None)
                   null, // North pruning (None)
                   null, // East pruning (None)
                   null, // South pruning (None)
                   null // West pruning (None)
            )},
            // 1: _    
            //    x  x|  x  |x
            //           `
            /*{1, new ObstacleShape(
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {1, 0, 0, 0})
                   }, // North rotation
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {0, 1, 0, 0})
                   }, // East rotation (None)
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {0, 0, 1, 0})
                   }, // South rotation (None)
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {0, 0, 0, 1})
                   }, // West rotation (None)
                   null, // North pruning (None)
                   null, // East pruning (None)
                   null, // South pruning (None)
                   null // West pruning (None)
            )},*/
            // 2: _       _
            //    x  |x|  x  |x|
            //    `       `
            /*{2, new ObstacleShape(
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {1, 0, 1, 0})
                   }, // North rotation
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {0, 1, 0, 1})
                   }, // East rotation (None)
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {1, 0, 1, 0})
                   }, // South rotation (None)
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {0, 1, 0, 1})
                   }, // West rotation (None)
                   null, // North pruning (None)
                   null, // East pruning (None)
                   null, // South pruning (None)
                   null // West pruning (None)
            )},*/
            // 3: _            _
            //    x|  x|  |x  |x
            //        `    ` 
            /*{3, new ObstacleShape(
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {1, 1, 0, 0})
                   }, // North rotation
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {0, 1, 1, 0})
                   }, // East rotation (None)
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {0, 0, 1, 1})
                   }, // South rotation (None)
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {1, 0, 0, 1})
                   }, // West rotation (None)
                   null, // North pruning (None)
                   null, // East pruning (None)
                   null, // South pruning (None)
                   null // West pruning (None)
            )},*/
            // 4:  _   _         _
            //    |x|  x|  |x|  |x
            //         `    `    `
            /*{4, new ObstacleShape(
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {1, 1, 0, 1})
                   }, // North rotation
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {1, 1, 1, 0})
                   }, // East rotation (None)
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {0, 1, 1, 1})
                   }, // South rotation (None)
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {1, 0, 1, 1})
                   }, // West rotation (None)
                   null, // North pruning (None)
                   null, // East pruning (None)
                   null, // South pruning (None)
                   null // West pruning (None)
            )},*/
            // 5: |x|  _ _  | |  _ _
            //    | |    x  |x|  x  
            //         ` `       ` `
            {5, new ObstacleShape(
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {0, 1, -1, 1}),
                       (1, 0, new int[] {-1, 1, 0, 1})
                   }, // North rotation
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {1, 0, 1, -1}),
                       (0, -1, new int[] {1, -1, 1, 0})
                   }, // East rotation (None)
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {-1, 1, 0, 1}),
                       (-1, 0, new int[] {0, 1, -1, 1})
                   }, // South rotation (None)
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {1, -1, 1, 0}),
                       (0, 1, new int[] {1, 0, 1, -1})
                   }, // West rotation (None)
                   null, // North pruning (None)
                   null, // East pruning (None)
                   null, // South pruning (None)
                   null // West pruning (None)
            )},
            // 6: |x|  _ _ _  | |  _ _ _
            //    | |      x  | |  x    
            //    | |  ` ` `  |x|  ` ` `
            {6, new ObstacleShape(
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {0, 1, -1, 1}),
                       (1, 0, new int[] {-1, 1, -1, 1}),
                       (2, 0, new int[] {-1, 1, 0, 1})
                   }, // North rotation
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {1, 0, 1, -1}),
                       (0, -1, new int[] {1, -1, 1, -1}),
                       (0, -2, new int[] {1, -1, 1, 0})
                   }, // East rotation (None)
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {-1, 1, 0, 1}),
                       (-1, 0, new int[] {-1, 1, -1, 1}),
                       (-2, 0, new int[] {0, 1, -1, 1})
                   }, // South rotation (None)
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {1, -1, 1, 0}),
                       (0, 1, new int[] {1, -1, 1, -1}),
                       (0, 2, new int[] {1, 0, 1, -1})
                   }, // West rotation (None)
                   null, // North pruning (None)
                   null, // East pruning (None)
                   null, // South pruning (None)
                   null // West pruning (None)
            )},
            //     _
            // 7: |x|   _ _ _ _ _   | |  _ _ _ _ _
            //    | |           x|  | | |x          
            //    | |   ` ` ` ` `   | |  ` ` ` ` `
            //    | |               | |
            //    | |               |x|
            //                       `
            {7, new ObstacleShape(
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {1, 0, -1, 0}),
                       (1, 0, new int[] {-1, 0, -1, 0}),
                       (2, 0, new int[] {-1, 0, -1, 0}),
                       (3, 0, new int[] {-1, 0, -1, 0}),
                       (4, 0, new int[] {-1, 0, 0, 0})
                   }, // North rotation
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {0, 1, 0, -1}),
                       (0, -1, new int[] {0, -1, 0, -1}),
                       (0, -2, new int[] {0, -1, 0, -1}),
                       (0, -3, new int[] {0, -1, 0, -1}),
                       (0, -4, new int[] {0, -1, 0, 0})
                   }, // East rotation (None)
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {-1, 0, 1, 0}),
                       (-1, 0, new int[] {-1, 0, -1, 0}),
                       (-2, 0, new int[] {-1, 0, -1, 0}),
                       (-3, 0, new int[] {-1, 0, -1, 0}),
                       (-4, 0, new int[] {0, 0, -1, 0})
                   }, // South rotation (None)
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {0, -1, 0, 1}),
                       (0, 1, new int[] {0, -1, 0, -1}),
                       (0, 2, new int[] {0, -1, 0, -1}),
                       (0, 3, new int[] {0, -1, 0, -1}),
                       (0, 4, new int[] {0, 0, 0, -1})
                   }, // West rotation (None)
                   null, // North pruning (None)
                   null, // East pruning (None)
                   null, // South pruning (None)
                   null // West pruning (None)
            )},
            //     _ _    _ _         _ _
            // 8: |   |  |  _  | |x|  x  |
            //    |x| |  |  x  |   |  `  |
            //            ` `   ` `   ` `
            //     _ _   _ _          _ _
            // 8: |   |  x  |  | |x| |  _
            //    |x| |  `  |  |   | |  x
            //           ` `    ` `   ` `
            {8, new ObstacleShape(
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {-1, 1, 0, 0}),
                       (-1, 0, new int[] {1, -1, -1, 1}),
                       (-1, 1, new int[] {1, 1, -1, -1}),
                       (0, 1, new int[] {-1, 0, 0, 1})
                   }, // North rotation
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {0, -1, 1, 0}),
                       (0, 1, new int[] {1, 1, -1, -1}),
                       (1, 1, new int[] {-1, 1, 1, -1}),
                       (1, 0, new int[] {1, -1, 0, 0})
                   }, // East rotation (None)
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {0, 0, -1, 1}),
                       (1, 0, new int[] {-1, 1, 1, -1}),
                       (1, -1, new int[] {-1, -1, 1, 1}),
                       (0, -1, new int[] {0, 1, -1, 0})
                   }, // South rotation (None)
                   new List<(int, int, int[])> {
                       (0, 0, new int[] {1, 0, 0, -1}),
                       (0, -1, new int[] {-1, -1, 1, 1}),
                       (-1, -1, new int[] {1, -1, -1, 1}),
                       (-1, 0, new int[] {0, 0, 1, -1})
                   }, // West rotation (None)
                   null, // North pruning (None)
                   null, // East pruning (None)
                   null, // South pruning (None)
                   null // West pruning (None)
            )}
        };

    public static int GetNrDifferentSizes() {
        List<int> differentSizes = new List<int>();
        foreach(KeyValuePair<int, ObstacleShape> entry in shapes) {
            // If a new size is found, add it to the list of different sizes
            if(!differentSizes.Contains(entry.Value.size)) {
                differentSizes.Add(entry.Value.size);
            }
        }
        return differentSizes.Count;
    }








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

    /*private static readonly Dictionary<int, ObstacleShape> shapes =
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
        };*/

}

public class ObstacleShape {
    public int size;
    public List<(int, int, int[])>[] cellsRelativeToAnchor;
    // pairs (index, rotation) of shapes that are to be automatically
    // excluded from being checked if current direction cannot be mapped
    public List<(int, int)>[] prunedShapes;

    public ObstacleShape(List<(int, int, int[])> northRotation,
                         List<(int, int, int[])> eastRotation,
                         List<(int, int, int[])> southRotation,
                         List<(int, int, int[])> westRotation,
                         List<(int, int)> prunedByNorth,
                         List<(int, int)> prunedByEast,
                         List<(int, int)> prunedBySouth,
                         List<(int, int)> prunedByWest) {
        cellsRelativeToAnchor = new List<(int, int, int[])>[4];
        cellsRelativeToAnchor[0] = northRotation;
        cellsRelativeToAnchor[1] = eastRotation;
        cellsRelativeToAnchor[2] = southRotation;
        cellsRelativeToAnchor[3] = westRotation;
        prunedShapes = new List<(int, int)>[4];
        prunedShapes[0] = prunedByNorth;
        prunedShapes[1] = prunedByEast;
        prunedShapes[2] = prunedBySouth;
        prunedShapes[3] = prunedByWest;
        // search for an existing rotation and get its size
        for(int i = 0; i < 4; i ++) {
            if(cellsRelativeToAnchor[i].Count != 0) {
                size = cellsRelativeToAnchor[i].Count;
                break;
            }
        }
    }
}

/*
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
*/