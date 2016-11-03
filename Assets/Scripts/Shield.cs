using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {

    public GameObject shieldObject;

    public float shieldStrength;
    public float energyDrain;

    public float damageMultiplier = 1; //use this to make stronger shields
    public float rechargeTime = 2f;

    bool activated;
    bool recharging = false;

	void Start () {
        Deactivate(); //shield always starts deactivated
	}
	
	void Update () {
	
	}

    public void Activate() {
        if (!recharging) {
            activated = true;
            shieldObject.SetActive(true);
        }
    }

    public void Deactivate() {
        activated = false;
        shieldObject.SetActive(false);
    }

    public void UpdateShield(float newStrength) {
        shieldStrength = newStrength;
    }

    public float Hit(float amount) {
        //should only be called if activated
        if (activated) {
            shieldStrength -= amount;

            //return 0 if shield still up, or amount of hit left if weak
            if (shieldStrength < 0) {
                StartCoroutine(Recharge());
                return -shieldStrength;
            }
            else {
                return 0;
            }
        }

        else {
            return 0;
        }
    }

    /*
     * For player use 
     */
    public float Hit(float amount, ref float energy) {
        float retval = 0;

        //should only be called if activated
        if (activated) {
            energy -= amount;

            //return 0 if shield still up, or amount of hit left if weak
            if (shieldStrength < 0) {
                retval = -energy;
                StartCoroutine(Recharge());
            }

            //keep energy above 0
            energy = Mathf.Max(0, energy);
        }

        return retval;
    }

    /*
     * Recharge the shield when it has been destroyed
     */
    IEnumerator Recharge() {
        Deactivate();
        recharging = true;
        yield return new WaitForSeconds(rechargeTime);
        recharging = false;
    }
}
