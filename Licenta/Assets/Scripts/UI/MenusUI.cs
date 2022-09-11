using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenusUI : MonoBehaviour {
    [SerializeField]
    private GameObject mainMenuUI;
    [SerializeField]
    private GameObject pauseMenuUI;
    [SerializeField]
    private GameObject helpMenuUI;
    [SerializeField]
    private GameObject confirmQuitUI;
    [SerializeField]
    private GameObject confirmReturnUI;
    [SerializeField]
    private GameObject deathScreenUI;
    [SerializeField]
    private GameObject endGameScreenUI;
    [Space]
    [SerializeField]
    private GameObject healthBar;

    private bool gameIsPaused;
    private bool inMainMenu;
    private bool escapeKeyAvailable;

    private void Start() {
        gameIsPaused = true;
        inMainMenu = true;
        escapeKeyAvailable = false;

        mainMenuUI.SetActive(true);
        InGameUI.MinimapWindow.Hide();

        GameManager.inputManager.PlayerMovement.Disable();
        GameManager.inputManager.UI.Disable();
        GameManager.inputManager.Others.Disable();

        GameManager.inputManager.Others.PauseGame.started += _ => MenuButtonEscape();
        // GameManager.inputManager.Others.RestartGame.started += _ => Test_RestartGame();

        // GameEventSystem.instance.OnPlayerDeath += PlayerDeathReaction;
    }

    /*public void Test_RestartGame() {
        GameManager.instance.currentLevelIndex++;
        GameManager.instance.RestartGame();
    }*/

    // ------------------------------------------------------------------------
    // Pause menu functions
    // ------------------------------------------------------------------------

    public void MenuButtonEscape() {
        if (escapeKeyAvailable) {
            if (gameIsPaused) {
                // Unpause
                pauseMenuUI.SetActive(false);
                helpMenuUI.SetActive(false);
                confirmReturnUI.SetActive(false);
                gameIsPaused = false;
                Time.timeScale = 1f;
                GameManager.inputManager.PlayerMovement.Enable();
                GameManager.inputManager.UI.Enable();
            } else {
                // Pause
                pauseMenuUI.SetActive(true);
                gameIsPaused = true;
                Time.timeScale = 0f;
                GameManager.inputManager.PlayerMovement.Disable();
                GameManager.inputManager.UI.Disable();
            }
        }
    }

    public void MenuButtonMainMenu() {
        pauseMenuUI.SetActive(false);
        confirmReturnUI.SetActive(true);
    }

    public void MenuButtonMainMenuYes() {
        // UI
        confirmReturnUI.SetActive(false);
        mainMenuUI.SetActive(true);
        InGameUI.MinimapWindow.Hide();
        healthBar.SetActive(false);
        inMainMenu = true;
        escapeKeyAvailable = false;
        // restart game
        GameManager.instance.currentLevelIndex = 1;
        GameManager.instance.RestartGame();
        // Pause the game
        //Time.timeScale = 0f;
    }

    public void MenuButtonMainMenuNo() {
        confirmReturnUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    // ------------------------------------------------------------------------
    // Main menu functions
    // ------------------------------------------------------------------------
    public void MenuButtonPlay() {
        // UI
        mainMenuUI.SetActive(false);
        InGameUI.MinimapWindow.Show();
        healthBar.SetActive(true);
        gameIsPaused = false;
        inMainMenu = false;
        escapeKeyAvailable = true;
        // Time.timeScale = 1f;
        // GameManager.instance.StartLevel();
        StartCoroutine(DelayedLevelStartCoroutine());
    }

    public void MenuButtonQuit() {
        mainMenuUI.SetActive(false);
        confirmQuitUI.SetActive(true);
    }

    public void MenuButtonQuitYes() {
        Application.Quit();
    }

    public void MenuButtonQuitNo() {
        confirmQuitUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    // ------------------------------------------------------------------------
    // Death menu functions
    // ------------------------------------------------------------------------

    public void DisplayDeathScreenUI() {
        // If player did not press escape while death animation was still playing
        if (!gameIsPaused) {
            deathScreenUI.SetActive(true);
            escapeKeyAvailable = false;
            gameIsPaused = true;
            GameManager.inputManager.PlayerMovement.Disable();
            GameManager.inputManager.UI.Disable();
        }
    }

    public void MenuButtonRetryYes() {
        deathScreenUI.SetActive(false);
        escapeKeyAvailable = true;
        gameIsPaused = false;
        // restart game
        GameManager.instance.currentLevelIndex = 1;
        GameManager.instance.RestartGame();
        StartCoroutine(DelayedLevelStartCoroutine());
    }

    public void MenuButtonRetryNo() {
        deathScreenUI.SetActive(false);
        mainMenuUI.SetActive(true);
        inMainMenu = true;
        InGameUI.MinimapWindow.Hide();
        healthBar.SetActive(false);
        // restart game
        GameManager.instance.currentLevelIndex = 1;
        GameManager.instance.RestartGame();
        // Pause the game
        // Time.timeScale = 0f;
    }

    // ------------------------------------------------------------------------
    // End game menu functions
    // ------------------------------------------------------------------------

    public void DisplayEndGameScreen() {
        endGameScreenUI.SetActive(true);
        escapeKeyAvailable = false;
        gameIsPaused = true;
        GameManager.inputManager.PlayerMovement.Disable();
        GameManager.inputManager.UI.Disable();
    }

    public void MenuButtonDirectMainMenu() {
        // UI
        mainMenuUI.SetActive(true);
        inMainMenu = true;
        escapeKeyAvailable = false;
        endGameScreenUI.SetActive(false);
        InGameUI.MinimapWindow.Hide();
        healthBar.SetActive(false);
        GameManager.instance.currentLevelIndex = 1;
        // restart game
        GameManager.instance.RestartGame();
    }

    // ------------------------------------------------------------------------
    // Other menu functions
    // ------------------------------------------------------------------------

    public void MenuButtonHelpBack() {
        helpMenuUI.SetActive(false);
        if (inMainMenu) {
            // Back to main menu
            mainMenuUI.SetActive(true);
        } else {
            // Back to pause menu
            pauseMenuUI.SetActive(true);
        }
    }

    public void MenuButtonHelp() {
        helpMenuUI.SetActive(true);
        if (inMainMenu) {
            // Accessed from main menu
            mainMenuUI.SetActive(false);
        } else {
            // Accessed from pause menu
            pauseMenuUI.SetActive(false);
        }
    }

    // Delay StartLevel() method execution to ensure
    // ReadyLevel() coroutines are over
    private IEnumerator DelayedLevelStartCoroutine() {
        yield return new WaitForSecondsRealtime(1.2f);
        GameManager.instance.StartLevel();
    }

    public bool isGamePaused() {
        return gameIsPaused;
    }

    public bool isMainMenuActive() {
        return inMainMenu;
    }

    /*public void PlayerDeathReaction(object sender) {
        deathScreenUI.SetActive(true);
    }*/

    private void OnEnable() {
        /*GameManager.inputManager.PlayerMovement.Enable();
        GameManager.inputManager.UI.Enable();
        GameManager.inputManager.Others.Enable();*/
    }

    private void OnDisable() {
        GameManager.inputManager.PlayerMovement.Disable();
        GameManager.inputManager.UI.Disable();
        GameManager.inputManager.Others.Disable();
    }
}
