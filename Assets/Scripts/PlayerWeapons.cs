using UnityEngine;
using System.Collections;

public class PlayerWeapons : MonoBehaviour {

    public GameObject boltType;
    public Transform gun;

    float nextFire; 

	// Use this for initialization
	void Start () {
        nextFire = Time.time;
        if (gun == null) {
            print("PlayerWeapon gun is null");
        }
        if (boltType == null) {
            print("Object with gun has a null bolt type");
        }
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetButton("Fire1") || Input.GetKeyDown("space")) {
            Fire();
        }
	}

    void Fire() {

        //only fire after a certain time quantum
        if (Time.time >= nextFire) {

            //create the bolt
            GameObject spawnedBolt = Instantiate(boltType, gun.position, gun.rotation) as GameObject;
            //set the velocity to be the normal of the gun plane (up should be correct)
            spawnedBolt.GetComponent<StraightMover>().initialDirection = gun.up;

            //set next fire
            nextFire = Time.time + boltType.GetComponent<WeaponFireRate>().fireRate;

            //Play Sound
        }

    }
}
