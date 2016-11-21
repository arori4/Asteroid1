using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;

public class MainMenu : MonoBehaviour {

    public CanvasGroup mainMenu;
    public CanvasGroup titleText;
    public CanvasGroup startLevelsButton;
    public CanvasGroup startSurvivalButton;
    public CanvasGroup blackScreen;
    public CanvasGroup goToHighScoresButton;
    AudioSource backgroundMusic;

    public CanvasGroup highScoreMenu;
    public CanvasGroup highScoreBack;
    public Text highScoreText;

    GameSave gameSave;
    bool callOnce = false;
    const float TITLE_TEXT_FADE_SPEED = 0.2f;
    const float TITLE_TEXT_RISE_SPEED = 10f;
    const float START_GAME_FADE_SPEED = 0.4f;
    

	void Start () {
        titleText.alpha = 0;
        blackScreen.alpha = 0;
        startLevelsButton.alpha = 0;
        startSurvivalButton.alpha = 0;
        mainMenu.alpha = 1;
        highScoreMenu.alpha = 0;
        goToHighScoresButton.alpha = 0;

        backgroundMusic = GetComponent<AudioSource>();
        gameSave = GetComponent<GameSave>();

        StartCoroutine(FadeUI());
	}

    private IEnumerator FadeUI() {
        while (titleText.alpha < 1) {
            titleText.alpha += Time.deltaTime * TITLE_TEXT_FADE_SPEED;
            yield return null;
        }

        yield return new WaitForSeconds(0.8f);

        while (startLevelsButton.alpha < 1) {
            startLevelsButton.alpha += Time.deltaTime * START_GAME_FADE_SPEED;
            startSurvivalButton.alpha += Time.deltaTime * START_GAME_FADE_SPEED;
            goToHighScoresButton.alpha += Time.deltaTime * START_GAME_FADE_SPEED;
            yield return null;
        }
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
        StartCoroutine(ShowHighScores());

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
        StartCoroutine(DoneHighScores());
    }

    private IEnumerator ShowHighScores() {
        while (mainMenu.alpha > 0) {
            mainMenu.alpha -= Time.deltaTime * 2f;
            yield return null;
        }

        while (highScoreMenu.alpha < 1) {
            highScoreMenu.alpha += Time.deltaTime * 2f;
            yield return null;
        }
    }

    private IEnumerator DoneHighScores() {
        while (highScoreMenu.alpha > 0) {
            highScoreMenu.alpha -= Time.deltaTime * 2f;
            yield return null;
        }

        while (mainMenu.alpha < 1) {
            mainMenu.alpha += Time.deltaTime * 2f;
            yield return null;
        }
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

}
