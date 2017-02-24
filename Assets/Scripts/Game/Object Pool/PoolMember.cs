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
    public bool isObjectActive = false;
    bool locallyActive = true;

    static readonly Vector3 AWAY_LOCATION = new Vector3(-1000f, -1000f, -1000f);

    public ObjectPool pool; //pool this object belongs to
    
    void Update() {
        //Client checks to active or inactive this object locally based on the server version state @isObjectActive
        
        if (NetworkServer.active || !isServer) {
            
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
                if (comps[i].GetType().IsSubclassOf(typeof(MonoBehaviour))) {
                    ((MonoBehaviour)comps[i]).enabled = active;
                }

                if (comps[i].GetType().IsSubclassOf(typeof(Collider))) {
                    ((Collider)comps[i]).enabled = active;
                }

                if (comps[i].GetType().IsSubclassOf(typeof(Collider2D))) {
                    ((Collider2D)comps[i]).enabled = active;
                }

                if (comps[i].GetType().IsSubclassOf(typeof(Renderer)) ||
                    comps[i].GetType() == typeof(Renderer)) {
                    ((Renderer)comps[i]).enabled = active;
                }
                
                if (comps[i].GetType() == typeof(Light)) {
                    ((Light)comps[i]).enabled = active;
                }

                if (comps[i].GetType() == typeof(AudioSource)) {
                    ((AudioSource)comps[i]).enabled = active;
                }
                
            }
        }

        //We deactivate children because there shouldn't be any networking on children.
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(active);
        }

        locallyActive = active; //this was true always before...why?

        if (!active) {
            transform.position = AWAY_LOCATION;
        }
    }

}
