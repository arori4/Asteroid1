using UnityEngine;
using System.Collections;

/*
 * Defines behaviour for alien weapons and shields (in the future)
 */
public class AIWeapons : MonoBehaviour {
    
    public GunDefinition[] guns;
    
    void OnEnable () {
        if (guns.Length == 0) {
            print("AI weapon defined with no available guns.");
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
