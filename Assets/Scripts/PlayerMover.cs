﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public struct Boundary {
    public float left;
    public float right;
    public float top;
    public float bottom;
}

public class PlayerMover : MonoBehaviour {

    public Boundary boundary;
    public float speed;
    public float tiltFront;
    public float tiltSide;

    public Vector2 sensitivity;

    bool keyboardInput;

    Rigidbody mRigidbody;

	// Use this for initialization
	void Start () {
        mRigidbody = GetComponent<Rigidbody>();
        Input.gyro.enabled = true;

        //set platform for keyboard input
        if (Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.WindowsEditor) {
            keyboardInput = true;
        }
    }
	
	void Update () {

    }

    void FixedUpdate() {

        float moveHorizontal = 0;
        float moveVertical = 0;

        if (keyboardInput) {
            moveHorizontal = Input.GetAxis("Horizontal") * speed;
            moveVertical = Input.GetAxis("Vertical") * speed;
        }
        else {
            moveVertical = (Input.acceleration.y + 0.5f) * sensitivity.y; //added offset
            moveHorizontal = Input.acceleration.x * sensitivity.x;
        }
             
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        mRigidbody.velocity = movement;

        //Clamp position
        mRigidbody.position = new Vector3(
            Mathf.Clamp(mRigidbody.position.x, boundary.left, boundary.right),
            0,
            Mathf.Clamp(mRigidbody.position.z, boundary.bottom, boundary.top));

        //Intentionally siwtch z and x
        mRigidbody.rotation = Quaternion.Euler(mRigidbody.velocity.z * tiltSide, 0.0f, mRigidbody.velocity.x * -tiltFront);
    }
}
