using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

/**
 * Defines movement only for the player
 */
public class PlayerMover : NetworkBehaviour {

    public Boundary boundary;
    public float speed;
    public float tiltFront;
    public float tiltSide;

    public Vector2 sensitivity;
    float inputHoriz = 0;
    float inputVert = 0;

    bool keyboardInput;
    [SyncVar]
    Vector3 currentVelocity = Vector3.zero;
    
	void Start () {
        Input.gyro.enabled = true;

        //set platform for keyboard input
        if (Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.WindowsEditor) {
            keyboardInput = true;
        }
    }

    public override void OnStartLocalPlayer() {
        //GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    void Update() {
        //check if local player
        if (isLocalPlayer) {
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
            CmdSetVelocity(inputHoriz, inputVert);
        }
        
        transform.position += currentVelocity * Time.deltaTime;

        //Set and clap position
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, boundary.left, boundary.right),
            0,
            Mathf.Clamp(transform.position.z, boundary.bottom, boundary.top));

        //Set pitch and yaw, intentionally siwtch z and x
        transform.rotation = Quaternion.Euler(currentVelocity.z * tiltSide, 0.0f, currentVelocity.x * -tiltFront);

    }

    [Command]//super bad implementation
    void CmdSetVelocity(float inputX, float inputZ) {
        currentVelocity.x = inputX;
        currentVelocity.z = inputZ;
        RpcSetVelocity(currentVelocity);
    }

    [ClientRpc]
    void RpcSetVelocity(Vector3 newVelocity) {
        currentVelocity = newVelocity;
    }

}

[System.Serializable]
public struct Boundary {
    public float left;
    public float right;
    public float top;
    public float bottom;
}