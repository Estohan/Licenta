using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *      Scriptable object that stores the contents of a room cell.
 */
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
