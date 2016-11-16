using UnityEngine;
using System.Collections;

/*
 * Defines behaviour for alien weapons and shields (in the future)
 */
public class AlienWeapons : MonoBehaviour {
    
    public GunDefinition[] guns;
    
    void Start () {
        if (guns.Length == 0) {
            print("Alien Weapon defined with no available guns.");
        }

        //Start fire coroutine for each gun
        for (int index = 0; index < guns.Length; index++) {
            guns[index].Initialize();
            StartCoroutine(FireSequence(guns[index]));
        }
        
    }

    IEnumerator FireSequence(GunDefinition gun) {
        while (true) {
            yield return new WaitForSecondsRealtime(gun.GetNextFire());
            gun.Fire();
        }
    }

}

[System.Serializable]
public class GunDefinition {

    public Transform gunLocation;
    public GameObject weaponType;
    Vector2 fireRate; //fire rate is between 100% and 200% of bolt type fire rate
    float nextFire;

    //TODO: on damage gun, have small explosion

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
        GameObject spawnedBolt = GameObject.Instantiate(
            weaponType, gunLocation.position, gunLocation.rotation) as GameObject;

        //set the velocity to be the normal of the gun plane (up should be correct)
        spawnedBolt.GetComponent<ObjectStraightMover>().finalDirection = gunLocation.up;

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
