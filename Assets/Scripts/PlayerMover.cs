using UnityEngine;
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

    Rigidbody mRigidbody;

	// Use this for initialization
	void Start () {
        mRigidbody = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        /*
        if ((Input.GetButton("Fire1") || Input.GetKeyDown("space")) &&
        Time.time > mNextFire) {
            GameObject clone = Instantiate(mShot, mShotSpawnTransform.position, mShotSpawnTransform.rotation) as GameObject;
            mNextFire = Time.time + mFireRate;

            //Play Sound
            GetComponent<AudioSource>().Play();
        }
        */
    }

    void FixedUpdate() {
        float moveHorizontal = Input.GetAxis("Horizontal") * speed;
        float moveVertical = Input.GetAxis("Vertical") * speed;

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
