﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

/**
 * Controls the player's weapons, shields, and missiles
 */
public class PlayerWeapons : NetworkBehaviour {
    
    public float maxEnergy = 100;
    public float rechargeRate = 1f;
    public CanvasGroup regularUI;
    [SyncVar]
    float energy;
    ObjectCollisionHandler playerCollision;

    //weapons
    [Header("Weapon Settings")]
    public GameObject weaponType;
    public UIButtonGroup weaponUIGroup;
    WeaponInfo weaponInfo;
    float weaponNextFire;
    float weaponCurrentCharge;
    bool weaponButtonPressed;

    //shield
    [Header("Shield Settings")]
    public GameObject shieldType;
    public UIButtonGroup shieldUIGroup;
    ShieldInfo shieldInfo;
    bool shieldActivated;
    bool shieldRecharging;
    bool shieldButtonPressed;

    //missile
    [Header("Missile Settings")]
    public GameObject missileType;
    public UIButtonGroup missileUIGroup;
    MissileInfo missileInfo;
    int missileCount;
    bool missileButtonPressed;
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

    //UI
    [Header("UI")]
    public Sliders sliders;

    //red hit canvas
    public CanvasGroup hitCanvas;
    bool hitCanvasActivated; //lock

    //Constants and single use variables
    private float healthSliderVelocityFront;
    private float energySliderVelocityFront;
    private float healthSliderVelocityBack;
    private float energySliderVelocityBack;
    private const float ENERGY_SLIDER_FRONT_SMOOTH = 0.3f;
    private const float ENERGY_SLIDER_BACK_SMOOTH = 1f;
    private const float HEALTH_SLIDER_FRONT_SMOOTH = 0.7f;
    private const float HEALTH_SLIDER_BACK_SMOOTH = 1.5f;

    void Start () {
        //Set player variables
        playerCollision = GetComponent<ObjectCollisionHandler>();

        //Set UI
        sliders.shield.gameObject.SetActive(false);
        hitCanvasActivated = false;
        hitCanvas.alpha = 0;

        //Post Start
    }

    void OnEnable() {
        //Set ship stats
        weaponNextFire = Time.time;
        energy = maxEnergy;
        
        //Set UI Elements
        if (isLocalPlayer) {
            //move UI to a new parent

            
        }

        //Set weapons
        ChangeShip(shipType);
        ChangeWeapon(weaponType, false);
        ChangeShield(shieldType, false);
        ChangeMissile(missileType, 5, false);

        StartCoroutine(OnPostStart());
    }
	
	void Update () {
        if (!isLocalPlayer) {
            return;
        }

        //weapon input
        if (Input.GetKey(KeyCode.Space) || weaponButtonPressed) {
            WeaponFire();
        }

        //shield input
        if (Input.GetKey(KeyCode.LeftShift) || shieldButtonPressed) {
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

        //handle health bar
        sliders.health.value = Mathf.SmoothDamp(sliders.health.value,
            playerCollision.GetCurrentHealth() / SLIDER_SIZE_DIVIDER,
            ref healthSliderVelocityFront, HEALTH_SLIDER_FRONT_SMOOTH);
        sliders.healthBack.value = Mathf.SmoothDamp(sliders.healthBack.value,
            playerCollision.GetCurrentHealth() / SLIDER_SIZE_DIVIDER,
            ref healthSliderVelocityBack, HEALTH_SLIDER_BACK_SMOOTH);
        //force back slider to be over or at the regular health slider amount
        sliders.healthBack.value = Mathf.Max(sliders.health.value, sliders.healthBack.value);

        //handle energy bar
        sliders.energy.value = Mathf.SmoothDamp(sliders.energy.value, energy / SLIDER_SIZE_DIVIDER,
            ref energySliderVelocityFront, ENERGY_SLIDER_FRONT_SMOOTH);
        sliders.energyBack.value = Mathf.SmoothDamp(sliders.energyBack.value, energy / SLIDER_SIZE_DIVIDER,
            ref energySliderVelocityBack, ENERGY_SLIDER_BACK_SMOOTH);
        //force back slider up
        sliders.energyBack.value = Mathf.Max(sliders.energy.value, sliders.energyBack.value);
	}

    public void WeaponFire() {
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

    public void MissileFire() {
        //only fire after a certain time quantum and amount of energy
        if (Time.time >= missileNextFire && missileCount > 0) {

            //create the missile
            GameObject spawnedMissile = Pools.Initialize(missileType, gunLocations[0].position, gunLocations[0].rotation);
            spawnedMissile.SetActive(true); //line is included to remove warnings for now

            //set next fire and energy amount
            missileNextFire = Time.time + missileInfo.fireRate;
            missileCount--;

            //Set ui to show lower missileCount
            missileUIGroup.SetText(missileInfo.missileName + " (" + missileCount + ")");
            if (missileCount == 0) {
                missileUIGroup.Hide();
            }
        }
    }
    
    public void ActivateShield() {
        if (!shieldRecharging) {
            shieldActivated = true;
            shieldType.SetActive(true);
            energy -= shieldInfo.energyDrain * Time.deltaTime;
        }
    }

    public void DeactivateShield() {
        if (!shieldRecharging) {
            shieldActivated = false;
            shieldType.SetActive(false);
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

        //Set weapon UI
        if (duringGame) {
            weaponUIGroup.ChangeObjectDuringGame(weaponInfo.weaponIcon, weaponInfo.weaponName);
        }
        else {
            weaponUIGroup.SetModel(weaponInfo.weaponIcon);
            weaponUIGroup.SetText(weaponInfo.weaponName);
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

        //Set shield UI
        if (duringGame) {
            shieldUIGroup.ChangeObjectDuringGame(shieldInfo.shieldIcon, shieldInfo.shieldName);
        }
        else {
            shieldUIGroup.SetModel(shieldInfo.shieldIcon);
            shieldUIGroup.SetText(shieldInfo.shieldName);
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

        //Set missile UI
        if (duringGame) {
            missileUIGroup.ChangeObjectDuringGame(missileInfo.missileIcon, missileInfo.missileName);
        }
        else {
            missileUIGroup.SetModel(missileInfo.missileIcon);
            missileUIGroup.SetText(missileInfo.missileName + " (" + missileCount + ")");
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
        shipType.SetActive(false);

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
                StartCoroutine(RechargeShield());
                float retval = -energy;
                energy = 0;
                StartCoroutine(HitCanvasFader(retval));
                return retval;
            }
            else {
                return 0;
            }
        }

        else {
            StartCoroutine(HitCanvasFader(amount));
            return amount;
        }
    }

    private IEnumerator HitCanvasFader(float damage) {
        if (!hitCanvasActivated) {
            hitCanvasActivated = true;
            //cap damage to highest amount
            damage = Mathf.Min(damage, 49.99f);

            //Does not use provided methods because there is a different alpha
            while (hitCanvas.alpha < damage / 50.0f) {
                hitCanvas.alpha += Time.deltaTime * 70.0f / damage;
                yield return null;
            }

            while (hitCanvas.alpha > 0) {
                hitCanvas.alpha -= Time.deltaTime * 35.0f / damage;
                yield return null;
            }

            hitCanvasActivated = false;
        }
    }

    /*
     * Recharge the shield when it has been destroyed
     */
    private IEnumerator RechargeShield() {
        DeactivateShield();
        shieldUIGroup.Hide();

        sliders.shield.gameObject.SetActive(true);
        sliders.shield.value = 0f;
   
        shieldRecharging = true;

        while (sliders.shield.value < 1) {
            sliders.shield.value += Time.deltaTime / shieldInfo.rechargeTime;
            yield return null;
        }

        //keep shield bar up for a second before removing it
        CanvasGroup shieldBarCanvas = sliders.shield.gameObject.GetComponent<CanvasGroup>();
        for (int index = 0; index < 2; index++) {
            while (shieldBarCanvas.alpha > 0) {
                shieldBarCanvas.alpha -= Time.deltaTime / 0.25f;
                yield return null;
            }
            while (shieldBarCanvas.alpha < 1) {
                shieldBarCanvas.alpha += Time.deltaTime / 0.25f;
                yield return null;
            }

        }

        //finallly fade the shield bar
        while (shieldBarCanvas.alpha > 0) {
            shieldBarCanvas.alpha -= Time.deltaTime / 0.25f;
            yield return null;
        }
        //set alpha back to 1 so that when we need it again, it appears
        shieldBarCanvas.alpha = 1;
        sliders.shield.gameObject.SetActive(false);

        shieldUIGroup.Show();
        shieldRecharging = false;
    }
    
    public void AddEnergy(float add) {
        energy = Mathf.Min(maxEnergy, energy + add);
    }

    /**
     * Fades the canvas out to an alpha of 0.
     * Canvas will be transparent
     */
    private IEnumerator FadeOutUI(CanvasGroup canvas, float smoothing) {
        while (canvas.alpha > 0) {
            canvas.alpha -= Time.deltaTime * smoothing;
            yield return null;
        }
    }

    private IEnumerator OnPostStart() {
        yield return new WaitForSeconds(0.2f);
        AddGuns(numStartingGuns);
    }

    void OnDisable() {
        //hide the ui buttons
        weaponUIGroup.Deactivate();
        shieldUIGroup.Deactivate();
        missileUIGroup.Deactivate();

        //set the hit background to 0
        hitCanvas.alpha = 0;

        //set slider values to 0
        sliders.health.value = 0;
        sliders.healthBack.value = 0;
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

[System.Serializable]
public struct Sliders {
    public Slider health;
    public Slider healthBack;
    public Slider energy;
    public Slider energyBack;
    public Slider shield;
}