using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class Boundary {
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

    public float sensitivityX;
    public float sensitivityY;

    Rigidbody mRigidbody;

	// Use this for initialization
	void Start () {
        mRigidbody = GetComponent<Rigidbody>();
        Input.gyro.enabled = true;
    }
	
	void Update () {

    }

    void FixedUpdate() {
        /* This is for keyboard input
        float moveHorizontal = Input.GetAxis("Horizontal") * speed;
        float moveVertical = Input.GetAxis("Vertical") * speed;
        */
        
        float moveVertical = (Input.acceleration.y + 0.4f) * sensitivityY; //added offset
        float moveHorizontal = Input.acceleration.x * sensitivityX;
             
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
