using UnityEngine;
using System.Collections;

public class LightDissipator : MonoBehaviour {

    new Light light;
    public float duration;

    float startingIntensity;
    float currentVelocity;
    
	void Start () {
        light = GetComponent<Light>();
        startingIntensity = light.intensity;
    }
    
	void Update () {
        light.intensity = Mathf.SmoothDamp(light.intensity, 0, ref currentVelocity, duration / 2);
	}

    void OnDisable() {
        if (light != null) {
            light.intensity = startingIntensity;
        }
    }
}
