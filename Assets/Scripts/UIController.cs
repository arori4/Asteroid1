using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {

    //regular UI
    public CanvasGroup regularUI;
    public Slider healthSlider;
    public Slider energySlider;
    public Text scoreText;

    //Game over UI
    public CanvasGroup gameOverGUI;
    public Text gameOverScoreText;

    //Others
    public GameObject player;

    //Game States
    int score;
    PlayerWeapons weapons;
    ObjectCollisionHandler playerCollision;
    
	void Start () {
        gameOverGUI.alpha = 0;
        regularUI.alpha = 1;

        weapons = player.GetComponent<PlayerWeapons>();
        playerCollision = player.GetComponent<ObjectCollisionHandler>();
        score = 0;
	}
	
	void Update () {
        energySlider.value = weapons.GetEnergy() / 100;
        healthSlider.value = playerCollision.currentHealth / 100;

	}

    //Used by the button
    public void RestartGame () {
        StopAllCoroutines();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AddScore(int scoreToAdd) {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }

    public void GameOver() {
        gameOverScoreText.text = "Score: " + score;
        StartCoroutine(FadeUI());
    }

    private IEnumerator FadeUI() {
        while (gameOverGUI.alpha < 1) {
            gameOverGUI.alpha += Time.deltaTime * 0.5f;
            yield return null;
        }

        yield return new WaitForSeconds(1);

        while (regularUI.alpha > 0) {
            regularUI.alpha -= Time.deltaTime * 1;
            yield return null;
        }
    }

}
