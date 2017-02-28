using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/**
 * Randomly rotates the object based on the rotate speed
 */
public class ObjectRandomRotater : NetworkBehaviour {

    public Transform target;

    public float rotateSpeed = 30;
    public bool exactSpeed;

    public bool xRotate = true;
    public bool yRotate = true;
    public bool zRotate = true;
    
    [SyncVar]
    Vector3 rotateAmount;

	void OnEnable () {
        if (!isServer) { return; }
        
        rotateAmount = new Vector3(0, 0, 0);

        if (xRotate) {
            if (exactSpeed) {
                rotateAmount.x = rotateSpeed;
            }
            else {
                rotateAmount.x = Random.Range(-rotateSpeed, rotateSpeed);
            }
        }
        if (yRotate) {
            if (exactSpeed) {
                rotateAmount.y = rotateSpeed;
            }
            else {
                rotateAmount.y = Random.Range(-rotateSpeed, rotateSpeed);
            }
        }
        if (zRotate) {
            if (exactSpeed) {
                rotateAmount.z = rotateSpeed;
            }
            else {
                rotateAmount.z = Random.Range(-rotateSpeed, rotateSpeed);
            }
        }
    }
	
	void Update () {
        target.Rotate(rotateAmount);
    }
}
