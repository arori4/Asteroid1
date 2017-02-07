﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/**
 * A networked pool object
 */
public class PoolMember : NetworkBehaviour {

    [SyncVar]
    public bool isObjectActive = true;
    bool locallyActive = true;
    public ObjectPool pool; //pool this object belongs to

    ParticleSystem particles;

    void Start() {
        //check for different components
        particles = GetComponent<ParticleSystem>();
    }

    void Update() {
        //Client checks to active or inactive this object locally based on the server version state @isObjectActive
        if (NetworkServer.active && !isServer) {
            if (isObjectActive && !locallyActive) {
                SetObjectActive();
            }
            if (!isObjectActive && locallyActive) {
                SetObjectInactive();
            }
        }
    }


    /**
    * Network stuff
    * Cuffently don't know what this is for
    */

    GameObject ClientSpawnHandler(Vector3 position, NetworkHash128 assetId) {
        var go = pool.CreateObject();
        return go;
    }

    void ClientUnSpawnHandler(GameObject spawned) {
        spawned.GetComponent<PoolMember>().SetObjectInactive();
    }

    void Awake() {
        ClientScene.RegisterSpawnHandler(
            gameObject.GetComponent<NetworkIdentity>().assetId, ClientSpawnHandler, ClientUnSpawnHandler);
    }


    /**
     * Active
     */

    void OnEnable() {
        SetObjectActive();
    }

    private void SetObjectActive() {
        ChangeComponentActive(true);

        if (particles != null) {
            ParticleSystem.EmissionModule emission = particles.emission;
            emission.enabled = true;
        }

        if (isServer) {
            isObjectActive = true;
            RpcSetObjectActive();
        }
    }

    [ClientRpc]
    void RpcSetObjectActive() {
        print("ClientRPC Set Object active");
        ChangeComponentActive(true);
    }


    /**
     * Inactive
     */

    void OnDisable() {
        Invoke("SetObjectInactive", 0.01f);
    }

    public void SetObjectInactive() {
        ChangeComponentActive(false);

        if (particles != null) {
            ParticleSystem.EmissionModule emission = particles.emission;
            emission.enabled = false;
        }

        if (isServer) {
            isObjectActive = false;
            RpcSetObjectInactive();
        }
    }

    [ClientRpc]
    void RpcSetObjectInactive() {
        ChangeComponentActive(false);
    }
    

    /**
     * Changes the activity of inner components, except for NetworkBehaviour.
     * NetworkBehaviour is kept active so that this module can continue to communicate.
     */
    private void ChangeComponentActive(bool active) {
        Component[] comps = GetComponents<Component>();

        for (int i = 0; i < comps.Length; i++) {
            if (comps[i] != this && comps[i].GetType() != typeof(NetworkIdentity)) {
                if (comps[i].GetType().IsSubclassOf(typeof(MonoBehaviour)))
                    ((MonoBehaviour)comps[i]).enabled = active;

                if (comps[i].GetType().IsSubclassOf(typeof(Collider)))
                    ((Collider)comps[i]).enabled = active;

                if (comps[i].GetType().IsSubclassOf(typeof(Collider2D)))
                    ((Collider2D)comps[i]).enabled = active;

                if (comps[i].GetType().IsSubclassOf(typeof(Renderer)))
                    ((Renderer)comps[i]).enabled = active;
            }
        }

        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(active);
        }

        locallyActive = active; //this was true always before...why?

        if (!active) {
            //put back in pool
            pool.nextObject = gameObject;
        }
    }

}