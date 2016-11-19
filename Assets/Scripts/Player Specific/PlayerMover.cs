using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/**
 * Defines movement only for the player
 */
public class PlayerMover : MonoBehaviour {

    public Boundary boundary;
    public float speed;
    public float tiltFront;
    public float tiltSide;

    public Vector2 sensitivity;
    float inputHoriz = 0;
    float inputVert = 0;

    bool keyboardInput;
    Vector3 currentVelocity = Vector3.zero;

	// Use this for initialization
	void Start () {
        Input.gyro.enabled = true;

        //set platform for keyboard input
        if (Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.WindowsEditor) {
            keyboardInput = true;
        }
    }
	
	void Update () {
        //get input
        if (keyboardInput) {
            inputHoriz = Input.GetAxis("Horizontal") * speed;
            inputVert = Input.GetAxis("Vertical") * speed;
        }
        else {
            inputVert = (Input.acceleration.y + 0.4f) * sensitivity.y; //added offset
            inputHoriz = Input.acceleration.x * sensitivity.x;
        }

        //Set velocity
        currentVelocity.x = inputHoriz;
        currentVelocity.z = inputVert;

        //Set and clap position
        transform.position += currentVelocity * Time.deltaTime;
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, boundary.left, boundary.right),
            0,
            Mathf.Clamp(transform.position.z, boundary.bottom, boundary.top));

        //Set pitch and yaw, intentionally siwtch z and x
        transform.rotation = Quaternion.Euler(currentVelocity.z * tiltSide, 0.0f, currentVelocity.x * -tiltFront);
    }

}

[System.Serializable]
public struct Boundary {
    public float left;
    public float right;
    public float top;
    public float bottom;
}