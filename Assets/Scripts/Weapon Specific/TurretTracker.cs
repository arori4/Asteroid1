using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class TurretTracker : MonoBehaviour {

    public float trackSpeed;
    public float trackRadius;
    public CanCollideWith collideDefinitions;
    public Transform gunLocation;

    SphereCollider tracker;
    GameObject currentTarget;
    Vector3 targetDirection;

    void Start() {
        targetDirection = Vector3.zero;
    }

    void OnEnable() {
        tracker = GetComponent<SphereCollider>();
        tracker.radius = trackRadius;
        currentTarget = null;
    }

    void Update() {
        if (currentTarget == null || !currentTarget.activeSelf) {
            return;
        }
        else {
            targetDirection = currentTarget.transform.position - transform.position;
            transform.right = Vector3.RotateTowards(transform.right, targetDirection, trackSpeed, 1);
        }
    }

    void OnTriggerEnter(Collider other) {
        //Set target only if the current target is no longer active and definitions allow
        if ((currentTarget == null  || !currentTarget.activeSelf) && 
            collideDefinitions.collidesWith(other)) {
            currentTarget = other.transform.root.gameObject;
            print("Tracking enemy " + other);
        }

    }

    void OnTriggerExit(Collider other) {
        //remove tracking if no longer in range
        if (other.transform.root.gameObject == currentTarget) {
            currentTarget = null;
            print("No longer tracking " + other.transform.root.gameObject);
        }
    }

}
