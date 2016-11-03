using UnityEngine;
using System.Collections;

public class LightDissipator : MonoBehaviour {

    new Light light;
    public float duration;

    float currentVelocity;
    
	void Start () {
        light = GetComponent<Light>();
	}
	
	void Update () {
        light.intensity = Mathf.SmoothDamp(light.intensity, 0, ref currentVelocity, duration / 2);

	    if (light.intensity <= 0.001) {
            Destroy(gameObject);
        }
	}
}
