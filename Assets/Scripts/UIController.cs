﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {

    //regular UI
    public CanvasGroup regularUI;
    public Slider healthSlider;
    public Slider healthSliderBack;
    public Slider energySlider;
    public Slider energySliderBack;
    public Slider shieldBar;
    CanvasGroup shieldBarCanvas;
    public Text scoreText;

    //regular UI weapons
    public Text weaponText;
    public GameObject weaponUIParent;
    public Text shieldText;
    public GameObject shieldUIParent;
    public Text missileText;
    public GameObject missileUIParent;
    CanvasGroup missileTextCanvas;
    CanvasGroup missileUIParentCanvas;
    GameObject missileIcon;

    //Game over UI
    public CanvasGroup gameOverGUI;
    public Text gameOverScoreText;

    //Overlays
    public CanvasGroup blackFader;
    public CanvasGroup healthLoss;

    //Others
    GameObject player;

    //Game States
    int score;
    PlayerWeapons weapons;
    ObjectCollisionHandler playerCollision;
    bool gameOverButtonPressed;
    bool hitCanvasActivated;

    private float healthSliderVelocityFront;
    private float energySliderVelocityFront;
    private float healthSliderVelocityBack;
    private float energySliderVelocityBack;
    private const float ENERGY_SLIDER_FRONT_SMOOTH = 0.3f;
    private const float ENERGY_SLIDER_BACK_SMOOTH = 1f;
    private const float HEALTH_SLIDER_FRONT_SMOOTH = 0.7f;
    private const float HEALTH_SLIDER_BACK_SMOOTH = 1.5f;

    void Start () {
        //set alphas
        blackFader.alpha = 1;
        healthLoss.alpha = 0;
        gameOverGUI.alpha = 0;
        regularUI.alpha = 1;

        player = GameObject.FindGameObjectWithTag("Player").transform.root.gameObject;
        weapons = player.GetComponent<PlayerWeapons>();
        playerCollision = player.GetComponent<ObjectCollisionHandler>();
        score = 0;
        shieldBar.gameObject.SetActive(false);
        shieldBarCanvas = shieldBar.GetComponent<CanvasGroup>();
        missileTextCanvas = missileText.GetComponent<CanvasGroup>();
        missileUIParentCanvas = missileUIParent.GetComponent<CanvasGroup>();

        //make sure game over is deactivated
        gameOverGUI.interactable = false;
        gameOverButtonPressed = false;
        hitCanvasActivated = false;

        //start the game and fade up
        StartCoroutine(FadeOutUI(blackFader, 0.4f));
	}
	
	void Update () {
        //handle health bar
        healthSlider.value = Mathf.SmoothDamp(healthSlider.value,
            playerCollision.GetCurrentHealth() / 100.0f,
            ref healthSliderVelocityFront,  HEALTH_SLIDER_FRONT_SMOOTH);
        healthSliderBack.value = Mathf.SmoothDamp( healthSliderBack.value,
            playerCollision.GetCurrentHealth() / 100.0f,
            ref healthSliderVelocityBack, HEALTH_SLIDER_BACK_SMOOTH);
        //force back slider to be over or at the regular health slider amount
        healthSliderBack.value = Mathf.Max(healthSlider.value, healthSliderBack.value);

        //handle energy bar
        energySlider.value = Mathf.SmoothDamp(energySlider.value, 
            weapons.GetEnergy() / 100.0f, 
            ref energySliderVelocityFront, ENERGY_SLIDER_FRONT_SMOOTH);
        energySliderBack.value = Mathf.SmoothDamp(energySliderBack.value,
            weapons.GetEnergy() / 100.0f,
            ref energySliderVelocityBack, ENERGY_SLIDER_BACK_SMOOTH);
        //force back slider up
        energySliderBack.value = Mathf.Max(energySlider.value, energySliderBack.value);
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

        //hide the ui buttons
        weaponUIParent.SetActive(false);
        shieldUIParent.SetActive(false);

        StartCoroutine(GameOverFadeUI());
    }

    public void ChangeWeapon(string weaponType, GameObject uiWeaponIcon) {
        //set weapon text
        weaponText.text = weaponType;

        //remove all children of the parent
        foreach (Transform child in weaponUIParent.transform) {
            GameObject.Destroy(child.gameObject);
        }

        //add in new child
        GameObject newSymbol = Instantiate(uiWeaponIcon,
            weaponUIParent.transform.position, new Quaternion(90, 90, 225, 0)) as GameObject;
        newSymbol.transform.parent = weaponUIParent.transform;
    }


    public void ChangeMissile(string missileType, int amount, GameObject uiMissileIcon) {
        //set weapon text
        ChangeMissileCount(missileType, amount);

        //remove all children of the parent
        foreach (Transform child in missileUIParent.transform) {
            GameObject.Destroy(child.gameObject);
        }

        //add in new child
        missileIcon = Instantiate(uiMissileIcon,
            missileUIParent.transform.position, new Quaternion(90, 90, 225, 0)) as GameObject;
        missileIcon.transform.parent = missileUIParent.transform;
    }

    /**
     * Separate method that will be called upon firing a missile
     */
    public void ChangeMissileCount(string missileType, int amount) {
        //start initial alpha at 1
        missileUIParentCanvas.alpha = 1;
        missileTextCanvas.alpha = 1;
        missileIcon.SetActive(true);

        missileText.text = missileType + " (" + amount + ")";

        //fade if there are no more missiles to show
        if (amount == 0) {
            FadeOutUI(missileUIParentCanvas, 1.0f);
            FadeOutUI(missileTextCanvas, 1.0f);
            missileIcon.SetActive(false);
        }
    }

    public void HitUI(float damage) {
        if (!hitCanvasActivated) {
            StartCoroutine(HitRoutine(damage));
            hitCanvasActivated = true;
        }
    }

    public void StartShieldRecharge(float numSeconds) {
        shieldBar.gameObject.SetActive(true);
        shieldBar.value = 0f;

        StartCoroutine(ShieldRechargeRoutine(numSeconds));
    }

    private IEnumerator ShieldRechargeRoutine(float numSeconds) {
        while (shieldBar.value < 1) {
            shieldBar.value += Time.deltaTime / numSeconds;
            yield return null;
        }

        //keep shield bar up for a second before removing it
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
        shieldBar.gameObject.SetActive(false);
    }

    private IEnumerator HitRoutine(float damage) {
        //cap damage to highest amount
        damage = Mathf.Min(damage, 49.99f);

        //Does not use provided methods because there is a different alpha
        while (healthLoss.alpha < damage / 50.0f) {
            healthLoss.alpha += Time.deltaTime * 70.0f / damage;
            yield return null;
        }

        while (healthLoss.alpha > 0) {
            healthLoss.alpha -= Time.deltaTime * 35.0f / damage;
            yield return null;
        }

        hitCanvasActivated = false;
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

    private IEnumerator RestartGameCoroutine() {
        StartCoroutine(FadeInUI(blackFader, 0.5f));
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator MainMenuCoroutine() {
        StartCoroutine(FadeInUI(blackFader, 0.3f));
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene("Main Menu");
    }

}
