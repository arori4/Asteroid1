using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class LobbyUIController : MonoBehaviour {

    [Header("General")]
    public CanvasGroup blackScreen;
    public Text statusText;

    [Header("Server Menu")]
    public CanvasGroup serverMenu;
    public CanvasGroup serverHelp;
    public Button serverModeButton;
    public Button clientModeButton;

    [Header("Game Menu")]
    public CanvasGroup gameMenu;
    public CanvasGroup gameStartButton;

    [Header("Help Menu")]
    public CanvasGroup helpMenu;

    //Members
    public static LobbyUIController instance;
    bool loadOnce = false;
    CanvasGroup currentMenu;
    AudioSource backgroundMusic;
    bool isServer = false; //initially server mode

    //constants
    const float MENU_SWITCH_FADE_DURATION = 0.3f;

    void Start () {
        backgroundMusic = GetComponent<AudioSource>();
        instance = this;
    }
	
	void Update () {
		
	}

    /**
     * Delegate for OnSceneLoaded
     */
    void Awake() {
        SceneManager.sceneLoaded += SceneLoaded;
    }
    void SceneLoaded(Scene scene, LoadSceneMode mode) {
        //Load scene
        serverMenu.alpha = 0;
        currentMenu = blackScreen;//done to allow showMainMenu to work
        currentMenu.alpha = 1;
        ShowServerMenu();

        //Set correct buttons
        serverModeButton.interactable = true;
        clientModeButton.interactable = false;
        statusText.text = "Currently client";

        //Deregister delegate
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    /**
     * UI management
     */

    public void SetServerMode() {
        Debug.Assert(isServer == false, "Already set to server mode");

        serverModeButton.interactable = false;
        clientModeButton.interactable = true;
        isServer = true;

        statusText.text = "Currently host";
    }

    public void SetClientMode() {
        Debug.Assert(isServer == true, "Already set to client mode");

        serverModeButton.interactable = true;
        clientModeButton.interactable = false;
        isServer = false;

        statusText.text = "Currently client";
    }

    
    public void SetStatusText(string newText) {
        statusText.text = newText;
    }

    public void ShowHelpMenu() {
        StartCoroutine(SwitchMenuCoroutine(helpMenu));
    }

    public void ShowServerMenu() {
        StartCoroutine(SwitchMenuCoroutine(serverMenu));
    }

    public void JoinGame(ulong gameID) {
        ShowGameMenu();
    }

    //do not use this outside of helper methods
    private void ShowGameMenu() {
        StartCoroutine(SwitchMenuCoroutine(gameMenu));
    }

    public void ShowMainMenu() {
        StartCoroutine(LoadNextSceneCoroutine("Main Menu"));
    }


    /*
     * Helper fading coroutines
     * Also copied to MainMenu
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
            Debug.Log("LobbyManager SwitchMenuCoroutine: current canvas is not fully opaque");
        }
        if (target.alpha > 0f) {
            Debug.Log("LobbyManager SwitchMenuCoroutine: target canvas is not starting fully transparent");
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

    private IEnumerator LoadNextSceneCoroutine(string sceneName) {
        loadOnce = true;
        while (blackScreen.alpha < 1) {
            blackScreen.alpha += Time.deltaTime * 1f;
            currentMenu.alpha -= Time.deltaTime * 1.4f; //slightly faster so stars stay in background longer
            backgroundMusic.volume -= Time.deltaTime * 1f;
            yield return null;
        }
        SceneManager.LoadScene(sceneName);
    }
}
