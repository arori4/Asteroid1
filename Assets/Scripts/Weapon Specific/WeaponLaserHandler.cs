using UnityEngine;
using System.Collections;

public class WeaponLaserHandler : MonoBehaviour {

    public float glowTime = 0.25f;
    
    float startZSize;

	void Start () {
        startZSize = transform.localScale.z;

        //set position to appropriate place
        transform.position += Vector3.right * transform.localScale.y / 4;

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
            transform.localScale -= new Vector3(0, 0, startZSize * Time.deltaTime / glowTime);
            yield return null;
        }

        Destroy(gameObject);


    }
}
