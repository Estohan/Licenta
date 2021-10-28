using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    public LevelGenerator _LevelGenerator;
    public GameObject player;

    private LevelGenerator levelGenerator;
    private Level currentLevel;

    // TEST
    public int sizeZ;
    public int sizeX;
    public int outerPaddingDiv;
    public int innerPaddingDiv;
        

    private void Awake() {
        if(instance != null && instance != this) {
            Debug.LogError("Duplicate instance of GameManager.\n");
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Instantiate the level generator
        levelGenerator = Instantiate(_LevelGenerator, new Vector3(0, 0, 0), Quaternion.identity);
        levelGenerator.transform.position.Set(0f, 0f, 0f);

        // Generate level
        currentLevel = levelGenerator.GenerateLevel(sizeZ, sizeX, outerPaddingDiv, innerPaddingDiv);

        // Instantiate level
        levelGenerator.InstantiateLevel();

        // Move player to the start position
        MazeCoords startCellPos = currentLevel.startCellPos;
        Transform targetCellTransform = currentLevel.cellsObjects[startCellPos.z, startCellPos.x].transform;
        float playerPosZ = targetCellTransform.position.z;
        float playerPosX = targetCellTransform.position.x;

        player.transform.position = new Vector3(playerPosX,
                                                0f + player.transform.localScale.y / 2,
                                                playerPosZ);
    }

    public Level getCurrentLevel() {
        return currentLevel;
    }
}
