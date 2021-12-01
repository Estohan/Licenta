using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RoomLayouts {
    // The room layouts and their corresponding rotations
    public static List<Room>[] rooms = {
        // ============================================================
        // 1-cell rooms
        // ============================================================
        new List<Room> {
            // Simplest room, the anchor itself is the whole room
            // A
            new Room(new List<(int, int)> {(0, 0)}, // South orientation (default)
                    new List<(int, int)> {(0, 0)}, // West orientation
                    new List<(int, int)> {(0, 0)}, // North orientation
                    new List<(int, int)> {(0, 0)}) // East orientation
        },

        // ============================================================
        // 2-cells rooms
        // ============================================================
        new List<Room> {
            // Access point on the side 2-cells room
            // XA
            new Room(new List<(int, int)> {(0, 0), (0, -1)},
                    new List<(int, int)> {(0, 0), (-1, 0)}, // West orientation
                    new List<(int, int)> {(0, 0), (0, 1)}, // North orientation
                    new List<(int, int)> {(0, 0), (1, 0)}), // East orientation

            // Access point at the end 2-cells room
            // X
            // A
            new Room(new List<(int, int)> {(0, 0), (-1, 0)},
                    new List<(int, int)> {(0, 0), (0, 1)}, // West orientation
                    new List<(int, int)> {(0, 0), (1, 0)}, // North orientation
                    new List<(int, int)> {(0, 0), (0, -1)}) // East orientation
        },

        // ============================================================
        // 3-cells rooms
        // ============================================================
        new List<Room> {
            // 3-cell corner, access point at one end
            // XX
            //  A
            new Room(new List<(int, int)> {(0, 0), (-1, 0), (-1, -1)}, // South orientation (default)
                    new List<(int, int)> {(0, 0), (0, 1), (-1, 1)}, // West orientation
                    new List<(int, int)> {(0, 0), (1, 0), (1, 1)}, // North orientation
                    new List<(int, int)> {(0, 0), (0, -1), (1, -1)}), // East orientation

            // 3-cell corner, access point on the inside
            // XA
            // X
            new Room(new List<(int, int)> {(0, 0), (0, -1), (1, -1)}, // South orientation (default)
                    new List<(int, int)> {(0, 0), (-1, 0), (-1, -1)}, // West orientation
                    new List<(int, int)> {(0, 0), (0, 1), (-1, 1)}, // North orientation
                    new List<(int, int)> {(0, 0), (1, 0), (1, 1)}) // East orientation
        },
        // ============================================================
        // 4-cells rooms
        // ============================================================
        new List<Room> {
            // 4-cell sqare
            // XX
            // XA
            new Room(new List<(int, int)> {(0, 0), (-1, 0), (-1, -1), (0, -1)}, // South orientation (default)
                    new List<(int, int)> {(0, 0), (0, 1), (-1, 1), (-1, 0)}, // West orientation
                    new List<(int, int)> {(0, 0), (1, 0), (1, 1), (0, 1)}, // North orientation
                    new List<(int, int)> {(0, 0), (0, -1), (1, -1), (1, 0)}) // East orientation
        }
    };

    // DEBUG: Do I need this?
    // Return the rotations of a random room of MAXIMUM size n
    public static List<(int, int)>[] GetRandomRoomOfMaxSizeN(int n) {
        if (n < rooms.Length && rooms[n].Count > 0) {
            int roomSize = UnityEngine.Random.Range(0, n);
            int index = UnityEngine.Random.Range(0, rooms[roomSize].Count);
            return rooms[roomSize][index].cellsRelativeToAnchor;
        }
        throw new System.Exception("RoomLayouts: GetRandomRoomOFMaxSizeN(" + n + ")");
    }

    // DEBUG: Do I need this?
    // Return the rotations of a random room of size n
    public static List<(int, int)>[] GetRandomRoomOfSizeN(int n) {
        if (n < rooms.Length && rooms[n].Count > 0) {
            int index = UnityEngine.Random.Range(0, rooms[n].Count);
            return rooms[n][index].cellsRelativeToAnchor;
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

public class Room {
    public List<(int, int)>[] cellsRelativeToAnchor;
    //public List<int> list;
    //public int DEBUG_NUMBER;

    public Room(List<(int, int)> southOrientation,
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
}

public struct RoomData {
    public MazeCoords anchor;
    public MazeDirection rotation;

    public RoomData(MazeCoords anchor, MazeDirection rotation) {
        this.anchor = anchor;
        this.rotation = rotation;
    }
}
