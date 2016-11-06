using UnityEngine;
using System.Collections;

public class PlayerWeapons : MonoBehaviour {

    public UIController ui;
    public Transform gun;
    public float maxEnergy = 100;
    public float rechargeRate = 0;
    float energy;

    public GameObject boltType;
    string boltName;
    float boltNextFire;
    float boltFireRate;
    float boltEnergyCost;

    public GameObject shieldType;
    float shieldEnergyDrain;
    float shieldDamageMultiplier;
    float shieldRechargeTime;
    bool shieldIsOnline;
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

        //Set UIController
        ui = GameObject.FindWithTag("GameController").GetComponent<UIController>();

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
            shieldType.SetActive(true);
            energy -= shieldEnergyDrain * Time.deltaTime;
        }
    }

    public void ChangeWeapon(GameObject bolt) {
        WeaponInfo info = bolt.GetComponent<WeaponInfo>();
        if (info == null) {
            print("Changing weapon to a non bolt. Weapon not assigned");
            return;
        }

        //Receive weapon info
        boltType = bolt;
        boltFireRate = info.fireRate;
        boltEnergyCost = info.energyCost;
        boltName = info.name;

        //Change Weapon UI Info
        ui.ChangeWeapon(boltName);

    }

    public void ChangeShield(GameObject shield) {
        Shield shieldInfo = shield.GetComponent<Shield>();
        if (shieldInfo == null) {
            print("Changing shield to a non shield. Shield not assigned");
            return;
        }

        //Receive shield info
        shieldType = shield;
        shieldEnergyDrain = shieldInfo.energyDrain;
        shieldDamageMultiplier = shieldInfo.damageMultiplier;
        shieldRechargeTime = shieldInfo.rechargeTime;
        shieldIsOnline = true;
    }

    /*
     * Determines hit when a shield is hit
     */
    public float Hit(float amount) {
        //should only be called if activated
        if (isShieldButtonPressed && shieldIsOnline && !shieldRecharging) {
            energy -= amount * shieldDamageMultiplier;

            //return 0 if shield still up, or amount of hit left if weak
            if (energy < 0) {
                StartCoroutine(Recharge());
                float retval = -energy;
                energy = 0;
                ui.HitUI(retval);
                return retval;
            }
            else {
                return 0;
            }
        }

        else {
            ui.HitUI(amount);
            return amount;
        }
    }


    /*
     * Recharge the shield when it has been destroyed
     */
    IEnumerator Recharge() {
        shieldIsOnline = false;
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
    }
    public void onShieldButtonUp() {
        isShieldButtonPressed = false;
        shieldType.SetActive(false);
    }
}
