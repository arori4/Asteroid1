using UnityEngine;
using System.Collections;

public class PowerUpHandler : MonoBehaviour {

    public HealthPowerup healthDef;
    public EnergyPowerup energyDef;

    GameObject player;

	void Start () {
        player = GameObject.FindWithTag("Player").transform.root.gameObject;
	}

    public void activate() {
        if (healthDef.activated) {
            player.GetComponent<ObjectCollisionHandler>().addHealth(healthDef.amount);
        }

        if (energyDef.activated) {

        }
    }
	
}

[System.Serializable]
public class PowerupDefinition {
    public bool activated;
}

[System.Serializable]
public class HealthPowerup : PowerupDefinition{
    public float amount;
}

[System.Serializable]
public class EnergyPowerup : PowerupDefinition{
    public float amount;
}