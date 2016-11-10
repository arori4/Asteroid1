using UnityEngine;
using System.Collections;

public class PlayerWeapons : MonoBehaviour {

    public UIController ui;
    public Transform gunLocation;
    public float maxEnergy = 100;
    public float rechargeRate = 1f;
    float energy;

    //weapons
    public GameObject weaponType;
    string weaponName;
    float weaponNextFire;
    float weaponFireRate;
    float weaponEnergyCost;
    bool weaponButtonPressed;

    //shield
    public GameObject shieldType;
    float shieldEnergyDrain;
    float shieldDamageMultiplier;
    float shieldRechargeTime;
    bool shieldActivated;
    bool shieldRecharging;
    bool shieldButtonPressed;

    //missile
    public GameObject missileType;
    string missileName;
    int missileCount;
    float missileFireRate;
    bool missileButtonPressed;
    float missileNextFire;


    void Start () {
        //Null Checks
        if (gunLocation == null) {
            Debug.Log("PlayerWeapon gun is null");
        }
        if (weaponType == null) {
            Debug.Log("Object with gun has a null bolt type");
        }

        //Set UIController
        ui = GameObject.FindWithTag("GameController").GetComponent<UIController>();

        //Set script global values
        weaponNextFire = Time.time;
        energy = maxEnergy;

        //Set bolt specific values
        ChangeWeapon(weaponType);
        ChangeShield(shieldType);
        ChangeMissile(missileType, 5);
    }
	
	void Update () {
        //weapon input
	    if (Input.GetKey(KeyCode.Space) || weaponButtonPressed) {
            WeaponFire();
        }

        //shield input
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || shieldButtonPressed) {
            ActivateShield();
        }
        else {
            DeactivateShield();
        }

        //missile input
        if (Input.GetKey(KeyCode.LeftControl) || missileButtonPressed) {
            MissileFire();
        }

        //Update Energy to maximum energy
        energy += rechargeRate * Time.deltaTime;
        energy = Mathf.Min(maxEnergy, energy);
	}

    public void WeaponFire() {
        //only fire after a certain time quantum and amount of energy
        if (Time.time >= weaponNextFire && energy > weaponEnergyCost) {

            //create the weapon
            GameObject spawnedWeapon = Instantiate(weaponType, gunLocation.position, gunLocation.rotation) as GameObject;
            //set the velocity to be the normal of the gun plane (up should be correct)
            spawnedWeapon.GetComponent<ObjectStraightMover>().initialDirection = gunLocation.up;

            //set next fire and energy amount
            weaponNextFire = Time.time + weaponFireRate;
            energy -= weaponEnergyCost;
        }
    }

    public void MissileFire() {
        //only fire after a certain time quantum and amount of energy
        if (Time.time >= missileNextFire && missileCount > 0) {

            //create the missile
            GameObject spawnedMissile = Instantiate(
                missileType,
                gunLocation.position,
                gunLocation.rotation) as GameObject;
            spawnedMissile.SetActive(true); //line is included to remove warnings for now

            //set next fire and energy amount
            missileNextFire = Time.time + missileFireRate;
            missileCount--;

            //Set ui to show lower missileCount (0 missile count is handled by this method)
            ui.ChangeMissileCount(missileName, missileCount);
        }
    }
    
    public void ActivateShield() {
        if (!shieldRecharging) {
            shieldActivated = true;
            shieldType.SetActive(true);
            energy -= shieldEnergyDrain * Time.deltaTime;
        }
    }

    public void DeactivateShield() {
        if (!shieldRecharging) {
            shieldActivated = false;
            shieldType.SetActive(false);
        }
    }

    public void ChangeWeapon(GameObject bolt) {
        WeaponInfo info = bolt.GetComponent<WeaponInfo>();
        if (info == null) {
            print("Changing weapon to a non bolt. Weapon not assigned.");
            return;
        }

        //Receive weapon info
        weaponType = bolt;
        weaponFireRate = info.fireRate;
        weaponEnergyCost = info.energyCost;
        weaponName = info.weaponName;

        //Change Weapon UI Info
        ui.ChangeWeapon(weaponName, info.weaponIcon);

    }

    public void ChangeShield(GameObject shield) {
        Shield shieldInfo = shield.GetComponent<Shield>();
        if (shieldInfo == null) {
            print("Changing shield to a non shield. Shield not assigned.");
            return;
        }

        //Receive shield info
        shieldType = shield;
        shieldEnergyDrain = shieldInfo.energyDrain;
        shieldDamageMultiplier = shieldInfo.shieldStrength;
        shieldRechargeTime = shieldInfo.rechargeTime;
        shieldRecharging = false;
    }

    public void ChangeMissile(GameObject missile, int count) {
        MissileInfo info = missile.GetComponent<MissileInfo>();

        missileType = missile;
        missileFireRate = info.fireRate;
        missileName = info.name;
        missileCount = count;

        //Change Missile UI Info
        ui.ChangeMissile(missileName, missileCount, info.missileIcon);
    }

    /*
     * Determines hit when a shield is hit
     * Returns 0 on full shield hit
     * Returns remaining damage to health otherwise
     */
    public float Hit(float amount) {
        //should only be called if activated
        if (shieldActivated && !shieldRecharging) {
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
        DeactivateShield();
        ui.StartShieldRecharge(shieldRechargeTime);
        shieldRecharging = true;
        yield return new WaitForSeconds(shieldRechargeTime);
        shieldRecharging = false;
    }

    public float GetEnergy() {
        return energy;
    }

    public void AddEnergy(float add) {
        energy = Mathf.Min(maxEnergy, energy + add);
    }


    //For UI Use
    public void onFireButtonDown() {
        weaponButtonPressed = true;
    }
    public void onFireButtonUp() {
        weaponButtonPressed = false;
    }
    public void onShieldButtonDown() {
        shieldButtonPressed = true;
    }
    public void onShieldButtonUp() {
        shieldButtonPressed = false;
    }
    public void onMissileButtonDown() {
        missileButtonPressed = true;
    }
    public void onMissileButtonUp() {
        missileButtonPressed = false;
    }
}
