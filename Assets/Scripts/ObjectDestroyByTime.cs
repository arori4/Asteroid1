using UnityEngine;
using System.Collections;

public class ObjectDestroyByTime : MonoBehaviour {

    public float lifetime;

    void Start() {
        Destroy(gameObject, lifetime);
    }
}
