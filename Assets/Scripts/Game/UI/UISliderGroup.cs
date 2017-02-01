using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Defines a bar group
 */
public class UISliderGroup : MonoBehaviour {

    public Slider frontSlider;
    public Slider backSlider;

    public float val;
    public float FRONT_SMOOTH = 1f;
    public float BACK_SMOOTH = 1f;

    private float frontVelocity;
    private float backVelocity;

    public float SLIDER_SIZE_DIVIDER = 200f;

    bool hasBack;

    void Start () {

        //Error checks
        hasBack = (backSlider != null);

    }
	
	void Update () {
        frontSlider.value = Mathf.SmoothDamp(frontSlider.value,
            val / SLIDER_SIZE_DIVIDER,
            ref frontVelocity, FRONT_SMOOTH);

        if (hasBack) {
            backSlider.value = Mathf.SmoothDamp(backSlider.value,
                val / SLIDER_SIZE_DIVIDER,
                ref backVelocity, BACK_SMOOTH);
            //force back slider to be over or at the regular health slider amount
            backSlider.value = Mathf.Max(frontSlider.value, backSlider.value);
        }
    }

    public void ChangeValue(float value) {
        val = value;
    }
}
