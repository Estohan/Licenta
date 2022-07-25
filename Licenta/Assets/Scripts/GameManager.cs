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
    // ignored if greater than 20%
    public int outerPaddingPercentage;
    // ignored if greater than 20%
    public int innerPaddingPercentage;
    public int sectorsNumber;
    [Space]
    public int difficulty;
    public int chanceOfObstDowngrade;
    public int chanceOfObstUpgrade;
        

    private void Awake() {
        if(instance != null && instance != this) {
            Debug.LogError("Duplicate instance of GameManager.\n");
            Destroy(gameObject);
        } else {
            instance = this;
        }

        SaveSystem.InitSaveSystem();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Instantiate the level generator
        levelGenerator = Instantiate(_LevelGenerator, new Vector3(0, 0, 0), Quaternion.identity);
        levelGenerator.transform.position.Set(0f, 0f, 0f);

        // Generate a new level
        StartGame();
    }

    public void StartGame() {
        // Generate level
        LevelGenerator.LayoutRequirements layoutRec = 
            new LevelGenerator.LayoutRequirements(sizeZ, sizeX, outerPaddingPercentage, innerPaddingPercentage, sectorsNumber);
        layoutRec.stage = 1;
        layoutRec.difficulty = difficulty;
        layoutRec.chanceOfObstDowngrade = chanceOfObstDowngrade;
        layoutRec.chanceOfObstUpgrade = chanceOfObstUpgrade;
        currentLevel = levelGenerator.GenerateLevel(layoutRec);

        // Instantiate level
        levelGenerator.InstantiateLevel();

        // Ready level
        ReadyLevel();
    }

    public void RestartGame() {
        if(levelGenerator.transform.GetChild(0) != null) {
            // Destroy previous level
            Destroy(levelGenerator.transform.GetChild(0).gameObject);
            // Create a new one
            StartGame();
            Debug.Log("Game Restarted.");
        }
    }

    public void LoadGame() {
        SavedData savedData = SaveSystem.LoadGame();
        if(savedData != null) {
            if (levelGenerator.transform.GetChild(0) != null) {
                // Destroy previous level
                Destroy(levelGenerator.transform.GetChild(0).gameObject);
                // Create a new one based on the saved data
                currentLevel = levelGenerator.GenerateLevelFromSave(savedData);
                // Instantiate level
                levelGenerator.InstantiateLevel();
                // Ready level
                ReadyLevel();
            }
        } else {
            Debug.LogError("GameManager: No existing save data found!");
        }
    }

    public void SaveGame() {
        SaveSystem.SaveGame();
    }

    // Final preparations before the level is playable
    public void ReadyLevel() {
        // Move player to the start position
        MazeCoords startCellPos = currentLevel.stats.startCell;
        Debug.Log("Start cell is " + startCellPos);
        Transform targetCellTransform = currentLevel.cellsObjects[startCellPos.z, startCellPos.x].transform;
        float playerPosZ = targetCellTransform.position.z;
        float playerPosX = targetCellTransform.position.x;
        Debug.Log("Cell transform: " + currentLevel.cellsObjects[startCellPos.z, startCellPos.x].transform.position);

        player.transform.position = new Vector3(playerPosX,
                                                0f + player.transform.localScale.y / 2,
                                                playerPosZ);
        Debug.Log("Player transform: " + player.transform.position);
    }

    public Level getCurrentLevel() {
        return currentLevel;
    }
}
