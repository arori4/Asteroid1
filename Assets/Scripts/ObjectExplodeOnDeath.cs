using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Defines an explosion at the center of an object.
 */
public class ObjectExplodeOnDeath : MonoBehaviour {

    public List<GameObject> explosionList;

    bool createdYet = false; //simple fix for now for removing first explosion

    void OnDisable() {
        //create explosion if it exists
        if (explosionList.Count > 0 && createdYet) {
            Instantiate(explosionList[Random.Range(0, explosionList.Count)],
                transform.position, transform.rotation);
        }
        else {
            createdYet = true;
        }
    }

}
