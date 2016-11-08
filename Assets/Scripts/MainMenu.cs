using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;

public class MainMenu : MonoBehaviour {

    public CanvasGroup titleText;
    public CanvasGroup startGameButton;
    public CanvasGroup blackScreen;
    AudioSource backgroundMusic;
    
    bool callOnce = false;
    const float TITLE_TEXT_FADE_SPEED = 0.2f;
    const float TITLE_TEXT_RISE_SPEED = 10f;
    const float START_GAME_FADE_SPEED = 0.4f;
    

	void Start () {
        titleText.alpha = 0;
        blackScreen.alpha = 0;
        startGameButton.alpha = 0;

        backgroundMusic = GetComponent<AudioSource>();

        StartCoroutine(FadeUI());
	}

    private IEnumerator FadeUI() {
        while (titleText.alpha < 1) {
            titleText.alpha += Time.deltaTime * TITLE_TEXT_FADE_SPEED;
            yield return null;
        }

        yield return new WaitForSeconds(0.8f);

        while (startGameButton.alpha < 1) {
            startGameButton.alpha += Time.deltaTime * START_GAME_FADE_SPEED;
            yield return null;
        }
    }

    public void StartGame() {
        if (callOnce == false) {
            StartCoroutine(LoadNextSceneCoroutine());
        }
    }

    private IEnumerator LoadNextSceneCoroutine() {
        callOnce = true;
        while (blackScreen.alpha < 1) {
            blackScreen.alpha += Time.deltaTime * 0.5f;
            backgroundMusic.volume -= Time.deltaTime * 0.5f;
            yield return null;
        }
        SceneManager.LoadScene("Main Game");
    }

}
