using UnityEngine;
using System.Collections;

public class ObjectRandomRotater : MonoBehaviour {

    public float rotateSpeed = 3;

    public bool xRotate = true;
    public bool yRotate = true;
    public bool zRotate = true;

    float xRotateSpeed;
    float yRotateSpeed;
    float zRotateSpeed;
    
	void Start () {
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
        gameObject.transform.Rotate(new Vector3(xRotateSpeed, yRotateSpeed, zRotateSpeed));

    }
}
