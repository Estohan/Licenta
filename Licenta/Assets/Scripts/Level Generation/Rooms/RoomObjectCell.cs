using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RoomObjectCell")]
public class RoomObjectCell : ScriptableObject {
    public MazeCoords offset;

    public GameObject _floor;
    [Space]
    [Header("Walls")]
    public GameObject _northWall;
    public GameObject _eastWall;
    public GameObject _southWall;
    public GameObject _westWall;
    [Space]
    [Header("Corners")]
    public GameObject _NWcorner;
    public GameObject _NEcorner;
    public GameObject _SEcorner;
    public GameObject _SWcorner;
    /*[Space]
    [Header("Subsections")]*/
    // [TRY] List of GameObjects instead
    /*public GameObject _subsection_1;
    public MazeDirection _subsection_1_rot;
    public GameObject _subsection_2;
    public MazeDirection _subsection_2_rot;
    public GameObject _subsection_3;
    public MazeDirection _subsection_3_rot;*/
    [Space]
    [Header("Room interior (Anchor only!)")]
    public GameObject interiorObject;
    [Space]
    [Header("PickUp Items arrangements (Anchor only!)")]
    public PickUpItemArrangement defaultArrangement;
    public List<PickUpItemArrangement> arrangements;

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

    public GameObject GetInterior() {
        return interiorObject;
    }

    public bool HasInterior() {
        return interiorObject != null;
    }

    public PickUpItemArrangement GetDefaultItemArrangement() {
        if (defaultArrangement != null) {
            return defaultArrangement;
        } else {
            return new PickUpItemArrangement(new Vector3(0f, 0f, 0f));
        }
    }

    public List<PickUpItemArrangement> GetSpecialItemArrangements() {
        return arrangements;
    }

    public bool HasSpecialItemArrangements() {
        return arrangements == null;
    }

    // [TODO] Treat unassigned reference exceptions
    /*public List<(GameObject, MazeDirection)> GetSubsectionObj() {
        List < (GameObject, MazeDirection) > subsectionObjects = new List<(GameObject, MazeDirection)>();

        subsectionObjects.Add((_subsection_1, _subsection_1_rot));
        subsectionObjects.Add((_subsection_2, _subsection_2_rot));
        subsectionObjects.Add((_subsection_3, _subsection_3_rot));

        return subsectionObjects;
    }*/
}

[System.Serializable]
public class PickUpItemArrangement {
    public ItemRarity targetedItemRarity;
    public int targetedItemID;

    public Vector3 itemPosRelativeToAnchor;

    public bool hasArrangement;
    public Vector3 arrangementLocation;
    public Vector3 arrancementRotation;
    public GameObject arrangement;

    public PickUpItemArrangement(Vector3 position) {
        this.itemPosRelativeToAnchor = position;
    }
}
