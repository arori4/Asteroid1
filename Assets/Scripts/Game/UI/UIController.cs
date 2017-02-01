using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

/**
 * Controls some UI elements
 * Might be deprecated
 */
public class UIController : MonoBehaviour {

    [Header("Regular UI")]
    public CanvasGroup regularUI;
    public Text scoreText;
    public UIButtonGroup weaponUIGroup;
    public UIButtonGroup shieldUIGroup;
    public UIButtonGroup missileUIGroup;
    public UISliderGroup healthSlider;
    public UISliderGroup energySlider;
    public UISliderGroup shieldSlider;
    
    [Header("Hit Canvas")]
    public CanvasGroup hitCanvas;
    bool hitCanvasActivated; //lock
    
    [Header("Game Over")]
    public CanvasGroup gameOverGUI;
    public Text gameOverScoreText;
    
    [Header("Overlays")]
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

    public IEnumerator HitCanvasFader(float damage) {
        if (!hitCanvasActivated) {
            hitCanvasActivated = true;
            //cap damage to highest amount
            damage = Mathf.Min(damage, 49.99f);

            //Does not use provided methods because there is a different alpha
            while (hitCanvas.alpha < damage / 50.0f) {
                hitCanvas.alpha += Time.deltaTime * 70.0f / damage;
                yield return null;
            }

            while (hitCanvas.alpha > 0) {
                hitCanvas.alpha -= Time.deltaTime * 35.0f / damage;
                yield return null;
            }

            hitCanvasActivated = false;
        }
    }

    /*
     * Recharge the shield when it has been destroyed
     */
    public IEnumerator RechargeShield(float rechargeTime, PlayerWeapons caller) {
        shieldUIGroup.Hide();

        shieldSlider.gameObject.SetActive(true);
        shieldSlider.val = 0f;

        caller.shieldRecharging = true;

        //TODO: move this to an option in UISLiderGroup later
        while (shieldSlider.val < 1) {
            shieldSlider.val += Time.deltaTime / rechargeTime;
            yield return null;
        }

        //keep shield bar up for a second before removing it
        CanvasGroup shieldBarCanvas = shieldSlider.gameObject.GetComponent<CanvasGroup>();
        for (int index = 0; index < 2; index++) {
            while (shieldBarCanvas.alpha > 0) {
                shieldBarCanvas.alpha -= Time.deltaTime / 0.25f;
                yield return null;
            }
            while (shieldBarCanvas.alpha < 1) {
                shieldBarCanvas.alpha += Time.deltaTime / 0.25f;
                yield return null;
            }

        }

        //finallly fade the shield bar
        while (shieldBarCanvas.alpha > 0) {
            shieldBarCanvas.alpha -= Time.deltaTime / 0.25f;
            yield return null;
        }
        //set alpha back to 1 so that when we need it again, it appears
        shieldBarCanvas.alpha = 1;
        shieldSlider.gameObject.SetActive(false);

        shieldUIGroup.Show();
        caller.shieldRecharging = false;
    }

}
