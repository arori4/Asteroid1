using UnityEngine;
using System.Collections;

public class RotateScript : MonoBehaviour {

    public float rotateSpeed = 3;

    Rigidbody rb;
    float xRotate;
    float yRotate;
    float zRotate;

	// Use this for initialization
	void Start () {
        rb = FindObjectOfType<Rigidbody>();

        xRotate = Random.Range(-rotateSpeed, rotateSpeed);
        yRotate = Random.Range(-rotateSpeed, rotateSpeed);
        zRotate = Random.Range(-rotateSpeed, rotateSpeed);
    }
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.Rotate(new Vector3(xRotate, yRotate, zRotate));

    }
}
