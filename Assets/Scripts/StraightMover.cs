using UnityEngine;
using System.Collections;

public class StraightMover : MonoBehaviour {

    public Vector3 initalVelocty;
    public float speed;

	// Use this for initialization
	void Start () {
        Rigidbody rb = GetComponent<Rigidbody>();

        Vector3 normalizedVelocity = Vector3.Normalize(initalVelocty);

        rb.velocity = normalizedVelocity * speed;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
