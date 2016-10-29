using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {

    public Slider healthSlider;
    public Slider energySlider;

    public GameObject player;

    string TAG = "UIController";
    PlayerWeapons weapons;

	// Use this for initialization
	void Start () {
        if (healthSlider == null) {
            Debug.Log(TAG + " health slider is null ");
        }
        if (energySlider == null) {
            Debug.Log(TAG + " energy slider is null");
        }
        if (player == null) {
            Debug.Log(TAG + " player is null");
        }

        weapons = player.GetComponent<PlayerWeapons>();
	}
	
	// Update is called once per frame
	void Update () {

        //Energy Slider
        energySlider.value = weapons.GetEnergy() / 100;

	}

    //Used by the button
    public void RestartGame () {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
