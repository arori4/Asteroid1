using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/**
 * Defines and handles powerups.
 */
public class PowerUpHandler : NetworkBehaviour {

    public HealthPowerup healthDef;
    public EnergyPowerup energyDef;
    public WeaponPowerup changeWeaponDef;
    public MissilePowerup changeMissileDef;
    public SpawnObject spawnObjectDef;
    public SpawnTurret spawnTurretDef;
    public SpawnGun spawnGunDef;
    

    public void Activate(GameObject player) {
        ObjectCollisionHandler playerCollisionHandler = player.GetComponent<ObjectCollisionHandler>();
        PlayerWeapons playerWeapons = player.GetComponent<PlayerWeapons>();

        if (healthDef.activated) {
            if (healthDef.increaseMax) {
                playerCollisionHandler.maxHealth += healthDef.amount;
            }
            playerCollisionHandler.AddHealth(healthDef.amount);
        }
        if (energyDef.activated) {
            if (energyDef.increaseMax) {
                playerWeapons.maxEnergy += energyDef.amount;
            }
            playerWeapons.AddEnergy(energyDef.amount);
        }
        if (changeWeaponDef.activated) {
            playerWeapons.ChangeWeapon(changeWeaponDef.weaponType, true);
        }
        if (changeMissileDef.activated) {
            playerWeapons.ChangeMissile(changeMissileDef.missileType, changeMissileDef.amount, true);
        }
        if (spawnObjectDef.activated) {
            Pools.Initialize(spawnObjectDef.objectToSpawn, transform.position, Quaternion.identity);
        }
        if (spawnTurretDef.activated) {
            playerWeapons.AddTurret(1);
        }
        if (spawnGunDef.activated) {
            playerWeapons.AddGuns(1);
        }
    }
	
}

[System.Serializable]
public class PowerupDefinition {
    public bool activated;
}

[System.Serializable]
public class HealthPowerup : PowerupDefinition {
    public bool increaseMax;
    public float amount;
}

[System.Serializable]
public class EnergyPowerup : PowerupDefinition {
    public bool increaseMax;
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

[System.Serializable]
public class SpawnTurret : PowerupDefinition {
}

[System.Serializable]
public class SpawnGun : PowerupDefinition {
}