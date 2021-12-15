using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RoomObjectCell")]
public class RoomObjectCell : ScriptableObject {
    public MazeCoords offset;

    public GameObject _floor;

    public GameObject _northWall;
    public GameObject _eastWall;
    public GameObject _southWall;
    public GameObject _westWall;

    public GameObject _NWcorner;
    public GameObject _NEcorner;
    public GameObject _SEcorner;
    public GameObject _SWcorner;

    // [TRY] List of GameObjects instead
    public GameObject _subsection_1;
    public MazeDirection _subsection_1_rot;
    public GameObject _subsection_2;
    public MazeDirection _subsection_2_rot;
    public GameObject _subsection_3;
    public MazeDirection _subsection_3_rot;

    public bool HasFloor() {
        return _floor != null;
    }

    public GameObject GetWall(MazeDirection direction) {
        switch(direction) {
            case MazeDirection.North:
                return _northWall;
            case MazeDirection.East:
                return _eastWall;
            case MazeDirection.South:
                return _southWall;
            default:
                return _westWall;
        }
    }

    public bool HasWall(MazeDirection direction) {
        switch (direction) {
            case MazeDirection.North:
                return _northWall != null;
            case MazeDirection.East:
                return _eastWall != null;
            case MazeDirection.South:
                return _southWall != null;
            default:
                return _westWall != null;
        }
    }

    public GameObject GetCorner(int index) {
        switch (index) {
            case 0:
                return _NWcorner;
            case 1:
                return _NEcorner;
            case 2:
                return _SEcorner;
            default:
                return _SWcorner;
        }
    }

    public bool HasCorner(int index) {
        switch (index) {
            case 0:
                return _NWcorner != null;
            case 1:
                return _NEcorner != null;
            case 2:
                return _SEcorner != null;
            default:
                return _SWcorner != null;
        }
    }

    // [TODO] Treat unassigned reference exceptions
    public List<(GameObject, MazeDirection)> GetSubsectionObj() {
        List < (GameObject, MazeDirection) > subsectionObjects = new List<(GameObject, MazeDirection)>();

        subsectionObjects.Add((_subsection_1, _subsection_1_rot));
        subsectionObjects.Add((_subsection_2, _subsection_2_rot));
        subsectionObjects.Add((_subsection_3, _subsection_3_rot));

        return subsectionObjects;
    }
}
