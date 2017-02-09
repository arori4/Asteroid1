using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/**
 * Defines an object pool for efficient creation
 * Note: this is not a monobehaviour.
 */ 
public class ObjectPool {

    public readonly GameObject sourceObject;
    private List<GameObject> pool = new List<GameObject>();

    bool isServer;

    /**
     * Constructor
     * Note that this isn't a MonoBehaviour or NetworkBehaviour
     */
    public ObjectPool(GameObject obj, bool server) {
        if (obj == null) {
            Debug.Log("ObjectPool initiated with a null object.");
        }

        sourceObject = obj;
        isServer = server;
        
        Debug.Log(ClientScene.ready);
        Debug.Log(sourceObject);
        Debug.Log(sourceObject.GetComponent<NetworkIdentity>());
        Debug.Log(sourceObject.GetComponent<NetworkIdentity>().assetId);

        ClientScene.RegisterSpawnHandler(
            sourceObject.GetComponent<NetworkIdentity>().assetId, ClientSpawnHandler, ClientUnSpawnHandler);
    }

    public IEnumerator InitializeCoroutine(int amount) {
        //Add amount objects to the pool
        for (int index = 0; index < amount; index++) {
            CreateObject();
            yield return null;
        }
    }

    public GameObject CreateObject() {
        GameObject newClone = GameObject.Instantiate(sourceObject);

        //add object to pool and set member
        PoolMember member = newClone.AddComponent<PoolMember>();
        member.pool = this;

        //Set object to inactive
        member.SetObjectInactive();

        //Add object to pool
        nextObject = newClone;

        //If server, register the spawn
        if (isServer) {
            NetworkServer.Spawn(newClone);
        }

        //returns gameObject to be used to register client spawn handler in poolmemember
        return newClone;
    }
    
    public GameObject nextObject {

        get {
            if (!isServer) { return null; }
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
            if (clone.GetComponent<PoolMember>().isObjectActive) {
                Debug.Log(clone.name + " in pool was still active upon initialization.");
            }

            pool.Remove(clone);
            clone.GetComponent<PoolMember>().SetObjectActive();
            return clone;
        }

        //Add new member back on disable
        set {
            if (isServer) {
                pool.Add(value);
            }
        }
    }


    /**
     * Network stuff
     * Cuffently don't know what this is for
     */

    GameObject ClientSpawnHandler(Vector3 position, NetworkHash128 assetId) {
        var go = CreateObject();
        return go;
    }

    void ClientUnSpawnHandler(GameObject spawned) {
        spawned.GetComponent<PoolMember>().SetObjectInactive();
    }

}
