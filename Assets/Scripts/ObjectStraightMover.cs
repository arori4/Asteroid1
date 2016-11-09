using UnityEngine;
using System.Collections;

public class ObjectStraightMover : MonoBehaviour {

    public Vector3 initialDirection;
    public float speed;
    Vector3 normalizedInitialDirection;

    public bool dropMoveAway;
    public float dropSpeed;
    public float dropTime;
    public bool wasDropped = false;

    Rigidbody rb;

    void Start () {
        rb = GetComponent<Rigidbody>();
        Vector3 resultantDirection = new Vector3(0, 0, 0);
        normalizedInitialDirection = Vector3.Normalize(initialDirection);

        if (dropMoveAway && wasDropped) {
            //choose a random direction to go
            Vector3 newDirection = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
            resultantDirection = Vector3.Normalize(newDirection) * dropSpeed;
        }
        else {
            resultantDirection = Vector3.Normalize(initialDirection);
        }

        rb.velocity = resultantDirection * speed;
	}
	
    void FixedUpdate() {
        rb.velocity = Vector3.Lerp(rb.velocity,
            normalizedInitialDirection,
            Time.deltaTime / speed);
    }

	void Update () {
	    
	}
}
