using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ObjectDamageExplosion : NetworkBehaviour {

    public float maxRadius;
    public float expandTime;

    new SphereCollider collider;

	void OnEnable () {
        collider = GetComponent<SphereCollider>();
        collider.radius = 0;
        StartCoroutine(Expand());
	}
	
    IEnumerator Expand() {
        while (collider.radius < maxRadius) {
            collider.radius += maxRadius * Time.deltaTime / expandTime;
            yield return null;
        }

        Pools.Terminate(gameObject);
    }
    
}
