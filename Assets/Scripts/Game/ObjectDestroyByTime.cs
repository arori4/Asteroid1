using UnityEngine;
using System.Collections;

public class ObjectDestroyByTime : MonoBehaviour {

    public float lifetime;

    void OnEnable() {
        StartCoroutine(Disable());
    }

    IEnumerator Disable() {
        yield return new WaitForSeconds(lifetime);

        Pools.Terminate(gameObject);
    }
}
