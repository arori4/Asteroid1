using UnityEngine;
using System.Collections;

public class MissileTracker : MonoBehaviour {

    public float trackingRange;
    public float acceleration;
    public float trackingSpeed;

    SphereCollider trackingCollider;
    Rigidbody rb;
    GameObject target;

	void Start () {
        trackingCollider = GetComponent<SphereCollider>();
        trackingCollider.radius = trackingRange;

        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.right * 0.2f; //initial velocity is slow
        
	}
	
    void FixedUpdate() {
        
        //track if there is a target
        if (target != null) {
            float currentSpeed = rb.velocity.magnitude;

            //change direction of velocity vector to slightly go towards target
            //vector3 rotateTowards
            Vector3 turnedVelocity = Vector3.Lerp(
                Vector3.Normalize(rb.velocity), 
                Vector3.Normalize(target.transform.position - transform.position), 
                trackingSpeed * Time.deltaTime);
            rb.velocity = Vector3.Normalize(turnedVelocity) * currentSpeed;
        } 

        //speed up in the direction it's going
        rb.velocity += Vector3.Normalize(rb.velocity) * acceleration * Time.deltaTime;

        //change rotation
        transform.forward = rb.velocity;

    }

    void OnTriggerEnter(Collider other) {
        //Track an alien only if we haven't started
        if (target == null && other.transform.root.gameObject.tag.CompareTo("Alien") == 0) {
            print("Missile is tracking Alien");

            target = other.transform.root.gameObject;
        }
    }
    
}
