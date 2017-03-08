using UnityEngine;
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
    [HideInInspector] public bool weaponButtonPressed;

    //shield
    [Header("Shield Settings")]
    public GameObject shieldType;
    GameObject currentShield;
    ShieldInfo shieldInfo;
    bool shieldActivated;
    [HideInInspector] public bool shieldRecharging;
    [HideInInspector] public bool shieldButtonPressed;

    //missile
    [Header("Missile Settings")]
    public GameObject missileType;
    MissileInfo missileInfo;
    int missileCount;
    float missileNextFire;
    [HideInInspector] public bool missileButtonPressed;

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
        SetWeapon(weaponType);
        ChangeShip(shipType);
        SetShield(shieldType);
        if (missileType != null) {
            SetMissile(missileType, 5);
        }
    }

    /**
     * Sets up the UI for the local player
     */
    public override void OnStartLocalPlayer() {
        ui = GameObject.FindGameObjectWithTag("UI Controller").GetComponent<UIController>();
        ui.player = this;

        ui.SetWeaponUI(weaponInfo.weaponIcon, weaponInfo.weaponName);
        if (missileInfo != null) {//sometimes we don't have a missile to start
            ui.SetMissileUI(missileInfo.missileIcon, missileInfo.missileName);
        }
        ui.SetShieldUI(shieldInfo.shieldIcon, shieldInfo.shieldName);
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

            //increase charge time
            weaponCurrentCharge += Time.deltaTime;

            if (Input.GetKey(KeyCode.Space) || weaponButtonPressed) {
                WeaponFire();
            }

            //missile input
            if (Input.GetKey(KeyCode.LeftControl) || missileButtonPressed) {
                MissileFire();
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

    /**
     * Fire weapons and shields
     */
    private void WeaponFire() {
        if (Time.time >= weaponNextFire &&  weaponCurrentCharge > weaponInfo.chargeTime &&
            energy > weaponInfo.energyCost) {
            CmdWeaponFire();
            WeaponUpdateTime();
        }
    }
    [Command]
    private void CmdWeaponFire() {
        //create the weapon projectile
        foreach (Transform gun in gunLocations) {
            GameObject spawnedWeapon = Pools.Initialize(weaponType, gun.position, gun.rotation);
            //set the velocity to be the normal of the gun plane (up should be correct)
            spawnedWeapon.GetComponent<ObjectStraightMover>().SetFinalDirection(gun.up);
        }
        WeaponUpdateTime();
    }
    private void WeaponUpdateTime() {
        //set next fire and energy amount. method split to update on both client and server.
        weaponNextFire = Time.time + weaponInfo.fireRate;
        weaponCurrentCharge = 0;
        energy -= weaponInfo.energyCost;
    }

    private void MissileFire() {
        if (Time.time >= missileNextFire && missileCount > 0) {
            CmdMissileFire();
            MissileUpdateTime();

            //Set ui to show lower missileCount
            ui.missileUIGroup.SetText(missileInfo.missileName + " (" + missileCount + ")");
            if (missileCount == 0) {
                ui.missileUIGroup.Hide();
            }
        }
    }
    [Command]
    private void CmdMissileFire() {
        //create the missile on the main gun.
        GameObject spawnedMissile = Pools.Initialize(missileType, gunLocations[0].position, gunLocations[0].rotation);
        //spawnedMissile.SetActive(true); //line is included to remove warnings for now
    }
    private void MissileUpdateTime() {
        //set next fire and energy amount
        missileNextFire = Time.time + missileInfo.fireRate;
        missileCount--;
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
    
    /**
     * Setting variables with client calls 
     */
    public void SetWeapon(GameObject newWeapon) {
        weaponInfo = newWeapon.GetComponent<WeaponInfo>();
        if (weaponInfo == null) {
            print("Changing weapon to a non weapon. Weapon not assigned.");
            return;
        }
        weaponType = newWeapon;
    }
    [TargetRpc]
    public void TargetChangeWeapon(NetworkConnection conn, string weaponName) {
        //change weapon info by string ref
        GameObject newWeapon = Assets.Weapon(weaponName);
        WeaponInfo info = newWeapon.GetComponent<WeaponInfo>();
        SetWeapon(newWeapon);
        
        //set ui
        ui.ChangeWeaponUI(info.weaponIcon, info.name);
    }

    public void SetMissile(GameObject newMissile, int count) {
        missileInfo = newMissile.GetComponent<MissileInfo>();
        if (missileInfo == null) {
            print("Changing missile to a non missile. Missile not assigned.");
            return;
        }

        missileType = newMissile;
        missileCount = count;
    }
    [TargetRpc]
    public void TargetChangeMissile(NetworkConnection conn, string missileName, int count) {
        //Change missile info by string ref
        GameObject newMissile = Assets.Missile(missileName);
        MissileInfo info = newMissile.GetComponent<MissileInfo>();
        SetMissile(newMissile, count);

        //Replace UI 
        ui.ChangeMissileUI(missileInfo.missileIcon, missileInfo.missileName);
    }

    public void SetShield(GameObject newShield) {
        shieldInfo = newShield.GetComponent<ShieldInfo>();
        if (shieldInfo == null) {
            print("Changing shield to a non shield. Shield not assigned.");
            return;
        }

        shieldType = newShield;
        shieldRecharging = false;
    }
    [TargetRpc]
    public void TargetChangeShield(NetworkConnection conn, string weaponName) {
        //Change shield info by string ref
        GameObject newShield = Assets.Shield(weaponName);
        ShieldInfo info = newShield.GetComponent<ShieldInfo>();
        SetShield(newShield);

        //Replace UI 
        ui.ChangeShieldUI(shieldInfo.shieldIcon, shieldInfo.shieldName);

    }

    //TODO: change this for TargetRPC
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