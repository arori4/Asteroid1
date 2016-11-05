using UnityEngine;
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
    public Text scoreText;
    //regular UI weapons
    public Text weaponText;
    public WeaponStringPair[] weaponPairs;
    public GameObject weaponUIParent;

    //Game over UI
    public CanvasGroup gameOverGUI;
    public Text gameOverScoreText;

    //Overlays
    public CanvasGroup blackFader;
    public CanvasGroup healthLoss;

    //Others
    public GameObject player;

    //Game States
    int score;
    PlayerWeapons weapons;
    ObjectCollisionHandler playerCollision;

    private float healthSliderVelocityFront;
    private float energySliderVelocityFront;
    private float healthSliderVelocityBack;
    private float energySliderVelocityBack;
    private const float ENERGY_SLIDER_FRONT_SMOOTH = 0.3f;
    private const float ENERGY_SLIDER_BACK_SMOOTH = 1f;
    private const float HEALTH_SLIDER_FRONT_SMOOTH = 0.7f;
    private const float HEALTH_SLIDER_BACK_SMOOTH = 1.5f;

    void Start () {
        blackFader.alpha = 1;
        healthLoss.alpha = 0;
        gameOverGUI.alpha = 0;
        regularUI.alpha = 1;

        weapons = player.GetComponent<PlayerWeapons>();
        playerCollision = player.GetComponent<ObjectCollisionHandler>();
        score = 0;

        StartCoroutine(FadeOutUI(blackFader, 0.4f));
	}
	
	void Update () {
        //handle health bar
        healthSlider.value = Mathf.SmoothDamp(
            healthSlider.value,
            playerCollision.GetCurrentHealth() / 100.0f,
            ref healthSliderVelocityFront,
            HEALTH_SLIDER_FRONT_SMOOTH);
        healthSliderBack.value = Mathf.SmoothDamp(
            healthSliderBack.value,
            playerCollision.GetCurrentHealth() / 100.0f,
            ref healthSliderVelocityBack,
            HEALTH_SLIDER_BACK_SMOOTH);
        //force back slider to be over or at the regular health slider amount
        healthSliderBack.value = Mathf.Max(healthSlider.value, healthSliderBack.value);

        //handle energy bar
        energySlider.value = Mathf.SmoothDamp(
            energySlider.value, 
            weapons.GetEnergy() / 100.0f, 
            ref energySliderVelocityFront, 
            ENERGY_SLIDER_FRONT_SMOOTH);
        energySliderBack.value = Mathf.SmoothDamp(
            energySliderBack.value,
            weapons.GetEnergy() / 100.0f,
            ref energySliderVelocityBack,
            ENERGY_SLIDER_BACK_SMOOTH);
        //force back slider up
        energySliderBack.value = Mathf.Max(energySlider.value, energySliderBack.value);
    }

    //Used by the button
    public void RestartGame () {
        StopAllCoroutines();
        StartCoroutine(RestartGameCoroutine());
    }

    //Used by the button
    public void MainMenu() {
        StopAllCoroutines();
        StartCoroutine(MainMenuCoroutine());
    }

    private IEnumerator RestartGameCoroutine() {
        while (blackFader.alpha < 1) {
            blackFader.alpha += Time.deltaTime * 0.3f;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator MainMenuCoroutine() {
        while (blackFader.alpha < 1) {
            blackFader.alpha += Time.deltaTime * 0.3f;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Main Menu");
    }

    public void AddScore(int scoreToAdd) {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }

    public void GameOver() {
        gameOverScoreText.text = "Score: " + score;
        StartCoroutine(FadeUI());
    }

    public void ChangeWeapon(string weaponType) {

        for (int index = 0; index < weaponPairs.Length; index++) {
            if (weaponPairs[index].weaponName == weaponType) { //yes we can do this ==

                //set weapon text
                weaponText.text = weaponType;

                //remove all children of the parent
                foreach (Transform child in weaponUIParent.transform) {
                    GameObject.Destroy(child.gameObject);
                }

                //add in new child
                GameObject newSymbol = Instantiate(
                    weaponPairs[index].uiWeaponIcon,
                    weaponUIParent.transform.position, new Quaternion(90, 90, 225, 0)) as GameObject;
                newSymbol.transform.parent = weaponUIParent.transform;

                return;
            }
        }

        print("Could not find weapon type " + weaponType);

        
    }

    private IEnumerator FadeUI() {
        StartCoroutine(FadeInUI(gameOverGUI, 0.5f));

        yield return new WaitForSeconds(1);

        StartCoroutine(FadeOutUI(regularUI, 1));
    }

    private IEnumerator FadeInUI(CanvasGroup canvas, float smoothing) {
        while (canvas.alpha < 1) {
            canvas.alpha += Time.deltaTime * smoothing;
            yield return null;
        }
    }

    private IEnumerator FadeOutUI(CanvasGroup canvas, float smoothing) {
        while (canvas.alpha > 0) {
            canvas.alpha -= Time.deltaTime * smoothing;
            yield return null;
        }
    }

}

[System.Serializable]
public class WeaponStringPair {
    public string weaponName;
    public GameObject uiWeaponIcon;
}