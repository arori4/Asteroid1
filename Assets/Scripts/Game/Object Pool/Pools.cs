﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Pool class, single instance that keeps track of all the pools.
 */
public class Pools : MonoBehaviour {

    public GameObject[] aliens;
    public GameObject[] alienWeapons;
    public GameObject[] asteroids;
    public GameObject[] barriers;
    public GameObject[] friends;
    public GameObject[] mine;
    public GameObject[] playerWeapons;
    public GameObject[] powerups;

    public GameObject[] explosions;
    public GameObject[] audios;

    const int NUM_START = 5;
    List<GameObject[]> startingObjectArrays = new List<GameObject[]>();
    List<ObjectPool> pools = new List<ObjectPool>();
    bool started = false;
    static Pools singleton;

    void Start() {
        //check for single instance
        if (started) {
            print("More than one instance of class Pools was created.");
        }
        else {
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

            startingObjectArrays.Add(explosions);
            startingObjectArrays.Add(audios);

            //initialize all pools
            for (int outer = 0; outer < startingObjectArrays.Count; outer++) {
                GameObject[] currentArray = startingObjectArrays[outer];

                for (int inner = 0; inner < currentArray.Length; inner++) {
                    StartCoroutine(EnumerateType(currentArray[inner]));
                }
            }
        }
    }

    /**
     * Initializes a game object and returns it.
     * If a Game Object doesn't exist, then add it
     */
    public static GameObject Initialize(GameObject obj) {
        ObjectPool objPool = GetObjectList(obj, true);

        return objPool.nextObject;
    }

    public static GameObject Initialize(GameObject obj, Vector3 position, Quaternion rotation) {
        GameObject newObj = Initialize(obj);
        newObj.transform.position = position;
        newObj.transform.rotation = rotation;

        return newObj;
    }

    static ObjectPool GetObjectList(GameObject obj, bool shouldExistALready) {
        //find object in pools, if it exists already
        ObjectPool objPool = null;
        for (int index = 0; index < singleton.pools.Count; index++) {
            if (singleton.pools[index].sourceObject.Equals(obj)) {
                objPool = singleton.pools[index];

                if (!shouldExistALready) {
                    print("Object " + obj + " should not exist already, but it already has a pool.");
                }
                break;
            }
        }

        //if it doesn't exist, create a new object pool and add it
        if (objPool == null) {
            objPool = new ObjectPool(obj);
            singleton.pools.Add(objPool);

            if (shouldExistALready) {
                print("Object " + obj + " should exist already but did not have a pool.");
            }

        }

        return objPool;
    }

    void OnDestroy() {
        //clear all lists
        pools.Clear();
    }

    IEnumerator EnumerateType(GameObject obj) {
        yield return null;

        ObjectPool objPool = GetObjectList(obj, false);
    
        StartCoroutine(objPool.InitializeCoroutine(NUM_START));
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