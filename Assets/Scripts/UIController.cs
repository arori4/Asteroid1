using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

/**
 * Controls some UI elements
 * Might be deprecated
 */
public class UIController : MonoBehaviour {

    //regular UI
    public CanvasGroup regularUI;
    public Text scoreText;

    //Game over UI
    public CanvasGroup gameOverGUI;
    public Text gameOverScoreText;

    //Overlays
    public CanvasGroup blackFader;

    //Game States
    int score;
    bool gameOverButtonPressed;

    void Start () {
        Application.targetFrameRate = 60;

        //set alphas
        blackFader.alpha = 1;
        gameOverGUI.alpha = 0;
        regularUI.alpha = 1;

        //Set score
        if (PlayerPrefs.HasKey("Score")) {
            AddScore(PlayerPrefs.GetInt("Score")); //score starts at 0
        }
        else {
            PlayerPrefs.SetInt("Score", 0);
            AddScore(0);
        }

        //make sure game over is deactivated
        gameOverGUI.interactable = false;
        gameOverButtonPressed = false;

        //start the game and fade up
        StartCoroutine(FadeOutUI(blackFader, 0.4f));
	}
    
    public void ButtonRestartGame () {
        if (!gameOverButtonPressed) {
            StopAllCoroutines();
            gameOverButtonPressed = true;
            StartCoroutine(RestartGameCoroutine());
        }
    }
    
    public void ButtonMainMenu() {
        if (!gameOverButtonPressed) {
            StopAllCoroutines();
            gameOverButtonPressed = true;
            StartCoroutine(MainMenuCoroutine());
        }
    }

    public void AddScore(int scoreToAdd) {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }

    public void GameOver() {
        gameOverScoreText.text = "Score: " + score;
        gameOverGUI.interactable = true;
        GetComponent<GameSave>().SaveHighScore(score, 1); //for now, only 1 level

        //set the level back to 1
        PlayerPrefs.SetInt("Level", 1);
        PlayerPrefs.SetInt("Score", 0);

        StartCoroutine(GameOverFadeUI());
    }

    public void AdvanceLevel() {
        StartCoroutine(AdvanceLevelCoroutine());
    }

    private IEnumerator GameOverFadeUI() {
        StartCoroutine(FadeInUI(gameOverGUI, 0.5f));
        yield return new WaitForSeconds(1);
        StartCoroutine(FadeOutUI(regularUI, 1));
    }

    /**
     * Fades the canvas in to an alpha of 1
     * Canvas will be opaque
     */
    private IEnumerator FadeInUI(CanvasGroup canvas, float smoothing) {
        while (canvas.alpha < 1) {
            canvas.alpha += Time.deltaTime * smoothing;
            yield return null;
        }
    }

    /**
     * Fades the canvas out to an alpha of 0.
     * Canvas will be transparent
     */
    private IEnumerator FadeOutUI(CanvasGroup canvas, float smoothing) {
        while (canvas.alpha > 0) {
            canvas.alpha -= Time.deltaTime * smoothing;
            yield return null;
        }
    }

    private IEnumerator AdvanceLevelCoroutine() {
        StartCoroutine(FadeInUI(blackFader, 0.4f));
        yield return new WaitForSeconds(4.0f);

        //Add level and score
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
        PlayerPrefs.SetInt("Score", score);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator RestartGameCoroutine() {
        StartCoroutine(FadeInUI(blackFader, 0.5f));
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator MainMenuCoroutine() {
        StartCoroutine(FadeInUI(blackFader, 0.5f));
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene("Main Menu");
    }

}
