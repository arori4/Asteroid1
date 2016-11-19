using UnityEngine;
using System.Collections;

public class ObjectDamageExplosion : MonoBehaviour {

    public float maxRadius;
    public float expandTime;

    new SphereCollider collider;

	void OnEnable () {
        collider = GetComponent<SphereCollider>();
        StartCoroutine(Expand());
	}
	
    IEnumerator Expand() {
        while (collider.radius < maxRadius) {
            collider.radius += maxRadius * Time.deltaTime / expandTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }
    
}
