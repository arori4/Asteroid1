using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Defines an object pool for efficient creation
 */ 
public class ObjectPool {

    public readonly GameObject sourceObject;

    private List<GameObject> pool = new List<GameObject>();

    /**
     * Constructor
     * Note that this isn't a MonoBehaviour or NetworkBehaviour
     */
    public ObjectPool(GameObject obj) {
        if (obj == null) {
            Debug.Log("ObjectPool initiated with a null object.");
        }

        sourceObject = obj;
    }


    public IEnumerator InitializeCoroutine(int amount) {
        //Add amount objects to the pool
        for (int index = 0; index < amount; index++) {
            CreateObject();
            yield return null;
        }
    }

    public GameObject CreateObject() { //this now returns gameObject to be used to register client spawn handler in poolmemember
        GameObject newClone = GameObject.Instantiate(sourceObject);

        //add object to pool and set member
        PoolMember member = newClone.AddComponent<PoolMember>();
        member.CancelInvoke(); //cancels the onEnable invoke
        member.pool = this;

        //Set object to inactive (also adds it to pool)
        newClone.SetActive(false);

        return newClone;
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
            if (clone.activeSelf) {
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




}
