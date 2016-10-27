using UnityEngine;
using System.Collections;

public class PlayerMover : MonoBehaviour {

    public float speed;
    public float tilt;

    Rigidbody mRigidbody;

	// Use this for initialization
	void Start () {
        mRigidbody = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate() {
        float moveHorizontal = Input.GetAxis("Horizontal") * speed;
        float moveVertical = Input.GetAxis("Vertical") * speed;

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        mRigidbody.velocity = movement;

        /*
        mRigidbody.position = new Vector3(
            Mathf.Clamp(mRigidbody.position.x, mBoundary.xMin, mBoundary.xMax),
            0,
            Mathf.Clamp(mRigidbody.position.z, mBoundary.zMin, mBoundary.zMax));
            */
        mRigidbody.rotation = Quaternion.Euler(0.0f, 0.0f, mRigidbody.velocity.x * -tilt);
    }
}
