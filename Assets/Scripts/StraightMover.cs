using UnityEngine;
using System.Collections;

public class StraightMover : MonoBehaviour {

    public Vector3 initialDirection;
    public float speed;

	// Use this for initialization
	void Start () {
        Rigidbody rb = GetComponent<Rigidbody>();

        Vector3 normalizedVelocity = Vector3.Normalize(initialDirection);

        rb.velocity = normalizedVelocity * speed;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
