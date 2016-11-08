using UnityEngine;
using System.Collections;

public class PowerUpHandler : MonoBehaviour {

    public HealthPowerup healthDef;
    public EnergyPowerup energyDef;
    public WeaponPowerup powerupDef;

    GameObject player;
    ObjectCollisionHandler playerCollisionHandler;
    PlayerWeapons playerWeapons;

	void Start () {
        player = GameObject.FindWithTag("Player").transform.root.gameObject;
        playerCollisionHandler = player.GetComponent<ObjectCollisionHandler>();
        playerWeapons = player.GetComponent<PlayerWeapons>();
	}

    public void activate() {
        if (healthDef.activated) {
            playerCollisionHandler.addHealth(healthDef.amount);
        }

        if (energyDef.activated) {
            playerWeapons.AddEnergy(energyDef.amount);
        }

        if (powerupDef.activated) {
            playerWeapons.ChangeWeapon(powerupDef.boltType);
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
    public GameObject boltType;
}