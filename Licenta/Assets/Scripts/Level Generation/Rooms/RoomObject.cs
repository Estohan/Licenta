using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RoomObject")]
public class RoomObject : ScriptableObject {
    /*public int roomSize;
    public int roomIndex;*/

    // Rotations
    public List<RoomObjectCell> northRotation = new List<RoomObjectCell>();
    public List<RoomObjectCell> eastRotation = new List<RoomObjectCell>();
    public List<RoomObjectCell> southRotation = new List<RoomObjectCell>();
    public List<RoomObjectCell> westRotation = new List<RoomObjectCell>();

    public List<RoomObjectCell> GetRotation(MazeDirection rotation) {
        switch (rotation) {
            case MazeDirection.North:
                return northRotation;
            case MazeDirection.East:
                return eastRotation;
            case MazeDirection.South:
                return southRotation;
            default:
                return westRotation;
        }
    }

    public RoomObjectCell GetCellFromRotation(MazeDirection rotation, MazeCoords cellOffset) {
        List<RoomObjectCell> rotationCells;
        switch(rotation) {
            case MazeDirection.North:
                rotationCells = northRotation;
                break;
            case MazeDirection.East:
                rotationCells = eastRotation;
                break;
            case MazeDirection.South:
                rotationCells = southRotation;
                break;
            default:
                rotationCells = westRotation;
                break;
        }

        foreach(RoomObjectCell roomCell in rotationCells) {
            // [TODO] What's up with "==" and ".Equals"
            // Debug.Log("Comparing " + roomCell.offset + " with " + cellOffset);
            if(roomCell.offset.z == cellOffset.z && roomCell.offset.x == cellOffset.x) {
                return roomCell;
            }
        }

        return null;
    }
}
