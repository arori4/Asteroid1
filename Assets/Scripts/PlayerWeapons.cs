using UnityEngine;
using System.Collections;

public class PlayerWeapons : MonoBehaviour {

    public GameObject boltType;
    public Transform gun;
    public float maxEnergy = 100;
    public float rechargeRate = 0;

    float energy;
    float nextFire;

    float boltFireRate;
    float boltEnergyCost;

    bool isRacePressed;

	// Use this for initialization
	void Start () {
        //Null Checks
        if (gun == null) {
            Debug.Log("PlayerWeapon gun is null");
        }
        if (boltType == null) {
            Debug.Log("Object with gun has a null bolt type");
        }

        //Set script global values
        nextFire = Time.time;
        energy = 100;

        //Set bolt specific values
        ChangeWeapon(boltType);
    }
	
	// Update is called once per frame
	void Update () {

	    if (Input.GetKeyDown("space") || isRacePressed) {
            Fire();
        }

        //Update Energy
        energy += rechargeRate * Time.deltaTime;
        energy = Mathf.Min(maxEnergy, energy);
	}

    public void Fire() {

        //only fire after a certain time quantum and amount of energy
        if (Time.time >= nextFire && energy > boltEnergyCost) {

            //create the bolt
            GameObject spawnedBolt = Instantiate(boltType, gun.position, gun.rotation) as GameObject;
            //set the velocity to be the normal of the gun plane (up should be correct)
            spawnedBolt.GetComponent<StraightMover>().initialDirection = gun.up;

            //set next fire and energy amount
            nextFire = Time.time + boltFireRate;
            energy -= boltEnergyCost;

            //Play Sound
        }

    }

    public void ChangeWeapon(GameObject bolt) {
        boltType = bolt;
        boltFireRate = bolt.GetComponent<WeaponInfo>().fireRate;
        boltEnergyCost = bolt.GetComponent<WeaponInfo>().energyCost;
    }

    public float GetEnergy() {
        return energy;
    }

    public void Shield() {

    }



    //For UI Use
    public void onPointerDownRaceButton() {
        isRacePressed = true;
    }
    public void onPointerUpRaceButton() {
        isRacePressed = false;
    }
}
