﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

/**
 * Controls the player's weapons, shields, and missiles
 * Right now, controls anything related to energy
 */
public class PlayerWeapons : NetworkBehaviour {

    public float maxEnergy = 100;
    public float rechargeRate = 1f;
    [SyncVar]
    float energy;
    ObjectCollisionHandler playerCollision;
    UIController ui;

    //weapons
    [Header("Weapon Settings")]
    public GameObject weaponType;
    WeaponInfo weaponInfo;
    float weaponNextFire;
    float weaponCurrentCharge;
    public bool weaponButtonPressed;

    //shield
    [Header("Shield Settings")]
    public GameObject shieldType;
    GameObject currentShield;
    ShieldInfo shieldInfo;
    bool shieldActivated;
    [HideInInspector]
    public bool shieldRecharging;
    public bool shieldButtonPressed;

    //missile
    [Header("Missile Settings")]
    public GameObject missileType;
    MissileInfo missileInfo;
    int missileCount;
    public bool missileButtonPressed;
    float missileNextFire;

    //ship
    [Header("Ship Settings")]
    public GameObject shipType;
    public int numStartingGuns;
    ShipInfo currentShipInfo;
    List<Transform> gunLocations = new List<Transform>();
    List<GameObject> turretObjects = new List<GameObject>();
    public GameObject gunQuad;
    public GameObject turretModel;


    void OnEnable() {
        //Set ship stats
        weaponNextFire = Time.time;
        energy = maxEnergy;

        //Set weapons
        ChangeShip(shipType);
        ChangeWeapon(weaponType, false);
        ChangeShield(shieldType, false);
        ChangeMissile(missileType, 5, false);
    }

    /**
     * Sets up the UI for the local player
     */
    public override void OnStartLocalPlayer() {
        ui = GameObject.FindGameObjectWithTag("UI Controller").GetComponent<UIController>();
        ui.player = this;

        ui.ChangeWeaponUI(weaponInfo, false);
        if (missileInfo != null) {//sometimes we don't have a missile to start
            ui.ChangeMissileUI(missileInfo, false);
        }
        ui.ChangeShieldUI(shieldInfo, false);
    }

    void Start() {
        //Set player variables
        AddGuns(numStartingGuns);
        playerCollision = GetComponent<ObjectCollisionHandler>();
    }

    void Update() {
        //Update Energy to maximum energy
        energy = Mathf.Min(maxEnergy, energy + rechargeRate * Time.deltaTime);

        if (isLocalPlayer) {

            //handle health and energy bar
            ui.healthSlider.val = playerCollision.GetCurrentHealth();
            ui.energySlider.val = energy;

            if (Input.GetKey(KeyCode.Space) || weaponButtonPressed) {
                CmdWeaponFire();
            }

            //missile input
            if (Input.GetKey(KeyCode.LeftControl) || missileButtonPressed) {
                CmdMissileFire();
            }

            //shield input TODO: reduce amount of [Command]s sent
            if (Input.GetKey(KeyCode.LeftShift) || shieldButtonPressed) {
                CmdActivateShield();
            }
            else {
                CmdDeactivateShield();
            }
        }
    }

    [Command]
    public void CmdWeaponFire() {
        //increase charge time
        weaponCurrentCharge += Time.deltaTime;

        //only fire after a certain time quantum and amount of energy
        if (Time.time >= weaponNextFire &&
            weaponCurrentCharge > weaponInfo.chargeTime &&
            energy > weaponInfo.energyCost) {

            //create the weapon
            foreach (Transform gun in gunLocations) {
                GameObject spawnedWeapon = Pools.Initialize(weaponType, gun.position, gun.rotation);
                //set the velocity to be the normal of the gun plane (up should be correct)
                spawnedWeapon.GetComponent<ObjectStraightMover>().SetFinalDirection(gun.up);
            }

            //set next fire and energy amount
            weaponNextFire = Time.time + weaponInfo.fireRate;
            weaponCurrentCharge = 0;
            energy -= weaponInfo.energyCost;
        }
    }

    [Command]
    public void CmdMissileFire() {
        //only fire after a certain time quantum and amount of energy
        if (Time.time >= missileNextFire && missileCount > 0) {

            //create the missile
            GameObject spawnedMissile = Pools.Initialize(missileType, gunLocations[0].position, gunLocations[0].rotation);
            spawnedMissile.SetActive(true); //line is included to remove warnings for now

            //set next fire and energy amount
            missileNextFire = Time.time + missileInfo.fireRate;
            missileCount--;

            //Set ui to show lower missileCount
            ui.missileUIGroup.SetText(missileInfo.missileName + " (" + missileCount + ")");
            if (missileCount == 0) {
                ui.missileUIGroup.Hide();
            }
        }
    }

    [Command]
    public void CmdActivateShield() {
        if (!shieldRecharging && currentShield == null) {
            shieldActivated = true;
            currentShield = Pools.Initialize(shieldType, transform.position, transform.rotation,
                transform.root);
            energy -= shieldInfo.energyDrain * Time.deltaTime;
        }
    }

    [Command]
    public void CmdDeactivateShield() {
        if (!shieldRecharging && currentShield != null) {
            shieldActivated = false;
            Pools.Terminate(currentShield);
            currentShield = null;
        }
    }
    
    public void ChangeWeapon(GameObject newWeapon, bool duringGame) {
        if (newWeapon == null) {
            return;
        }

        weaponInfo = newWeapon.GetComponent<WeaponInfo>();
        if (weaponInfo == null) {
            print("Changing weapon to a non weapon. Weapon not assigned.");
            return;
        }

        weaponType = newWeapon;

        //Replace UI only for local player, and if in game
        if (isLocalPlayer && duringGame) {
            ui.ChangeWeaponUI(weaponInfo, duringGame);
        }
    }

    public void ChangeMissile(GameObject missile, int count, bool duringGame) {
        if (missile == null) {
            return;
        }

        missileInfo = missile.GetComponent<MissileInfo>();
        if (missileInfo == null) {
            print("Changing missile to a non missile. Weapon not assigned.");
            return;
        }

        missileType = missile;
        missileCount = count;

        //Replace UI only for local player, and if in game
        if (isLocalPlayer && duringGame) {
            ui.ChangeMissileUI(missileInfo, duringGame);
        }
    }

    public void ChangeShield(GameObject shield, bool duringGame) {
        if (shield == null) {
            return;
        }

        shieldInfo = shield.GetComponent<ShieldInfo>();
        if (shieldInfo == null) {
            print("Changing shield to a non shield. Shield not assigned.");
            return;
        }

        shieldType = shield;
        shieldRecharging = false;

        //Replace UI only for local player, and if in game
        if (isLocalPlayer && duringGame) {
            ui.ChangeShieldUI(shieldInfo, duringGame);
        }
    }

    public void ChangeShip(GameObject ship) {
        if (ship == null) {
            print("PlayerWeapons Ship is null");
            return;
        }

        currentShipInfo = ship.GetComponent<ShipInfo>();
        if (currentShipInfo == null) {
            print("Changing ship to a non ship. Ship not assigned.");
            return;
        }

        //Remove old ship
        Pools.Terminate(shipType.gameObject);

        //Set ship parent
        shipType = Instantiate(ship, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
        shipType.transform.parent = gameObject.transform;
        shipType.SetActive(true);

        //Clear transform list
        gunLocations.Clear();

        //Assign new amount of guns
        int oldAmtGuns = gunLocations.Count;
        AddGuns(oldAmtGuns);

        //Assign new amount of turrets
        int oldAmtTurrets = turretObjects.Count;
        AddTurret(oldAmtTurrets);
    }

    public void AddGuns(int gunsToAdd) {
        if (!isServer) { return; }

        //Find the maximum amount of guns to add
        int maxGunsAllowed = Mathf.Min(currentShipInfo.gunLocations.Count, gunLocations.Count + gunsToAdd);

        for (int index = gunLocations.Count; index < maxGunsAllowed; index++) {
            Transform newGun = Pools.Initialize(
                gunQuad,
                currentShipInfo.gunLocations[index] + gameObject.transform.position,
                gunQuad.transform.rotation)
                .transform;
            newGun.parent = gameObject.transform;
            gunLocations.Add(newGun);
        }
    }

    public void AddTurret(int turretsToAdd) {
        //Find the maximum amount of turrets to add
        int maxTurretsAllowed = Mathf.Min(currentShipInfo.turretLocations.Count, turretObjects.Count + turretsToAdd);

        for (int index = turretObjects.Count; index < maxTurretsAllowed; index++) {
            GameObject newTurret = Pools.Initialize(
                turretModel,
                currentShipInfo.turretLocations[index] + gameObject.transform.position,
                Quaternion.identity);
            //Add to relevant lists
            turretObjects.Add(newTurret);
            gunLocations.Add(newTurret.GetComponent<TurretTracker>().gunLocation);
            newTurret.transform.parent = gameObject.transform;
        }
    }

    /*
     * Determines hit when a shield is hit
     * Returns 0 on full shield hit
     * Returns remaining damage to health otherwise
     */
    public float Hit(float amount) {
        //should only be called if activated
        if (shieldActivated && !shieldRecharging) {
            energy -= amount * shieldInfo.shieldStrength;

            //return 0 if shield still up, or amount of hit left if weak
            if (energy < 0) {
                CmdDeactivateShield();
                ui.ShieldRecharge(shieldInfo.rechargeTime, this);
                float retval = -energy;
                energy = 0;
                if (isLocalPlayer) {
                    ui.RegisterHit(retval);
                }
                return retval;
            }
            else {
                return 0;
            }
        }

        else {
            if (isLocalPlayer) {
                ui.RegisterHit(amount);
            }
            return amount;
        }
    }

    public void AddEnergy(float add) {
        energy = Mathf.Min(maxEnergy, energy + add);
    }

}