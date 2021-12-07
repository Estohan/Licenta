using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Room")]
public class Room : ScriptableObject {
    public int roomSize;
    public int roomIndex;
    [SerializeField]
    public List<RoomCell> roomCells = new List<RoomCell>();

    
}

[SerializeField]
public class RoomCell {
    public MazeCoords anchorOffset;
}