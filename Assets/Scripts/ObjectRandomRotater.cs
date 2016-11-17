using UnityEngine;
using System.Collections;

/**
 * Randomly rotates the object based on the rotate speed
 */
public class ObjectRandomRotater : MonoBehaviour {

    public float rotateSpeed = 30;
    public bool exactSpeed;

    public bool xRotate = true;
    public bool yRotate = true;
    public bool zRotate = true;

    float xRotateSpeed;
    float yRotateSpeed;
    float zRotateSpeed;
    
    Vector3 rotateAmount;
    Quaternion qRotate;

	void Start () {
        rotateAmount = new Vector3(0, 0, 0);

        if (xRotate) {
            if (exactSpeed) {
                xRotateSpeed = rotateSpeed;
            }
            else {
                xRotateSpeed = Random.Range(-rotateSpeed, rotateSpeed);
            }
        }
        if (yRotate) {
            if (exactSpeed) {
                yRotateSpeed = rotateSpeed;
            }
            else {
                yRotateSpeed = Random.Range(-rotateSpeed, rotateSpeed);
            }
        }
        if (zRotate) {
            if (exactSpeed) {
                zRotateSpeed = rotateSpeed;
            }
            else {
                zRotateSpeed = Random.Range(-rotateSpeed, rotateSpeed);
            }
        }

        //Set rotation amount
        rotateAmount.x = xRotateSpeed * Time.deltaTime;
        rotateAmount.y = yRotateSpeed * Time.deltaTime;
        rotateAmount.z = zRotateSpeed * Time.deltaTime;
    }
	
	void Update () {
        transform.Rotate(rotateAmount);
    }
}
