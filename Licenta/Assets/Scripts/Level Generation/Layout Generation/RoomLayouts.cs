using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *      Representations of rooms that will be mapped on the maze layout.
 */
public static class RoomLayouts {
    // The room layouts and their corresponding rotations
    public static List<RoomLayout>[] rooms = {
        // ============================================================
        // 1-cell rooms
        // ============================================================
        new List<RoomLayout> {
            // Simplest room, the anchor itself is the whole room
            // A
            new RoomLayout(new List<(int, int)> {(0, 0)}, // North orientation
                    new List<(int, int)> {(0, 0)}, // East orientation
                    new List<(int, int)> {(0, 0)}, // South orientation
                    new List<(int, int)> {(0, 0)}) // West orientation
        },

        // ============================================================
        // 2-cells rooms
        // ============================================================
        new List<RoomLayout> {
            // Access point on the side 2-cells room
            // AX
            new RoomLayout(new List<(int, int)> {(0, 0), (0, 1)}, // North orientation
                    new List<(int, int)> {(0, 0), (1, 0)}, // East orientation
                    new List<(int, int)> {(0, 0), (0, -1)}, // South orientation
                    new List<(int, int)> {(0, 0), (-1, 0)}), // West orientation

            // Access point at the end 2-cells room
            // A
            // X
            new RoomLayout(new List<(int, int)> {(0, 0), (1, 0)}, // North orientation
                    new List<(int, int)> {(0, 0), (0, -1)}, // East orientation
                    new List<(int, int)> {(0, 0), (-1, 0)}, // South orientation
                    new List<(int, int)> {(0, 0), (0, 1)}), // West orientation
        },

        // ============================================================
        // 3-cells rooms
        // ============================================================
        new List<RoomLayout> {
            // 3-cell corner, access point at one end
            // A
            // XX
            new RoomLayout(new List<(int, int)> {(0, 0), (1, 0), (1, 1)}, // North orientation
                    new List<(int, int)> {(0, 0), (0, -1), (1, -1)}, // East orientation
                    new List<(int, int)> {(0, 0), (-1, 0), (-1, -1)}, // South orientation (default)
                    new List<(int, int)> {(0, 0), (0, 1), (-1, 1)}), // West orientation

            // 3-cell corner, access point on the inside
            //  X
            // AX
            new RoomLayout(
                    new List<(int, int)> {(0, 0), (0, 1), (-1, 1)}, // North orientation
                    new List<(int, int)> {(0, 0), (1, 0), (1, 1)}, // East orientation
                    new List<(int, int)> {(0, 0), (0, -1), (1, -1)}, // South orientation (default)
                    new List<(int, int)> {(0, 0), (-1, 0), (-1, -1)}) // West orientation
        },
        // ============================================================
        // 4-cells rooms
        // ============================================================
        new List<RoomLayout> {
            // 4-cell sqare
            // AX
            // XX
            new RoomLayout(new List<(int, int)> {(0, 0), (1, 0), (1, 1), (0, 1)}, // North orientation
                    new List<(int, int)> {(0, 0), (0, -1), (1, -1), (1, 0)}, // East orientation
                    new List<(int, int)> {(0, 0), (-1, 0), (-1, -1), (0, -1)}, // South orientation (default)
                    new List<(int, int)> {(0, 0), (0, 1), (-1, 1), (-1, 0)}) // West orientation
        }
    };

    // Return the rotations of a random room of MAXIMUM size n
    public static (List<(int, int)>[], int roomSize, int roomIndex) GetRandomRoomOfMaxSizeN(int n) {
        if (n <= rooms.Length && n > 0) {
            int roomSize = UnityEngine.Random.Range(0, n - 1);
            int roomIndex = UnityEngine.Random.Range(0, rooms[roomSize].Count);
            return (rooms[roomSize][roomIndex].cellsRelativeToAnchor, roomSize + 1, roomIndex);
        }
        throw new System.Exception("RoomLayouts: GetRandomRoomOFMaxSizeN(" + n + ")");
    }

    // Return the rotations of a random room of size n
    public static (List<(int, int)>[], int roomSize, int roomIndex) GetRandomRoomOfSizeN(int n) {
        if (n <= rooms.Length && n > 0 && rooms[n - 1].Count > 0) { // !!!
            int roomIndex = UnityEngine.Random.Range(0, rooms[n - 1].Count);
            return (rooms[n - 1][roomIndex].cellsRelativeToAnchor, n, roomIndex);
        }
        throw new System.Exception("RoomLayouts: GetRandomRoomOFSizeN(" + n + ")");
    }
}

public class RoomLayout {
    public List<(int, int)>[] cellsRelativeToAnchor;

    public RoomLayout(List<(int, int)> southOrientation,
                List<(int, int)> westOrientation,
                List<(int, int)> northOrientation,
                List<(int, int)> eastOrientation) {
        // Four rotations
        cellsRelativeToAnchor = new List<(int, int)>[4];
        cellsRelativeToAnchor[0] = southOrientation;
        cellsRelativeToAnchor[1] = westOrientation;
        cellsRelativeToAnchor[2] = northOrientation;
        cellsRelativeToAnchor[3] = eastOrientation;
    }

    public List<(int, int)> GetRotation(MazeDirection dir) {
        return cellsRelativeToAnchor[(int)dir];
    }
}

[System.Serializable]
public class RoomData {
    public MazeCoords anchor;
    public MazeDirection rotation;
    public int size;
    public int index;
    public int itemID;
    public ItemRarity itemRarity;

    public RoomData() {
        // Empty constructor
    }

    public RoomData(MazeCoords anchor, MazeDirection rotation, int size, int index) {
        this.anchor = anchor;
        this.rotation = rotation;
        this.size = size;
        this.index = index;
        this.itemID = -1;
    }

    public override string ToString() {
        return "{" + anchor + "," + rotation + "," + size + "," + index + "}";
    }
}
