using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Local class that allows lookup by keyword
 * Use RPC or Target Calls to reference
 */
public class Assets : MonoBehaviour{

    public WeaponInfo[] weapons;
    public MissileInfo[] missiles;
    public ShieldInfo[] shields;

    static Assets singleton;

    private void Start() {
        singleton = this;
    }

    public static GameObject Weapon(string name) {

        GameObject retval = null;

        for (int index = 0; index < singleton.weapons.Length; index++) {

            if (singleton.weapons[index].weaponName.Equals(name)) {
                retval = singleton.weapons[index].gameObject;
            }
        }

        if (retval == null) {
            print("Could not find asset " + name + " as a weapon");
        }

        return retval;
    }

    public static GameObject Missile(string name) {

        GameObject retval = null;

        for (int index = 0; index < singleton.missiles.Length; index++) {

            if (singleton.missiles[index].missileName.Equals(name)) {
                retval = singleton.missiles[index].gameObject;
            }
        }

        if (retval == null) {
            print("Could not find asset " + name + " as a missile");
        }

        return retval;
    }

    public static GameObject Shield(string name) {

        GameObject retval = null;

        for (int index = 0; index < singleton.shields.Length; index++) {

            if (singleton.shields[index].shieldName.Equals(name)) {
                retval = singleton.shields[index].gameObject;
            }
        }

        if (retval == null) {
            print("Could not find asset " + name + " as a shield");
        }

        return retval;
    }

}