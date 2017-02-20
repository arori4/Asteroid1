using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/**
 * 'Destroys' an object after a certain amount of time.
 * Sends it back to the pool.
 */
public class ObjectDestroyByTime : NetworkBehaviour {

    public float lifetime;

    void OnEnable() {
        if (!isServer) { return; }
        StartCoroutine(Disable());
    }

    IEnumerator Disable() {
        yield return new WaitForSeconds(lifetime);

        Pools.Terminate(gameObject);
    }
}
