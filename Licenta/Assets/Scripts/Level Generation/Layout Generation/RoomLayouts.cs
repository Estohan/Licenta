using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RoomLayouts {
    // The room layouts and their corresponding rotations
    public static List<RoomLayout>[] rooms = {
        // ============================================================
        // 1-cell rooms
        // ============================================================
        new List<RoomLayout> {
            // Simplest room, the anchor itself is the whole room
            // A
            new RoomLayout(new List<(int, int)> {(0, 0)}, // South orientation (default)
                    new List<(int, int)> {(0, 0)}, // West orientation
                    new List<(int, int)> {(0, 0)}, // North orientation
                    new List<(int, int)> {(0, 0)}) // East orientation
        },

        // ============================================================
        // 2-cells rooms
        // ============================================================
        new List<RoomLayout> {
            // Access point on the side 2-cells room
            // XA
            new RoomLayout(new List<(int, int)> {(0, 0), (0, 1)}, // North orientation
                    new List<(int, int)> {(0, 0), (1, 0)}, // East orientation
                    new List<(int, int)> {(0, 0), (-1, 0)}, // West orientation
                    new List<(int, int)> {(0, 0), (0, -1)}),

            // Access point at the end 2-cells room
            // X
            // A
            new RoomLayout(new List<(int, int)> {(0, 0), (1, 0)}, // North orientation
                    new List<(int, int)> {(0, 0), (0, -1)}, // East orientation
                    new List<(int, int)> {(0, 0), (0, 1)}, // West orientation
                    new List<(int, int)> {(0, 0), (-1, 0)})
        },

        // ============================================================
        // 3-cells rooms
        // ============================================================
        new List<RoomLayout> {
            // 3-cell corner, access point at one end
            // XX
            //  A
            new RoomLayout(new List<(int, int)> {(0, 0), (1, 0), (1, 1)}, // North orientation
                    new List<(int, int)> {(0, 0), (0, -1), (1, -1)}, // East orientation
                    new List<(int, int)> {(0, 0), (-1, 0), (-1, -1)}, // South orientation (default)
                    new List<(int, int)> {(0, 0), (0, 1), (-1, 1)}), // West orientation

            // 3-cell corner, access point on the inside
            // XA
            // X
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
            // XX
            // XA
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
        if (n <= rooms.Length && n > 0 && rooms[n - 1].Count > 0) { // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! [DEBUG] index out of bounds here
            int roomIndex = UnityEngine.Random.Range(0, rooms[n - 1].Count);
            return (rooms[n - 1][roomIndex].cellsRelativeToAnchor, n, roomIndex);
        }
        throw new System.Exception("RoomLayouts: GetRandomRoomOFSizeN(" + n + ")");
    }

    /*public RoomLayouts() {
        if(instance != null && instance !=  this) {
            Debug.Log("Duplicate instance of RoomLayouts");
        } else {
            instance = this;

            // Initialize rooms
            rooms = new List<Room>[4];
            for(int i = 0; i < 4; i ++) {
                rooms[i] = new List<Room>();
                rooms[i].Add(new Room());
                rooms[i].Add(new Room());
                rooms[i].Add(new Room());
                rooms[i].Add(new Room());
            }
        }
    }*/
}

public class RoomLayout {
    public List<(int, int)>[] cellsRelativeToAnchor;
    //public List<int> list;
    //public int DEBUG_NUMBER;

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
        // DEBUG_NUMBER = UnityEngine.Random.Range(0, 100);
        //this.list = list;
        //this.DEBUG_NUMBER = DEBUG_NUMBER;
    }

    public List<(int, int)> GetRotation(MazeDirection dir) {
        return cellsRelativeToAnchor[(int)dir];
    }
}

public struct RoomData {
    public MazeCoords anchor;
    public MazeDirection rotation;
    public int size;
    public int index;

    public RoomData(MazeCoords anchor, MazeDirection rotation, int size, int index) {
        this.anchor = anchor;
        this.rotation = rotation;
        this.size = size;
        this.index = index;
    }

    public override string ToString() {
        return "{" + anchor + "," + rotation + "," + size + "," + index + "}";
    }
}
