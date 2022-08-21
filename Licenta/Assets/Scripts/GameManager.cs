using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    // Gameobjects
    public LevelGenerator _LevelGenerator;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private CameraControl mainCamera;
    [SerializeField]
    private GameObject dropShuttle;
    private GameObject _instantiatedDropShuttle;

    // Level data
    private LevelGenerator levelGenerator;
    private Level currentLevel;
    private Queue<LevelEffectsManager.LevelEffects> levelEffectsQueue;

    // TEST
    public int sizeZ;
    public int sizeX;
    // ignored if greater than 20%
    public int outerPaddingPercentage;
    // ignored if greater than 20%
    public int innerPaddingPercentage;
    public int sectorsNumber;
    public int difficulty;
    public int chanceOfObstDowngrade;
    public int chanceOfObstUpgrade;

    public static InputManager inputManager;


    private void Awake() {
        if(instance != null && instance != this) {
            Debug.LogError("Duplicate instance of GameManager.\n");
            Destroy(gameObject);
        } else {
            instance = this;
        }

        levelEffectsQueue = new Queue<LevelEffectsManager.LevelEffects>();

        SaveSystem.InitSaveSystem();

        inputManager = new InputManager();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Instantiate the level generator
        levelGenerator = Instantiate(_LevelGenerator, new Vector3(0, 0, 0), Quaternion.identity);
        levelGenerator.transform.position.Set(0f, 0f, 0f);

        // Generate a new level
        CreateLevel();
        // Ready level
        ReadyFirstLevel();
    }

    public void CreateLevel() {
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
    }

    public void RestartGame() {
        if(levelGenerator.transform.GetChild(0) != null) {
            // Destroy previous level
            Destroy(levelGenerator.transform.GetChild(0).gameObject);
            // Create a new one
            CreateLevel();
            // Ready level
            ReadyLevel();
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

        // Move camera to player position
        mainCamera.SnapToPlayerPosition();

        //GameEventSystem.instance.PlayerStatsChanged();
    }

    public void ReadyFirstLevel() {
        // Starting positions offset so that the player is seen better when camera is zoomed in
        float startPositionOffset = 0.5f;
        // Move player to the start position
        MazeCoords startCellPos = currentLevel.stats.startCell;
        Transform targetCellTransform = currentLevel.cellsObjects[startCellPos.z, startCellPos.x].transform;
        float playerPosZ = targetCellTransform.position.z;
        float playerPosX = targetCellTransform.position.x;

        _instantiatedDropShuttle = Instantiate(dropShuttle, levelGenerator.gameObject.transform);
        _instantiatedDropShuttle.transform.position = new Vector3(playerPosX + startPositionOffset - 0.1f,
                                                                    0f, // + player.transform.localScale.y ,
                                                                    playerPosZ + startPositionOffset - 0.1f);

        player.transform.position = new Vector3(playerPosX + startPositionOffset + 0.1f,
                                                0f + player.transform.localScale.y, // / 2
                                                playerPosZ + startPositionOffset + 0.1f);

        // Move camera to player position
        mainCamera.SnapToPlayerPosition();
        mainCamera.PrepareZoomOutEffect();
        player.transform.rotation = Quaternion.Euler(0f, 225f, 0f); // rotate the player towards the camera
        player.GetComponent<PlayerAnimationHandler>().StandUp();
        //GameEventSystem.instance.PlayerStatsChanged();
        StartCoroutine(WaitForPlayerAnimationToStart());
        Debug.Log("Camera position:" + mainCamera.transform.position);
    }

    public void AddEffectToQueue(LevelEffectsManager.LevelEffects levelEffect) {
        levelEffectsQueue.Enqueue(levelEffect);
    }

    public void ExecuteLevelEffect(LevelEffectsManager.LevelEffects levelEffect) {
        LevelEffectsManager.ExecuteEffect(levelEffect);
    }


   /* public void Test() {
        StartCoroutine(PrepareStartGameScene());
    }*/

    public void Test2() {
        // Unpause the game
        Time.timeScale = 1f;
        mainCamera.ZoomOutEffect();
        _instantiatedDropShuttle.GetComponent<Animator>().SetTrigger("OpenShuttle");
        StartCoroutine(WaitForShuttleToOpen());
    }

    // Let the game run half a second to make sure the player animation started
    // and it is crouched inside the shuttle
    private IEnumerator WaitForPlayerAnimationToStart() {
        yield return new WaitForSeconds(0.5f);
        // Pause the game
        Time.timeScale = 0f;
    }

    // Wait for the shuttle to open enough so that the player can resume its stand up
    // animation
    private IEnumerator WaitForShuttleToOpen() {
        yield return new WaitForSeconds(2f);
        player.GetComponent<PlayerAnimationHandler>().AnimationResume();
    }

    public Level getCurrentLevel() {
        return currentLevel;
    }    
}
