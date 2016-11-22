using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class TurretTracker : MonoBehaviour {

    public float trackSpeed;
    public float trackRadius;
    public CanCollideWith collideDefinitions;

    GameObject currentTarget;

    void OnEnable() {
        currentTarget = null;
    }

    void Update() {
        if (currentTarget == null || !currentTarget.activeSelf) {
            return;
        }
        else {
            //vector3 rotateTowards
        }
    }

    void OnTriggerEnter(Collider other) {

        //Set target only if the current target is no longer active and definitions allow
        if ((currentTarget == null  || !currentTarget.activeSelf) && 
            collideDefinitions.collidesWith(other)) {
            currentTarget = other.transform.root.gameObject;
        }

    }

}
