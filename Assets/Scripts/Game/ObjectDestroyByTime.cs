using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/**
 * 'Destroys' an object after a certain amount of time.
 * Sends it back to the pool.
 */
public class ObjectDestroyByTime : NetworkBehaviour {

    public float lifetime;

    bool hasStarted;

    void OnEnable() {
        if (!isServer) { return; }

        if (!hasStarted) {
            StartCoroutine(Disable());
            hasStarted = true;
        }
    }

    void OnDisable() {
        StopAllCoroutines();
        hasStarted = false;
    }

    IEnumerator Disable() {
        yield return new WaitForSeconds(lifetime);
        Pools.Terminate(gameObject);
        hasStarted = false;
    }
}
