using UnityEngine;
using System.Collections;

/**
 * Handles the missile tracking and homing on different targets
 */
public class MissileTracker : MonoBehaviour {

    public float trackingRange;
    public float acceleration;
    public float trackingSpeed;
    public float trackingTime;

    SphereCollider trackingCollider;
    Rigidbody rb;
    GameObject target;
    bool tracking;

	void Start () {
        trackingCollider = GetComponent<SphereCollider>();
        trackingCollider.radius = trackingRange;

        rb = transform.root.gameObject.GetComponent<Rigidbody>();
        rb.velocity = Vector3.right * 0.2f; //initial velocity is slow

        //Manage tracking
        tracking = true;
        StartCoroutine(TurnOffTracking());
        
	}
	
    void FixedUpdate() {
        //track if there is a target and is set to tracking
        if (tracking && target != null) {
            float currentSpeed = rb.velocity.magnitude;

            //change direction of velocity vector to slightly go towards target
            //vector3 rotateTowards
            Vector3 turnedVelocity = Vector3.Lerp(
                Vector3.Normalize(rb.velocity), 
                Vector3.Normalize(target.transform.position - transform.root.transform.position), 
                trackingSpeed * Time.deltaTime);
            rb.velocity = Vector3.Normalize(turnedVelocity) * currentSpeed;
        } 

        //speed up in the direction it's going
        rb.velocity += Vector3.Normalize(rb.velocity) * acceleration * Time.deltaTime;

        //change rotation
        transform.root.transform.forward = rb.velocity;

    }

    void OnTriggerEnter(Collider other) {
        //Track an alien only if we haven't started
        if (target == null && other.transform.root.gameObject.tag.CompareTo("Alien") == 0) {
            target = other.transform.root.gameObject;
        }
    }

    private IEnumerator TurnOffTracking() {
        yield return new WaitForSeconds(trackingTime);
        tracking = false;
    }
    
}
