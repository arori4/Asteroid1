using UnityEngine;
using System.Collections;

public class Explode : MonoBehaviour {

    public GameObject explosion;
    public Vector2 xBounds;

	void Start () {
	    if (explosion == null) {
            Debug.Log("Explode: no explosion defined for object " + gameObject.ToString());
        }
	}
	
    void OnDestroy() {
        if (transform.position.x > xBounds.x && transform.position.x < xBounds.y) {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }
    }

}
