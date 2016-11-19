using UnityEngine;
using System.Collections;

/**
 * Removes HP from an object to kill it
 * Must have ObjectCollisionHandler to use
 */
public class ObjectKillByTime : MonoBehaviour {

    public Vector2 time;
    ObjectCollisionHandler handler;

    void OnEnable () {
        handler = GetComponent<ObjectCollisionHandler>();
        if (handler == null) {
            print("ObjectKillByTime was added to an object without script ObjectCollisionHandler. This script will now stop.");
        }
        else {
            StartCoroutine(KillObject());
        }
	}
	
    private IEnumerator KillObject() {
        yield return new WaitForSeconds(Random.Range(time.x, time.y));

        handler.addHealth(-handler.GetCurrentHealth() - 383283);

        yield return null;
    }


}
