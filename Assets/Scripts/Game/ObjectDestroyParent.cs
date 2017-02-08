using UnityEngine;
using System.Collections;

/**
 * Allows a child object to disable an entire parent.
 */
public class ObjectDestroyParent : MonoBehaviour {

    /* Deactivated cuz we don't use OnDisable anymore
	void OnDisable() {
        Pools.Terminate(transform.root.gameObject);
    }
    */
}
