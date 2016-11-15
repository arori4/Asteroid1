using UnityEngine;
using System.Collections;

public class FriendWeapons : MonoBehaviour {

    public GunDefinition[] guns;

    void Start() {
        if (guns.Length == 0) {
            print("Friend Weapon defined with no available guns.");
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

