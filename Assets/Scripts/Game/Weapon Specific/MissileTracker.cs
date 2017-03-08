using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/**
 * Handles the missile tracking and homing on different targets
 */
 [RequireComponent(typeof(SphereCollider))]
public class MissileTracker : MonoBehaviour {

    public float trackingRange;
    public float acceleration;
    public float trackingSpeed;
    public float trackingTime;
    public float startingSpeed;

    SphereCollider trackingCollider;
    Transform parentTransform;

    Vector3 currentVelocity;
    GameObject target;
    bool tracking;

    void Start() {
        trackingCollider = GetComponent<SphereCollider>();
        trackingCollider.radius = trackingRange;

        //Set parent
        parentTransform = transform.parent;
    }

	void OnEnable () {
        //Set initial velocity
        currentVelocity = Vector3.right * startingSpeed;

        //Manage tracking
        target = null;
        tracking = true;
        StartCoroutine(TurnOffTracking());
	}
	
    void Update() {
        //track if there is a target and is set to tracking
        if (tracking && target != null) {
            float currentSpeed = currentVelocity.magnitude;

            //change direction of velocity vector to slightly go towards target
            //vector3 rotateTowards
            Vector3 turnedVelocity = Vector3.RotateTowards(
                currentVelocity, 
                target.transform.position - parentTransform.position,
                trackingSpeed * Time.deltaTime,
                trackingSpeed * Time.deltaTime);
            currentVelocity = Vector3.Normalize(turnedVelocity) * currentSpeed;
        }

        //speed up in the direction it's going
        currentVelocity += Vector3.Normalize(currentVelocity) * acceleration * Time.deltaTime;

        //change position
        parentTransform.position += currentVelocity * Time.deltaTime;

        //change rotation
        parentTransform.forward = currentVelocity;

    }

    /**
     * Tracking methods
     */

    void OnTriggerEnter(Collider other) {
        //Track an alien only if we haven't started
        if (target == null && other.tag.CompareTo("Alien") == 0) {
            SetTarget(other.gameObject);
        }
    }
    
    void SetTarget(GameObject newTarget) {
        target = newTarget;
    }

    /**
     * Turns off tracking after a set amount of time.
     */
    private IEnumerator TurnOffTracking() {
        yield return new WaitForSeconds(trackingTime);
        tracking = false;
    }
    
}
