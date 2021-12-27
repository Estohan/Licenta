using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneViewBuilder : MonoBehaviour {
    public LevelGenerator levelGenerator;
    public GameObject _floor;
    public GameObject _wall;
    private Level currentLevel;
    [Space]
    [Space]
    public int mazeSizeX;
    public int mazeSizeZ;
    [Space]
    public int sectorCount;
    [Space]
    public int outerPaddingVal;
    public int innerPaddingVal;


    public void GenerateNewLevel() {
        currentLevel = levelGenerator.GenerateLevel(mazeSizeZ, mazeSizeX, outerPaddingVal, outerPaddingVal, sectorCount);
        // levelGenerator.InstantiateLevel();

    }
}
