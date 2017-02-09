using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

/**
 * Pool class, single instance that keeps track of all the pools.
 */
public class Pools : NetworkBehaviour {

    [Header("Meta")]
    public Transform parentTransform;

    [Header("Game Objects")]
    public GameObject[] aliens;
    public GameObject[] alienWeapons;
    public GameObject[] asteroids;
    public GameObject[] barriers;
    public GameObject[] friends;
    public GameObject[] mine;
    public GameObject[] playerWeapons;
    public GameObject[] powerups;
    public GameObject[] ships;

    [Header("Other Objects")]
    public GameObject[] explosions;
    public GameObject[] audios;

    const int NUM_START = 5;
    List<GameObject[]> startingObjectArrays = new List<GameObject[]>();
    List<ObjectPool> pools = new List<ObjectPool>();
    bool started = false;
    static Pools singleton;

    int numFinished = 0;
    int numNeededToStart = 0;

    void Start() {
        //check for single instance
        if (started) {
            print("More than one instance of class Pools was created. " +
                "This one in object " + gameObject);
            return;
        }

        //create pools for everything
        singleton = this;

        //signify started so it only starts once
        started = true;

        //add all the lists to the large list
        startingObjectArrays.Add(aliens);
        startingObjectArrays.Add(alienWeapons);
        startingObjectArrays.Add(asteroids);
        startingObjectArrays.Add(barriers);
        startingObjectArrays.Add(friends);
        startingObjectArrays.Add(mine);
        startingObjectArrays.Add(playerWeapons);
        startingObjectArrays.Add(powerups);
        startingObjectArrays.Add(ships);

        startingObjectArrays.Add(explosions);
        startingObjectArrays.Add(audios);

        //initialize all pools
        for (int outer = 0; outer < startingObjectArrays.Count; outer++) {
            GameObject[] currentArray = startingObjectArrays[outer];
            numNeededToStart++;

            for (int inner = 0; inner < currentArray.Length; inner++) {
                ObjectPool objPool = GetObjectPool(currentArray[inner], false);
                StartCoroutine(objPool.InitializeCoroutine(NUM_START));
            }
        }
    }

    private static ObjectPool GetObjectPool(GameObject obj, bool shouldExistALready) {
        //find object in pools, if it exists already
        ObjectPool objPool = null;
        for (int index = 0; index < singleton.pools.Count; index++) {
            if (singleton.pools[index].sourceObject.Equals(obj)) {
                objPool = singleton.pools[index];

                if (!shouldExistALready) {
                    print(obj + " should not have a pool yet.");
                }
                break;
            }
        }

        //if it doesn't exist, create a new object pool and add it
        if (objPool == null) {
            objPool = new ObjectPool(obj, singleton.isServer);
            singleton.pools.Add(objPool);

            if (shouldExistALready) {
                print(obj + " should have a pool already.");
            }
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
        if (!singleton.isServer) { return null; }
        return Initialize(obj, Vector3.zero, Quaternion.identity, singleton.parentTransform);
    }

    public static GameObject Initialize(GameObject obj, Transform parent) {
        if (!singleton.isServer) { return null; }
        return Initialize(obj, Vector3.zero, Quaternion.identity, parent);
    }

    public static GameObject Initialize(GameObject obj, Vector3 position, Quaternion rotation) {
        if (!singleton.isServer) { return null; }
        return Initialize(obj, position, rotation, singleton.parentTransform);
    }

    public static GameObject Initialize(
        GameObject obj, Vector3 position, Quaternion rotation, Transform parent) {
        if (!singleton.isServer) { return null; }
        if (obj == null) { print("Initialize on a null object"); return null; }

        //Get Object
        ObjectPool objPool = GetObjectPool(obj, obj.GetComponent<PoolMember>());
        GameObject nextObj = objPool.nextObject;

        //Set variables
        nextObj.transform.parent = singleton.parentTransform;
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
            print("Terminate on an already deactivated object " + obj); return;
        }

        ObjectPool objPool = GetObjectPool(obj, obj.GetComponent<PoolMember>());
        obj.GetComponent<PoolMember>().SetObjectInactive();
        objPool.nextObject = obj;
    }



    void OnDestroy() {
        //clear all lists
        pools.Clear();
    }

    [ContextMenu("Sort all by name")]
    void SortArrays() {
        System.Array.Sort(aliens, (a, b) => a.name.CompareTo(b.name));
        System.Array.Sort(alienWeapons, (a, b) => a.name.CompareTo(b.name));
        System.Array.Sort(asteroids, (a, b) => a.name.CompareTo(b.name));
        System.Array.Sort(barriers, (a, b) => a.name.CompareTo(b.name));
        System.Array.Sort(friends, (a, b) => a.name.CompareTo(b.name));
        System.Array.Sort(mine, (a, b) => a.name.CompareTo(b.name));
        System.Array.Sort(playerWeapons, (a, b) => a.name.CompareTo(b.name));
        System.Array.Sort(powerups, (a, b) => a.name.CompareTo(b.name));
        System.Array.Sort(explosions, (a, b) => a.name.CompareTo(b.name));
        System.Array.Sort(audios, (a, b) => a.name.CompareTo(b.name));
    }
}