using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

/**
 * Pool class, single instance that keeps track of all the pools.
 */
public class Pools : NetworkBehaviour {

    [Header("Game Objects")]
    public GameObject[] poolObjects;

    static List<ObjectPool> pools = new List<ObjectPool>();
    static readonly int NUM_START = 5;
    static bool started = false;
    static Pools singleton;

    void Awake() {
        for (int index = 0; index < poolObjects.Length; index++) {
            pools.Add(new ObjectPool(poolObjects[index]));
        }
        singleton = this;
    }

    void Start() {
        //check for single instance
        if (started) {
            print("More than one instance of class Pools was created. " +
                "This one in object " + gameObject + ". Other one in " + singleton.gameObject);
            return;
        }

        //signify started so it only starts once
        started = true;

        if (NetworkServer.active) {
            //initialize all pools
            for (int index = 0; index < poolObjects.Length; index++) {
                StartCoroutine(pools[index].InitializeCoroutine(NUM_START, isServer));
            }
        }
    }

    private static ObjectPool GetObjectPool(GameObject obj, bool shouldExistALready) {
        //find object in pools, if it exists already
        ObjectPool objPool = null;
        for (int index = 0; index < pools.Count; index++) {
            if (pools[index].sourceObject.Equals(obj)) {
                objPool = pools[index];

                if (!shouldExistALready) {
                    print(obj + " should not have a pool yet.");
                }
                break;
            }
        }

        //if it doesn't exist, create a new object pool and add it
        if (objPool == null) {
            objPool = new ObjectPool(obj);
            singleton.StartCoroutine(objPool.InitializeCoroutine(NUM_START, singleton.isServer));
            pools.Add(objPool);
            print(obj + " should have a pool already.");
        }

        return objPool;
    }

    private static ObjectPool GetObjectPool(GameObject obj, PoolMember member) {
        if (member == null) {
            return GetObjectPool(obj, true);
        }

        return member.pool;

    }


    /**
     * Initializes a game object and returns it.
     * If a Game Object doesn't exist, then add it
     * Overloaded for similarity to Instantiate
     */
    public static GameObject Initialize(GameObject obj) {
        return Initialize(obj, Vector3.zero, Quaternion.identity, null);
    }

    public static GameObject Initialize(GameObject obj, Transform parent) {
        return Initialize(obj, Vector3.zero, Quaternion.identity, parent);
    }

    public static GameObject Initialize(GameObject obj, Vector3 position, Quaternion rotation) {
        return Initialize(obj, position, rotation, null);
    }

    public static GameObject Initialize( GameObject obj, Vector3 position, Quaternion rotation, Transform parent) {
        if (!singleton.isServer) { return null; }
        if (obj == null) { print("Initialize on a null object"); return null; }

        //Get Object
        ObjectPool objPool = GetObjectPool(obj, obj.GetComponent<PoolMember>());
        GameObject nextObj = objPool.nextObject;

        //Set variables
        nextObj.transform.parent = parent;
        nextObj.transform.position = position;
        nextObj.transform.rotation = rotation;

        return nextObj;
    }

    /**
     * Terminates an object and sends it back into the pool.
     */
    public static void Terminate(GameObject obj) {
        if (!singleton.isServer) { return; }

        if (obj == null) {
            print("Terminate on a null object");
            return;
        }
        if (obj.GetComponent<PoolMember>() == null) {
            print("Terminate on a non pool member " + obj);
            return;
        }
        if (obj.GetComponent<PoolMember>().isObjectActive == false) {
            print("Terminate on a deactivated object " + obj + " " + obj.GetComponent<NetworkIdentity>().netId);
            return;
        }

        ObjectPool objPool = GetObjectPool(obj, obj.GetComponent<PoolMember>());
        objPool.nextObject = obj;
    }

    void OnDestroy() {
        pools.Clear();
    }

    [ContextMenu("Sort all by name")]
    void SortArrays() {
        System.Array.Sort(poolObjects, (a, b) => a.name.CompareTo(b.name));
    }
    


}