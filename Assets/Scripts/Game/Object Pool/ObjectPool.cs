using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/**
 * Defines an object pool for efficient creation
 * Note: this is not a monobehaviour.
 */ 
public class ObjectPool{

    public GameObject sourceObject;
    public NetworkHash128 sourceObjectHash;

    private Stack<GameObject> pool = new Stack<GameObject>();
    private bool isServer;
    
    public ObjectPool(GameObject obj) {
        if (obj == null) {
            Debug.Log("ObjectPool initiated with a null object.");
        }

        sourceObject = obj;
        sourceObjectHash = obj.GetComponent<NetworkIdentity>().assetId;
        
        ClientScene.RegisterSpawnHandler(sourceObjectHash, ClientSpawnHandler, ClientUnSpawnHandler);
    }

    /**
     * Network stuff
     * Cuffently don't know what this is for
     */

    GameObject ClientSpawnHandler(Vector3 position, NetworkHash128 assetId) {
        var go = CreateObject();
        pool.Push(go);
        Debug.LogError("ClientSpawn:  " + go + " " + go.GetInstanceID());
        return go;
    }

    void ClientUnSpawnHandler(GameObject spawned) {
        Debug.LogError("ClientUnSpawn:" + spawned.GetInstanceID());
        spawned.GetComponent<PoolMember>().SetObjectInactive();
    }

    /**
     * Coroutine to initialize stuff
     * We set server here because constructor is set on awake.
     */
    public IEnumerator InitializeCoroutine(int amount, bool server) {
        isServer = server;

        //Add amount objects to the pool
        for (int index = 0; index < amount; index++) {
            nextObject = CreateObject();
            yield return null;
        }
    }

    GameObject CreateObject() {
        GameObject newClone = GameObject.Instantiate(sourceObject) as GameObject;
        
        //add object to pool and set member
        PoolMember member = newClone.AddComponent<PoolMember>();
        member.pool = this;
        
        //Set object to inactive
        member.SetObjectInactive();

        //If server, register the spawn
        if (isServer) {
            NetworkServer.Spawn(newClone);
        }

        //returns gameObject to be used to register client spawn handler in poolmemember
        return newClone;
    }
    
    public GameObject nextObject {

        get {
            if (!isServer) {return null; }
            if (sourceObject == null) {
                Debug.Log("Source object is null.");
                return null;
            }

            //create new object if there are none available
            if (pool.Count < 1) {
                nextObject = CreateObject();
            }

            //Pop an object off the pool
            GameObject clone = pool.Pop();

            //debug check
            if (clone.GetComponent<PoolMember>().isObjectActive) {
                Debug.Log(clone.name + " in pool was still active upon initialization.");
            }
            
            clone.GetComponent<PoolMember>().SetObjectActive();
            return clone;
        }

        //Add new member back on disable
        set {
            if (isServer) {
                pool.Push(value);
            }
        }
    }




}
