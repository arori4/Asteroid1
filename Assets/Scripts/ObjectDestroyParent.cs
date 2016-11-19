using UnityEngine;
using System.Collections;

/**
 * Allows a child object to disable an entire parent.
 */
public class ObjectDestroyParent : MonoBehaviour {

	void OnDisable() {
        transform.root.gameObject.SetActive(false);
    }
}
