using UnityEngine;
using System.Collections;

/**
 * Defines and handles powerups.
 */
public class PowerUpHandler : MonoBehaviour {

    public HealthPowerup healthDef;
    public EnergyPowerup energyDef;
    public WeaponPowerup changeWeaponDef;
    public MissilePowerup changeMissileDef;
    public SpawnObject spawnObjectDef;

    GameObject player;
    ObjectCollisionHandler playerCollisionHandler;
    PlayerWeapons playerWeapons;

    //KEEP THIS AS START
	void Start () {
        //find player, if it exists. this can run when the player dies, which is a nullptr
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) {
            player = player.transform.root.gameObject;
            playerCollisionHandler = player.GetComponent<ObjectCollisionHandler>();
            playerWeapons = player.GetComponent<PlayerWeapons>();
        }
	}

    public void activate() {
        if (healthDef.activated) {
            playerCollisionHandler.addHealth(healthDef.amount);
        }

        if (energyDef.activated) {
            playerWeapons.AddEnergy(energyDef.amount);
        }

        if (changeWeaponDef.activated) {
            playerWeapons.ChangeWeapon(changeWeaponDef.weaponType);
        }
        if (changeMissileDef.activated) {
            playerWeapons.ChangeMissile(changeMissileDef.missileType, changeMissileDef.amount);
        }
        if (spawnObjectDef.activated) {
            Instantiate(spawnObjectDef.objectToSpawn, transform.position, Quaternion.identity);
        }
    }
	
}

[System.Serializable]
public class PowerupDefinition {
    public bool activated;
}

[System.Serializable]
public class HealthPowerup : PowerupDefinition {
    public float amount;
}

[System.Serializable]
public class EnergyPowerup : PowerupDefinition {
    public float amount;
}

[System.Serializable]
public class WeaponPowerup : PowerupDefinition {
    public GameObject weaponType;
}

[System.Serializable]
public class MissilePowerup : PowerupDefinition {
    public GameObject missileType;
    public int amount;
}

[System.Serializable]
public class SpawnObject : PowerupDefinition {
    public GameObject objectToSpawn;
}