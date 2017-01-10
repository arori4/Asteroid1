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
    public CanvasGroup startLevelsButton;
    public CanvasGroup startSurvivalButton;
    public CanvasGroup blackScreen;
    public CanvasGroup goToHighScoresButton;
    AudioSource backgroundMusic;

    [Header("High Score Menu")]
    public CanvasGroup highScoreMenu;
    public CanvasGroup highScoreBack;
    public Text highScoreText;

    [Header("Survival Menu")]
    public CanvasGroup survialMenu;
    public CanvasGroup survivalEasyButton;
    public CanvasGroup survivalMediumButton;
    public CanvasGroup survivalHardButton;
    public CanvasGroup survivalBackButton;

    GameSave gameSave;
    bool callOnce = false;
    const float MENU_SWITCH_FADE_DURATION = 0.5f;
    const float TITLE_TEXT_FADE_DURATION = 5f;
    const float TITLE_TEXT_RISE_DURATION = 10f;
    const float START_GAME_FADE_DURATION = 1f / 0.4f;
    const int EASY_LEVEL = 1;
    const int MEDIUM_LEVEL = 10;
    const int HARD_LEVEL = 20;
    

	void Start () {
        //Hard set beginning values
        titleText.alpha = 0;
        blackScreen.alpha = 0;
        startLevelsButton.alpha = 0;
        startSurvivalButton.alpha = 0;
        mainMenu.alpha = 1;
        highScoreMenu.alpha = 0;
        goToHighScoresButton.alpha = 0;

        //Components
        backgroundMusic = GetComponent<AudioSource>();
        gameSave = GetComponent<GameSave>();

        StartCoroutine(StartCoroutine());
	}

    private IEnumerator StartCoroutine() {
        StartCoroutine(FadeInCoroutine(titleText, TITLE_TEXT_FADE_DURATION));
        yield return new WaitForSeconds(1.6f);
        StartCoroutine(FadeInCoroutine(startLevelsButton, START_GAME_FADE_DURATION));
        StartCoroutine(FadeInCoroutine(startSurvivalButton, START_GAME_FADE_DURATION));
        StartCoroutine(FadeInCoroutine(goToHighScoresButton, START_GAME_FADE_DURATION));
    }

    public void StartLevelsGame() {
        if (callOnce == false) {
            //Set settings
            PlayerPrefs.SetInt("Level", 1);
            PlayerPrefs.SetInt("Score", 0);
            PlayerPrefs.SetInt("Mode", 0);

            //Set opposite to alpha of 0
            startSurvivalButton.alpha = 0;

            StartCoroutine(LoadNextSceneCoroutine());
        }
    }

    public void StartSurvivalGame() {
        if (callOnce == false) {
            //Set settings
            PlayerPrefs.SetInt("Level", 1);
            PlayerPrefs.SetInt("Score", 0);
            PlayerPrefs.SetInt("Mode", 1);

            //Set opposite to alpha of 0
            startLevelsButton.alpha = 0;

            StartCoroutine(LoadNextSceneCoroutine());
        }
    }

    public void HighScores() {
        mainMenu.interactable = false;
        mainMenu.blocksRaycasts = false;
        highScoreMenu.interactable = true;
        highScoreMenu.blocksRaycasts = true;
        StartCoroutine(ShowHighScoresCoroutine());

        //display high scores
        string text = "";
        for (int index = 0; index < 10; index++) {
            text += (index + 1) + ".  " + gameSave.highScores[index] + "\n";
        }
        highScoreText.text = text;
    }

    public void BackFromHighScores() {
        mainMenu.interactable = true;
        mainMenu.blocksRaycasts = true;
        highScoreMenu.interactable = false;
        highScoreMenu.blocksRaycasts = false;
        StartCoroutine(DoneHighScoresCoroutine());
    }

    private IEnumerator ShowHighScoresCoroutine() {
        StartCoroutine(FadeOutCoroutine(mainMenu, MENU_SWITCH_FADE_DURATION));
        yield return new WaitForSeconds(MENU_SWITCH_FADE_DURATION);
        StartCoroutine(FadeInCoroutine(highScoreMenu, MENU_SWITCH_FADE_DURATION));
    }

    private IEnumerator DoneHighScoresCoroutine() {
        StartCoroutine(FadeOutCoroutine(highScoreMenu, MENU_SWITCH_FADE_DURATION));
        yield return new WaitForSeconds(MENU_SWITCH_FADE_DURATION);
        StartCoroutine(FadeInCoroutine(mainMenu, MENU_SWITCH_FADE_DURATION));
    }

    private IEnumerator LoadNextSceneCoroutine() {
        callOnce = true;
        while (blackScreen.alpha < 1) {
            blackScreen.alpha += Time.deltaTime * 0.5f;
            mainMenu.alpha -= Time.deltaTime * 0.7f; //slightly faster so stars stay in background longer
            backgroundMusic.volume -= Time.deltaTime * 0.5f;
            yield return null;
        }
        SceneManager.LoadScene("Main Game");
    }


    public void StartSurvivalEasy() {

    }

    public void StartSurvivalMedium() {

    }

    public void StartSurvivalHard() {

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
}
