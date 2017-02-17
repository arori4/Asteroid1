using UnityEngine;
using System.Collections;

/**
 * Disappates a lightSource object that appeared
 */
public class LightDissipator : MonoBehaviour {

    Light lightSource;
    public float duration;

    float startingIntensity;
    float currentVelocity;
    
	void Start () {
        lightSource = GetComponent<Light>();
        startingIntensity = lightSource.intensity;
    }
    
	void Update () {
        lightSource.intensity = Mathf.SmoothDamp(lightSource.intensity, 0, ref currentVelocity, duration / 2);
	}

    void OnDisable() { //TODO: change onDisable behaviour to a differet
        if (lightSource != null) {
            lightSource.intensity = startingIntensity;
        }
    }
}
