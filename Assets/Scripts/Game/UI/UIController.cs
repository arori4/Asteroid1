using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

/**
 * Controls all UI elements during the game
 */
public class UIController : NetworkBehaviour {

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
    
    [Header("UI")]
    public CanvasGroup largeTextCanvas;
    public Text largeText;

    [Header("Game Over")]
    public CanvasGroup gameOverGUI;
    public Text gameOverScoreText;
    
    [Header("Other Overlays")]
    public CanvasGroup blackFader;

    bool gameOverButtonPressed;

    public PlayerWeapons player;

    void Start () {
        Application.targetFrameRate = 60;

        //set alphas
        regularUI.alpha = 1;
        blackFader.alpha = 1;
        hitCanvas.alpha = 0;
        largeTextCanvas.alpha = 0;
        gameOverGUI.alpha = 0;

        //make sure game over is deactivated
        gameOverGUI.interactable = false;
        gameOverButtonPressed = false;

        //start the game and fade up
        StartCoroutine(FadeOutCoroutine(blackFader, 0.4f));
	}

    [ClientRpc]
    public void RpcSetScoreText(int score) {
        scoreText.text = "Score: " + score;
    }

    /**
     * Game Over
     */
    [ClientRpc]
    public void RpcGameOver() {
        gameOverScoreText.text = scoreText.text;
        gameOverGUI.interactable = true;

        StartCoroutine(GameOverFadeUI());
    }
    private IEnumerator GameOverFadeUI() {
        StartCoroutine(FadeInCoroutine(gameOverGUI, 0.5f));
        yield return new WaitForSeconds(1);
        StartCoroutine(FadeOutCoroutine(regularUI, 1));
    }

    [ClientRpc]
    public void RpcAdvanceLevel() {
        StartCoroutine(AdvanceLevelCoroutine());
    }

    /**
     * Large Text
     */

    public void ShowLargeText(string text, float duration) {
        largeTextCanvas.alpha = 1;
        largeText.text = "text";
        StartCoroutine(FadeOutCoroutine(largeTextCanvas, 0.3f));
    }

    /**
     * Local Player usage
     */
    public void RegisterHit(float damage) {
        StartCoroutine(HitCanvasFader(damage));
    }
    private IEnumerator HitCanvasFader(float damage) {
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

    public void ShieldRecharge(float rechargeTime, PlayerWeapons caller) {
        StartCoroutine(RechargeShield(rechargeTime, caller));
    }
    private IEnumerator RechargeShield(float rechargeTime, PlayerWeapons caller) {
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
            StartCoroutine(FadeInCoroutine(shieldBarCanvas, 4f));
            yield return new WaitForSeconds(0.25f);
            StartCoroutine(FadeOutCoroutine(shieldBarCanvas, 4f));
            yield return new WaitForSeconds(0.25f);
        }

        StartCoroutine(FadeOutCoroutine(shieldBarCanvas, 4f));
        yield return new WaitForSeconds(0.25f);
        
        //set alpha back to 1 so that when we need it again, it appears
        shieldBarCanvas.alpha = 1;
        shieldSlider.gameObject.SetActive(false);

        shieldUIGroup.Show();
        caller.shieldRecharging = false;
    }

    //TODO separate functions for different cases instead of bool val?
    public void ChangeWeaponUI(WeaponInfo weaponInfo, bool duringGame) {
        if (duringGame) {
            weaponUIGroup.ChangeObjectDuringGame(weaponInfo.weaponIcon, weaponInfo.weaponName);
        }
        else {
            weaponUIGroup.SetModel(weaponInfo.weaponIcon);
            weaponUIGroup.SetText(weaponInfo.weaponName);
        }
    }
    public void ChangeMissileUI(MissileInfo missileInfo, bool duringGame) {
        if (duringGame) {
            missileUIGroup.ChangeObjectDuringGame(missileInfo.missileIcon, missileInfo.missileName);
        }
        else {
            missileUIGroup.SetModel(missileInfo.missileIcon);
            missileUIGroup.SetText(missileInfo.missileName);
        }
    }
    public void ChangeShieldUI(ShieldInfo shieldInfo, bool duringGame) {
        if (duringGame) {
            shieldUIGroup.ChangeObjectDuringGame(shieldInfo.shieldIcon, shieldInfo.shieldName);
        }
        else {
            shieldUIGroup.SetModel(shieldInfo.shieldIcon);
            shieldUIGroup.SetText(shieldInfo.shieldName);
        }
    }


    /**
     * Changing Scenes
     */

    private IEnumerator AdvanceLevelCoroutine() {
        StartCoroutine(FadeInCoroutine(blackFader, 0.4f));
        yield return new WaitForSeconds(4.0f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator RestartGameCoroutine() {
        StartCoroutine(FadeInCoroutine(blackFader, 0.5f));
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator MainMenuCoroutine() {
        StartCoroutine(FadeInCoroutine(blackFader, 0.5f));
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene("Main Menu");
    }


    /*
    * Helper fading coroutines
    * Also copied to NetworkMultiplayer
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

    //For UI Use
    public void onFireButtonDown() {
        player.weaponButtonPressed = true;
    }
    public void onFireButtonUp() {
        player.weaponButtonPressed = false;
    }
    public void onShieldButtonDown() {
        player.shieldButtonPressed = true;
    }
    public void onShieldButtonUp() {
        player.shieldButtonPressed = false;
    }
    public void onMissileButtonDown() {
        player.missileButtonPressed = true;
    }
    public void onMissileButtonUp() {
        player.missileButtonPressed = false;
    }

    public void ButtonRestartGame() {
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
}
