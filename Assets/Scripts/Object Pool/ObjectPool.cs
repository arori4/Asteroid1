using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Defines an object pool for efficient creation
 */ 
public class ObjectPool {

    public readonly GameObject sourceObject;

    private List<GameObject> pool = new List<GameObject>();

    public ObjectPool(GameObject obj) {
        if (obj == null) {
            Debug.Log("ObjectPool initiate with a null object.");
        }

        sourceObject = obj;
    }
    
    public GameObject nextObject {

        get {
            //if object is null, tell us
            if (sourceObject == null) {
                Debug.Log("Source object is null.");
                return null;
            }

            //create new object if there are none available
            if (pool.Count < 1) {
                CreateObject();
            }

            int indexToClaim = pool.Count - 1;
            GameObject clone = pool[indexToClaim];

            //debug check
            if (clone.active) {
                Debug.Log(clone.name + " in pool was still active upon initialization.");
            }

            pool.Remove(clone);
            clone.SetActive(true);
            return clone;
        }

        //Add new member back on disable
        set {
            value.SetActive(false);
            pool.Add(value);
        }

    }
    
    void CreateObject() {
        //create object
        GameObject newClone = GameObject.Instantiate(sourceObject);

        //add object to pool and set member
        PoolMember member = newClone.AddComponent<PoolMember>();
        member.pool = this;

        //Set object to inactive (also adds it to pool)
        newClone.SetActive(false);
    }

    public IEnumerator InitializeCoroutine(int amount) {
        //Add amount objects to the pool
        for (int index = 0; index < amount; index++) {
            CreateObject();
            yield return null;
        }
    }
    
}

[System.Serializable]
public class PoolMember : MonoBehaviour {

    public ObjectPool pool;

    ParticleSystem particles;

    void Start() {
        //check for different components
        particles = GetComponent<ParticleSystem>();
    }

    void OnEnable() {
        if (particles != null) {
            particles.enableEmission = true;
        }
    }

    void OnDisable() {
        pool.nextObject = gameObject; //calls set
        if (particles != null) {
            particles.enableEmission = false;
        }
    }
}