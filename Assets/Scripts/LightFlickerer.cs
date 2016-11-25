using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class LightFlickerer : MonoBehaviour {

    public float deltaFlicker;
    public float maxChange;

    float startingIntensity;
    Light lightSource;

	void Start () {
        lightSource = GetComponent<Light>();
        startingIntensity = lightSource.intensity;
	}
	
	void Update () {
        lightSource.intensity = Mathf.Clamp(
            lightSource.intensity + Random.Range(-deltaFlicker, deltaFlicker),
            startingIntensity - maxChange,
            startingIntensity + maxChange);
	}
}
