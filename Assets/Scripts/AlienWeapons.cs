using UnityEngine;
using System.Collections;

public class AlienWeapons : MonoBehaviour {

    public GameObject boltType;
    public Transform gun;

    float nextFire;
    float boltFireRate;

    // Use this for initialization
    void Start () {
        //Null Checks
        if (gun == null) {
            Debug.Log("EnemyWeapon gun is null");
        }
        if (boltType == null) {
            Debug.Log("Object with gun has a null bolt type");
        }

        //Set script global values
        nextFire = Time.time;

        //Set bolt specific values
        ChangeWeapon(boltType);
    }
	
	// Update is called once per frame
	void Update () {
        Fire();
	}

    public void Fire() {

        //only fire after a certain time quantum and amount of energy
        if (Time.time >= nextFire) {

            //create the bolt
            GameObject spawnedBolt = Instantiate(boltType, gun.position, gun.rotation) as GameObject;
            //set the velocity to be the normal of the gun plane (up should be correct)
            spawnedBolt.GetComponent<StraightMover>().initialDirection = gun.up;

            //set next fire and energy amount
            nextFire = Time.time + boltFireRate;
        }

    }

    public void ChangeWeapon(GameObject bolt) {
        boltType = bolt;
        boltFireRate = bolt.GetComponent<WeaponInfo>().fireRate;
    }
}
