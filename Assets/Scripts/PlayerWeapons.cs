using UnityEngine;
using System.Collections;

public class PlayerWeapons : MonoBehaviour {

    public Transform gun;
    public float maxEnergy = 100;
    public float rechargeRate = 0;
    float energy;

    public GameObject boltType;
    float boltNextFire;
    float boltFireRate;
    float boltEnergyCost;

    public GameObject shieldType;
    float shieldEnergyDrain;
    float shieldDamageMultiplier;
    float shieldRechargeTime;
    bool shieldIsActivated;
    bool shieldRecharging;

    bool isFireButtonPressed;
    bool isShieldButtonPressed;
    
	void Start () {
        //Null Checks
        if (gun == null) {
            Debug.Log("PlayerWeapon gun is null");
        }
        if (boltType == null) {
            Debug.Log("Object with gun has a null bolt type");
        }

        //Set script global values
        boltNextFire = Time.time;
        energy = 100;

        //Set bolt specific values
        ChangeWeapon(boltType);
        ChangeShield(shieldType);
    }
	
	void Update () {

	    if (Input.GetKeyDown("space") || isFireButtonPressed) {
            Fire();
        }

        Shield();

        //Update Energy
        energy += rechargeRate * Time.deltaTime;
        energy = Mathf.Min(maxEnergy, energy);
	}

    public void Fire() {

        //only fire after a certain time quantum and amount of energy
        if (Time.time >= boltNextFire && energy > boltEnergyCost) {

            //create the bolt
            GameObject spawnedBolt = Instantiate(boltType, gun.position, gun.rotation) as GameObject;
            //set the velocity to be the normal of the gun plane (up should be correct)
            spawnedBolt.GetComponent<StraightMover>().initialDirection = gun.up;

            //set next fire and energy amount
            boltNextFire = Time.time + boltFireRate;
            energy -= boltEnergyCost;
        }
    }
    
    public void Shield() {
        if (isShieldButtonPressed && !shieldRecharging) {
            energy -= shieldEnergyDrain * Time.deltaTime;
        }
    }

    public void ChangeWeapon(GameObject bolt) {
        WeaponInfo info = bolt.GetComponent<WeaponInfo>();
        if (info == null) {
            print("Changing weapon to a non bolt. Weapon not assigned");
            return;
        }

        boltType = bolt;
        boltFireRate = info.fireRate;
        boltEnergyCost = info.energyCost;
    }

    public void ChangeShield(GameObject shield) {
        Shield shieldInfo = shield.GetComponent<Shield>();
        if (shieldInfo == null) {
            print("Changing shield to a non shield. Shield not assigned");
            return;
        }

        shieldType = shield;
        shieldEnergyDrain = shieldInfo.energyDrain;
        shieldDamageMultiplier = shieldInfo.damageMultiplier;
        shieldRechargeTime = shieldInfo.rechargeTime;
        shieldIsActivated = true;
    }

    public float Hit(float amount) {
        //should only be called if activated
        if (shieldIsActivated && !shieldRecharging) {
            energy -= amount;

            //return 0 if shield still up, or amount of hit left if weak
            if (energy < 0) {
                StartCoroutine(Recharge());
                float retval = -energy;
                energy = 0;
                return retval;
            }
            else {
                return 0;
            }
        }

        else {
            return amount;
        }
    }


    /*
     * Recharge the shield when it has been destroyed
     */
    IEnumerator Recharge() {
        shieldIsActivated = false;
        shieldRecharging = true;
        yield return new WaitForSeconds(shieldRechargeTime);
        shieldRecharging = false;
    }

    public float GetEnergy() {
        return energy;
    }

    public void addEnergy(float add) {
        energy = Mathf.Min(maxEnergy, energy + add);
    }

    //For UI Use
    public void onPointerDownRaceButton() {
        isFireButtonPressed = true;
    }
    public void onPointerUpRaceButton() {
        isFireButtonPressed = false;
    }

    public void onShieldButtonDown() {
        isShieldButtonPressed = true;
        shieldType.SetActive(true);
    }
    public void onShieldButtonUp() {
        isShieldButtonPressed = false;
        shieldType.SetActive(false);
    }
}
