using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/**
 * Globally deactivates an object if it is past a certain X value
 */
public class ObjectDestroyByDistance : MonoBehaviour {

    static int X_THRESHOLD = -30;
	
	void Update () {
		if (transform.position.x < X_THRESHOLD) {
            Pools.Terminate(gameObject);
        }
	}
}
