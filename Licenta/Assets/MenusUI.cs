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
    }

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
        inMainMenu = true;
        escapeKeyAvailable = false;
        // restart game
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
        gameIsPaused = false;
        inMainMenu = false;
        escapeKeyAvailable = true;
        // Time.timeScale = 1f;
        GameManager.instance.Test2();
        GameManager.inputManager.Others.Enable();
        GameManager.inputManager.PlayerMovement.Enable();
        GameManager.inputManager.UI.Enable();
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
        deathScreenUI.SetActive(true);
        escapeKeyAvailable = false;
        gameIsPaused = true;
        GameManager.inputManager.PlayerMovement.Disable();
        GameManager.inputManager.UI.Disable();
    }

    public void MenuButtonRetryYes() {
        deathScreenUI.SetActive(false);
        // restart game
        GameManager.instance.RestartGame();
    }

    public void MenuButtonRetryNo() {
        deathScreenUI.SetActive(false);
        mainMenuUI.SetActive(true);
        inMainMenu = true;
        InGameUI.MinimapWindow.Hide();
        // restart game
        GameManager.instance.RestartGame();
        // Pause the game
        // Time.timeScale = 0f;
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
