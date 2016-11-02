using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;

public class MainMenu : MonoBehaviour {

    public CanvasGroup titleText;
    public CanvasGroup startGameButton;
    public CanvasGroup blackScreen;

    bool callOnce = false;

	// Use this for initialization
	void Start () {
        titleText.alpha = 0;
        blackScreen.alpha = 0;
        startGameButton.alpha = 0;

        StartCoroutine(FadeUI());
	}

    private IEnumerator FadeUI() {
        while (titleText.alpha < 1) {
            titleText.alpha += Time.deltaTime * 0.2f;
            yield return null;
        }

        yield return new WaitForSeconds(0.8f);

        while (startGameButton.alpha < 1) {
            startGameButton.alpha += Time.deltaTime * 0.4f;
            yield return null;
        }
    }

    public void StartGame() {
        print("start game");

        if (callOnce == false) {
            StartCoroutine(LoadNextSceneCoroutine());
        }
    }

    private IEnumerator LoadNextSceneCoroutine() {
        callOnce = true;
        while (blackScreen.alpha < 1) {
            blackScreen.alpha += Time.deltaTime * 0.5f;
            yield return null;
        }
        SceneManager.LoadScene("Main Game");
    }

}
