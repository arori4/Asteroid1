using UnityEngine;
using System.Collections;

/**
 * Removes HP from an object to kill it
 * Must have ObjectCollisionHandler to use
 */
 [RequireComponent(typeof(ObjectCollisionHandler))]
public class ObjectKillByTime : MonoBehaviour {

    public Vector2 time;
    ObjectCollisionHandler handler;

    void OnEnable () {
        handler = GetComponent<ObjectCollisionHandler>();
        StartCoroutine(KillObject());
	}
	
    private IEnumerator KillObject() {
        yield return new WaitForSeconds(Random.Range(time.x, time.y));

        handler.AddHealth(-handler.GetCurrentHealth() - 383283);

        yield return null;
    }

}
