using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/**
 * A networked pool object
 * Script is attached to pool objects upon creation
 */
public class PoolMember : NetworkBehaviour {

    [SyncVar]
    public bool isObjectActive;
    bool locallyActive;

    public ObjectPool pool; //pool this object belongs to

    ParticleSystem particles;

    void Start() {
        //check for different components
        particles = GetComponent<ParticleSystem>();

        isObjectActive = false;
        locallyActive = true;
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
     * Active
     */

    public void SetObjectActive() {
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
        ChangeComponentActive(true);
    }


    /**
     * Inactive
     */

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

        //TODO: cache this information
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

                if (comps[i].GetType().IsSubclassOf(typeof(AudioSource)))
                    ((AudioSource)comps[i]).enabled = active;
            }
        }

        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(active);
        }

        locallyActive = active; //this was true always before...why?
    }

}
