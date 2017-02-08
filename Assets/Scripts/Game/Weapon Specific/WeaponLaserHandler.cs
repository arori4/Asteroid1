using UnityEngine;
using System.Collections;

/**
 * Handles a laser
 */
public class WeaponLaserHandler : MonoBehaviour {

    public float glowTime;
    
    float startZSize;

	void OnEnable () {
        //reset start size
        startZSize = 30f;

        //set appropriate transform values
        transform.localScale = new Vector3(1, startZSize, 1);

        //Enable all colliders
        foreach (Collider c in gameObject.GetComponentsInChildren<Collider>()) {
            c.enabled = true;
        }

        //Upon initialization, destroy collision handler and fade
        StartCoroutine(Fade());
    }
    
    private IEnumerator Fade() {
        yield return new WaitForSeconds(0.1f);

        foreach (Collider c in gameObject.GetComponentsInChildren<Collider>()) {
            c.enabled = false;
            yield return null;
        }

        while (transform.localScale.z > 0f) {
            transform.localScale -= new Vector3(Time.deltaTime / glowTime, Time.deltaTime / glowTime, 
                startZSize * Time.deltaTime / glowTime);
            yield return null;
        }

        Pools.Terminate(gameObject);
    }
}
