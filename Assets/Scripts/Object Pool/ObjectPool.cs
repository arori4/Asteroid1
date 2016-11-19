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

            GameObject clone = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
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
        pool.Add(newClone);
        PoolMember member = newClone.AddComponent<PoolMember>();
        member.pool = this;

        //Set object to inactive
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

    void OnDisable() {
        pool.nextObject = gameObject; //calls set
    }
}