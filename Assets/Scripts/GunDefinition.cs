using UnityEngine;
using System.Collections;

/**
 * Defines a gun
 * Gun has a single gun location and weapon type
 */
[System.Serializable]
public class GunDefinition {

    public Transform gunLocation;
    public GameObject weaponType;

    Vector2 fireRate; //fire rate is between 100% and 200% of bolt type fire rate
    float nextFire;

    public void Initialize() {
        if (gunLocation == null) {
            Debug.Log("GunDefinition gunLocation is null.");
        }
        if (weaponType == null) {
            Debug.Log("GunDefinition weaponType is null");
        }

        //set variables
        ChangeWeapon(weaponType);
        SetNextFire();
    }

    public void Fire() {
        //create the bolt
        GameObject spawnedBolt = Pools.Initialize(weaponType, gunLocation.position, gunLocation.rotation);

        //set the velocity to be the normal of the gun plane (up should be correct)
        spawnedBolt.GetComponent<ObjectStraightMover>().SetFinalDirection(gunLocation.up);

        SetNextFire();
    }


    public void ChangeWeapon(GameObject weapon) {
        weaponType = weapon;
        fireRate = new Vector2(0, 0);
        fireRate.x = weapon.GetComponent<WeaponInfo>().fireRate;
        fireRate.y = fireRate.x * 2.5f;
    }

    void SetNextFire() {
        nextFire = Random.Range(fireRate.x, fireRate.y);
    }

    public float GetNextFire() {
        return nextFire;
    }
}