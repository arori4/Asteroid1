using UnityEngine;
using System.Collections;

/**
 * Randomly rotates the object based on the rotate speed
 */
public class ObjectRandomRotater : MonoBehaviour {

    public float rotateSpeed = 30;

    public bool xRotate = true;
    public bool yRotate = true;
    public bool zRotate = true;

    float xRotateSpeed;
    float yRotateSpeed;
    float zRotateSpeed;

    Vector3 rotation;

	void Start () {
        rotation = new Vector3(0, 0, 0);

        if (xRotate) {
            xRotateSpeed = Random.Range(-rotateSpeed, rotateSpeed);
        }
        if (yRotate) {
            yRotateSpeed = Random.Range(-rotateSpeed, rotateSpeed);
        }
        if (zRotate) {
            zRotateSpeed = Random.Range(-rotateSpeed, rotateSpeed);
        }
    }
	
	void Update () {
        rotation.x = xRotateSpeed * Time.deltaTime;
        rotation.y = yRotateSpeed * Time.deltaTime;
        rotation.z = zRotateSpeed * Time.deltaTime;
        gameObject.transform.Rotate(rotation);

    }
}
