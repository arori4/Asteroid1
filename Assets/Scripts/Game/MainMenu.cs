using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;

/**
* Defines the main menu
*/
public class MainMenu : MonoBehaviour {

    [Header("Main Menu")]
    public CanvasGroup mainMenu;
    public CanvasGroup titleText;
    public CanvasGroup blackScreen;
    public CanvasGroup startLevelsButton;
    public CanvasGroup startSurvivalButton;
    public CanvasGroup startMultiplayerButton;
    public CanvasGroup goToHighScoresButton;
    AudioSource backgroundMusic;

    [Header("High Score Menu")]
    public CanvasGroup highScoreMenu;
    public CanvasGroup highScoreBack;
    public Text highScoreText;

    [Header("Survival Menu")]
    public CanvasGroup survivalMenu;
    public CanvasGroup survivalEasyButton;
    public CanvasGroup survivalMediumButton;
    public CanvasGroup survivalHardButton;
    public CanvasGroup survivalBackButton;

    [Header("Levels Menu")]
    public CanvasGroup levelsMenu;
    public CanvasGroup levelsEasyButton;
    public CanvasGroup levelsMediumButton;
    public CanvasGroup levelsHardButton;
    public CanvasGroup levelsBackButton;

    //other variables
    GameSave gameSave;
    bool startGameOnce = false;
    CanvasGroup currentMenu;

    //constants
    const float MENU_SWITCH_FADE_DURATION = 0.5f;
    const float TITLE_TEXT_FADE_DURATION = 5f;
    const float TITLE_TEXT_RISE_DURATION = 10f;
    const float START_GAME_FADE_DURATION = 1f / 0.4f;
    const int EASY_LEVEL = 1;
    const int MEDIUM_LEVEL = 10;
    const int HARD_LEVEL = 20;


    void Start() {
        //Hard set beginning values
        titleText.alpha = 0;
        blackScreen.alpha = 0;
        startLevelsButton.alpha = 0;
        startSurvivalButton.alpha = 0;
        startMultiplayerButton.alpha = 0;
        mainMenu.alpha = 1;
        highScoreMenu.alpha = 0;
        goToHighScoresButton.alpha = 0;

        //Components
        backgroundMusic = GetComponent<AudioSource>();
        gameSave = GetComponent<GameSave>();
        currentMenu = mainMenu;

        StartCoroutine(StartCoroutine());
    }

    private IEnumerator StartCoroutine() {
        StartCoroutine(FadeInCoroutine(titleText, TITLE_TEXT_FADE_DURATION));
        yield return new WaitForSeconds(1.6f);
        StartCoroutine(FadeInCoroutine(startLevelsButton, START_GAME_FADE_DURATION));
        StartCoroutine(FadeInCoroutine(startSurvivalButton, START_GAME_FADE_DURATION));
        StartCoroutine(FadeInCoroutine(startMultiplayerButton, START_GAME_FADE_DURATION));
        StartCoroutine(FadeInCoroutine(goToHighScoresButton, START_GAME_FADE_DURATION));
    }

    public void StartLevelsGame() {
        if (startGameOnce == false) {
            //Set settings
            PlayerPrefs.SetInt("Level", 1);
            PlayerPrefs.SetInt("Score", 0);
            PlayerPrefs.SetInt("Mode", 0);

            //Set opposite to alpha of 0
            startSurvivalButton.alpha = 0;

            StartCoroutine(LoadNextSceneCoroutine());
        }
    }

    public void ShowHighScoreMenu() {
        StartCoroutine(SwitchMenuCoroutine(highScoreMenu));

        //display high scores
        string text = "";
        for (int index = 0; index < 10; index++) {
            text += (index + 1) + ".  " + gameSave.highScores[index] + "\n";
        }
        highScoreText.text = text;
    }

    public void ShowMainMenu() {
        StartCoroutine(SwitchMenuCoroutine(mainMenu));
    }

    public void ShowSurvivalDifficultyMenu() {
        StartCoroutine(SwitchMenuCoroutine(survivalMenu));
    }

    public void ShowLevelsDifficultyMenu() {
        StartCoroutine(SwitchMenuCoroutine(levelsMenu));
    }

    private IEnumerator LoadNextSceneCoroutine() {
        startGameOnce = true;
        while (blackScreen.alpha < 1) {
            blackScreen.alpha += Time.deltaTime * 0.5f;
            mainMenu.alpha -= Time.deltaTime * 0.7f; //slightly faster so stars stay in background longer
            backgroundMusic.volume -= Time.deltaTime * 0.5f;
            yield return null;
        }
        SceneManager.LoadScene("Main Game");
    }

    /*
     * Start game methods
     * Most methods created for buttons
     */

    public void StartSurvivalEasy() {
        StartSurvivalGame(EASY_LEVEL, survivalEasyButton);
    }

    public void StartSurvivalMedium() {
        StartSurvivalGame(MEDIUM_LEVEL, survivalMediumButton);
    }

    public void StartSurvivalHard() {
        StartSurvivalGame(HARD_LEVEL, survivalHardButton);
    }

    private void StartSurvivalGame(int level, CanvasGroup selectedButton) {
        if (startGameOnce == false) {
            //Set settings
            PlayerPrefs.SetInt("Level", level);
            PlayerPrefs.SetInt("Score", 0);
            PlayerPrefs.SetInt("Mode", 1);

            //Make other buttons disappear
            if (survivalEasyButton != selectedButton) {
                survivalEasyButton.alpha = 0;
            }
            if (survivalMediumButton != selectedButton) {
                survivalMediumButton.alpha = 0;
            }
            if (survivalHardButton != selectedButton) {
                survivalHardButton.alpha = 0;
            }

            //Start next level
            StartCoroutine(LoadNextSceneCoroutine());
        }
    }

    public void StartLevelsEasy() {
        StartLevelsGame(EASY_LEVEL, levelsEasyButton);
    }

    public void StartLevelsMedium() {
        StartLevelsGame(MEDIUM_LEVEL, levelsMediumButton);
    }

    public void StartLevelsHard() {
        StartLevelsGame(HARD_LEVEL, levelsHardButton);
    }

    private void StartLevelsGame(int level, CanvasGroup selectedButton) {
        if (startGameOnce == false) {
            //Set settings
            PlayerPrefs.SetInt("Level", level);
            PlayerPrefs.SetInt("Score", 0);
            PlayerPrefs.SetInt("Mode", 0);

            //Make other buttons disappear
            if (levelsEasyButton != selectedButton) {
                levelsEasyButton.alpha = 0;
            }
            if (levelsMediumButton != selectedButton) {
                levelsMediumButton.alpha = 0;
            }
            if (levelsHardButton != selectedButton) {
                levelsHardButton.alpha = 0;
            }

            //Start next level
            StartCoroutine(LoadNextSceneCoroutine());
        }
    }


    /*
    * Helper fading coroutines
    */

    private IEnumerator FadeInCoroutine(CanvasGroup canvas, float duration) {
        float lambda = 1f / duration;

        while (canvas.alpha < 1f) {
            canvas.alpha += Time.deltaTime * lambda;
            yield return null;
        }

        //lock at final value
        canvas.alpha = 1f;
    }

    private IEnumerator FadeOutCoroutine(CanvasGroup canvas, float duration) {
        float lambda = 1f / duration;

        while (canvas.alpha > 0) {
            canvas.alpha -= Time.deltaTime * lambda;
            yield return null;
        }

        //lock at final value
        canvas.alpha = 0;
    }
    
    private IEnumerator SwitchMenuCoroutine(CanvasGroup target) {
        //debug checks
        if (currentMenu.alpha < 1f) {
            Debug.Log("MainMenu SwitchMenuCoroutine: current canvas is not fully opaque");
        }
        if (target.alpha > 0f) {
            Debug.Log("MainMenu SwitchMenuCoroutine: target canvas is not starting fully transparent");
        }

        //Fade out and fade in the target menus, and set interaction settings
        StartCoroutine(FadeOutCoroutine(currentMenu, MENU_SWITCH_FADE_DURATION));
        currentMenu.interactable = false; //set before fade out completes so we remove any extraneous inputs
        currentMenu.blocksRaycasts = false;
        yield return new WaitForSeconds(MENU_SWITCH_FADE_DURATION);

        StartCoroutine(FadeInCoroutine(target, MENU_SWITCH_FADE_DURATION));
        yield return new WaitForSeconds(MENU_SWITCH_FADE_DURATION);
        target.interactable = true; //set after fade in completes so we remove any extraneous inputs
        target.blocksRaycasts = true;
        currentMenu = target;
    }
}
