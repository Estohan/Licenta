using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants {
    
    public const float cellSize = 5f;
    // horizontally or vertically, from one side to the other, the size of the cell
    // can be divided in five segments: two for walls and three for subsections
    public const float wallWidth = 1.15f;
    public const float subsectionSize = 0.9f;

    public const int maxDifficulty = 3;
}
