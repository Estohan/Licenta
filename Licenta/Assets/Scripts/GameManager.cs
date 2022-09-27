using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *      Game Manager that ties in together level generation, level completion
 *  and game menus.
 */
public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    // Gameobjects
    [SerializeField]
    private LevelsConfigurations levelConfigsSO;
    [SerializeField]
    private LevelGenerator levelGenerator;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private CameraControl mainCamera;
    [SerializeField]
    private GameObject dropShuttle;
    private GameObject _instantiatedDropShuttle;
    [SerializeField]
    private GameObject artifact;
    private GameObject _instantiatedArtifact;

    // Level data
    private LevelGenerator _levelGenerator;
    private Level currentLevel;
    private Queue<LevelEffectsManager.LevelEffects> levelEffectsQueue;
    public int currentLevelIndex;

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
            // Debug.LogError("Duplicate instance of GameManager.\n");
            Destroy(gameObject);
        } else {
            instance = this;
        }

        levelEffectsQueue = new Queue<LevelEffectsManager.LevelEffects>();

        // SaveSystem.InitSaveSystem();

        inputManager = new InputManager();
    }

    // Start is called before the first frame update
    void Start() {
        // Deactivate player so that it does not interact with the level yet
        player.SetActive(false);

        // Instantiate the level generator
        _levelGenerator = Instantiate(levelGenerator, new Vector3(0, 0, 0), Quaternion.identity);
        _levelGenerator.transform.position.Set(0f, 0f, 0f);

        GameEventSystem.instance.OnPlayerDeath += PlayerDeathReaction;

        currentLevelIndex = 1;
        // Generate a new level
        CreateLevel();
        // Ready level
        ReadyLevel();
    }

    // TEST Start
    // Start is called before the first frame update
    /*void Start() {
        inputManager.PlayerMovement.Enable();
        inputManager.UI.Disable();
        inputManager.Others.Disable();
    }*/

    public void CreateLevel() {

        // Destroy previous level if it exists
        if (_levelGenerator.transform.childCount > 0) {
            Destroy(_levelGenerator.transform.GetChild(0).gameObject);
        }

        currentLevel = _levelGenerator.GenerateLevel(levelConfigsSO.levelsConfigs[currentLevelIndex - 1]);
        // Instantiate level
        _levelGenerator.InstantiateLevel();
    }

    public void RestartGame() {

        // Deactivte player to ensure it does not interact with the new level
        player.SetActive(false);

        // If _levelGenerator has any children, destroy them
        foreach (Transform child in _levelGenerator.transform) {
            GameObject.Destroy(child.gameObject);
        }

        // Disable all controlls
        inputManager.PlayerMovement.Disable();
        inputManager.UI.Disable();
        inputManager.Others.Disable();

        // Clear notifications and notification queues
        InGameUI.UINotifications.instance.StopAndDiscardNotifications();

        CreateLevel();
        ReadyLevel();
        if (! this.transform.GetComponent<MenusUI>().isMainMenuActive()) {
            StartLevel();
        }
    }


    public void ReadyLevel() {
        // Move player to the start position
        MazeCoords startCellPos = currentLevel.stats.startCell;
        MazeCoords finishCellPos = currentLevel.stats.finishCell;
        Transform targetCellTransform = currentLevel.cellsObjects[startCellPos.z, startCellPos.x].transform;
        float playerPosZ = targetCellTransform.position.z;
        float playerPosX = targetCellTransform.position.x;

        player.SetActive(false);
        player.transform.position = new Vector3(playerPosX,
                                                0f + player.transform.localScale.y / 2,
                                                playerPosZ);
        // Activate player object AFTER moving it, so that it is moved correctly
        // (otherwise Update() function PlayerControls.cs messes up things)
        player.SetActive(true);

        // Create artifact at finish position
        targetCellTransform = currentLevel.cellsObjects[finishCellPos.z, finishCellPos.x].transform;
        _instantiatedArtifact = Instantiate(artifact, _levelGenerator.gameObject.transform);
        _instantiatedArtifact.transform.position = new Vector3(targetCellTransform.position.x,
                                                                0f,
                                                                targetCellTransform.position.z);
        // Move camera to player position
        mainCamera.SnapToPlayerPosition();

        // Level specific preparations
        switch(currentLevelIndex) {
            /*
            LEVEL 1
            */
            case 1:
                // Clear effects queue
                levelEffectsQueue.Clear();
                // Player and shuttle starting position offset
                float startPositionOffset = 0.5f;
                // Instantiate and position drop shuttle
                _instantiatedDropShuttle = Instantiate(dropShuttle, _levelGenerator.gameObject.transform);
                _instantiatedDropShuttle.transform.position = new Vector3(playerPosX + startPositionOffset,
                                                                    0f,
                                                                    playerPosZ + startPositionOffset);
                // Move player adding the above offset
                player.transform.position = new Vector3(player.transform.position.x + startPositionOffset + 0.2f,
                                                player.transform.position.y,
                                                player.transform.position.z + startPositionOffset + 0.2f);
                // Rotate the player towards the camera
                player.transform.rotation = Quaternion.Euler(0f, 225f, 0f);
                // Start player's stand up animation
                player.GetComponent<PlayerAnimationHandler>().StandUp();
                // Wait for the animation to start and then pause the game
                Time.timeScale = 1f;
                // Prepare slower zoom effect for first level
                mainCamera.PrepareZoomOutEffect(0.75f);
                StartCoroutine(WaitForPlayerAnimationToStart());
                break;
            default:
                // Prepare faster zoom effect for the other levels
                mainCamera.PrepareZoomOutEffect(1.5f);
                // Reveal the starting cell and its neighbours
                currentLevel.cellsObjects[startCellPos.z, startCellPos.x].GetOccluder().RevealNeighbours();
                break;
        }

        //GameEventSystem.instance.PlayerStatsChanged();
    }

    public void StartLevel() {
        switch (currentLevelIndex) {
            /*
            LEVEL 1
            */
            case 1:
                // Unpause game
                Time.timeScale = 1f;
                // Camera zoom out effect
                mainCamera.ZoomOutEffect();
                // Update player's health
                player.GetComponent<PlayerStats>().maxHealth = 75f;
                player.GetComponent<PlayerStats>().currHealth = 75f;
                player.GetComponent<PlayerStats>().isAlive = true;
                player.GetComponent<PlayerStats>().isDamageImmune = false;
                player.GetComponent<PlayerStats>().isInactive = false;
                GameEventSystem.instance.PlayerStatsChanged();
                // Wait for the shuttle to open enough so that the player can stand up
                StartCoroutine(WaitForShuttleAnimation());
                break;
            default:
                // Camera zoom out effect
                mainCamera.ZoomOutEffect();
                // Display level
                InGameUI.UINotifications.instance.DisplayNotification("Level " + currentLevelIndex, true);
                // Activate controls
                inputManager.Others.Enable();
                inputManager.PlayerMovement.Enable();
                inputManager.UI.Enable();
                // Apply effects in queue
                StartCoroutine(WaitAndExecuteEffects());
                break;
        }
        // Debug.Log("Player transform: " + player.transform.position);
    }

    public void EndLevel() {
        // Display notification
        InGameUI.UINotifications.instance.DisplayNotification("Level " + currentLevelIndex + " complete.", true);
        // Disable all controlls
        inputManager.PlayerMovement.Disable();
        inputManager.UI.Disable();
        inputManager.Others.Disable();
        // Move to next level (or end game screen)
        StartCoroutine(WaitThenPassToNextLevel(2f));
    }

    public IEnumerator WaitAndExecuteEffects() {
        if (levelEffectsQueue.Count > 0) {
            // Since only existing effects are memory mapping one
            InGameUI.UINotifications.instance.DisplayNotification("Mapping aquired memory...");
            yield return new WaitForSeconds(5f);
            ExecuteEffectsInQueue();
        }
    }

    public IEnumerator WaitThenPassToNextLevel(float seconds) {
        yield return new WaitForSeconds(seconds);

        if (currentLevelIndex == 10) {
            // Endgame notification
            InGameUI.UINotifications.instance.DisplayNotification("Collected all artifacts!", true);
            // Endgame player animation
            player.GetComponent<PlayerAnimationHandler>().Cheer();
            // Endgame screen
            //StartCoroutine(WaitThenShowEndGameScreen(5f));
            yield return new WaitForSeconds(seconds);
            this.gameObject.GetComponent<MenusUI>().DisplayEndGameScreen();
            // Load the first level
            currentLevelIndex = 1;
        } else {
            currentLevelIndex++;/*
            inputManager.Others.Enable();
            inputManager.UI.Enable();
            inputManager.PlayerMovement.Enable();*/
            RestartGame();
        }
    }

    // Let the game run half a second to make sure the player animation started
    // and it is crouched inside the shuttle
    private IEnumerator WaitForPlayerAnimationToStart() {
        yield return new WaitForSeconds(1f);
        // Open shuttle
        _instantiatedDropShuttle.GetComponent<Animator>().SetTrigger("OpenShuttle");
        // Pause the game
        Time.timeScale = 0f;
    }

    // Wait for the shuttle to open enough so that the player can resume its stand up
    // animation
    private IEnumerator WaitForShuttleAnimation() {
        yield return new WaitForSeconds(2f);
        player.GetComponent<PlayerAnimationHandler>().AnimationResume();
        // Reveal the starting cell and its neighbours
        MazeCoords startCellPos = currentLevel.stats.startCell;
        currentLevel.cellsObjects[startCellPos.z, startCellPos.x].GetOccluder().RevealNeighbours();
        // Also wait for players stand up animation before activating the controls and displaying notification
        yield return new WaitForSeconds(4f);
        inputManager.Others.Enable();
        inputManager.PlayerMovement.Enable();
        inputManager.UI.Enable();
        InGameUI.UINotifications.instance.DisplayNotification("Level 1", true);
    }

    // Wait for the death animation to play and then show retry menu
    private IEnumerator WaitAfterDeathAnimation() {
        yield return new WaitForSeconds(4f);
        this.transform.GetComponent<MenusUI>().DisplayDeathScreenUI();
    }

    public void PlayerDeathReaction(object sender) {
        //inputManager.PlayerMovement.Disable();
        // Disable all controlls
        inputManager.PlayerMovement.Disable();
        inputManager.UI.Disable();
        inputManager.Others.Disable();
        currentLevelIndex = 1;
        StartCoroutine(WaitAfterDeathAnimation());
    }

    public Level getCurrentLevel() {
        return currentLevel;
    }


    public void ReadyFirstLevel() {
        // Starting positions offset so that the player is seen better when camera is zoomed in
        float startPositionOffset = 0.5f;
        // Move player to the start position
        MazeCoords startCellPos = currentLevel.stats.startCell;
        Transform targetCellTransform = currentLevel.cellsObjects[startCellPos.z, startCellPos.x].transform;
        float playerPosZ = targetCellTransform.position.z;
        float playerPosX = targetCellTransform.position.x;

        _instantiatedDropShuttle = Instantiate(dropShuttle, _levelGenerator.gameObject.transform);
        _instantiatedDropShuttle.transform.position = new Vector3(playerPosX + startPositionOffset - 0.1f,
                                                                    0f, // + player.transform.localScale.y ,
                                                                    playerPosZ + startPositionOffset - 0.1f);

        player.transform.position = new Vector3(playerPosX + startPositionOffset + 0.1f,
                                                0f + player.transform.localScale.y, // / 2
                                                playerPosZ + startPositionOffset + 0.1f);

        // Move camera to player position
        mainCamera.SnapToPlayerPosition();
        mainCamera.PrepareZoomOutEffect(0.75f);
        player.transform.rotation = Quaternion.Euler(0f, 225f, 0f); // rotate the player towards the camera
        player.GetComponent<PlayerAnimationHandler>().StandUp();
        //GameEventSystem.instance.PlayerStatsChanged();
        StartCoroutine(WaitForPlayerAnimationToStart());
    }

    public void AddEffectToQueue(LevelEffectsManager.LevelEffects levelEffect) {
        levelEffectsQueue.Enqueue(levelEffect);
    }

    public void ExecuteLevelEffect(LevelEffectsManager.LevelEffects levelEffect) {
        LevelEffectsManager.ExecuteEffect(levelEffect);
    }

    public void ExecuteEffectsInQueue() {
        while (levelEffectsQueue.Count > 0) {
            ExecuteLevelEffect(levelEffectsQueue.Dequeue());
        }
    }
}
