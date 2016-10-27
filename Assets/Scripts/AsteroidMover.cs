using UnityEngine;
using System.Collections;

public class AsteroidMover : MonoBehaviour {

    public float speed;

    Rigidbody rb;
    Transform trans;
    float initalY;
    float initalZ;

    // Use this for initialization
    void Start() {
        trans = FindObjectOfType<Transform>();
        initalY = trans.position.y;
        initalZ = trans.position.z;

        rb = FindObjectOfType<Rigidbody>();
        rb.velocity = Vector3.left * speed;
    }

}
