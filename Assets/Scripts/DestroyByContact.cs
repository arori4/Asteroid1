using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour {

    /* All collisions are triggers */

    GameObject gameController;

	// Use this for initialization
	void Start () {
        GameObject obj = GameObject.FindWithTag("GameController"); //find first game object with tag 
        if (obj == null) {
            Debug.Log("Cannot find 'GameController' script");
        }
    }
	
    void OnTriggerEnter(Collider other) {

        print(other.tag);

        //destroy if coming into contact with collider
        if (other.CompareTag("Asteroid")) {
            Destroy(gameObject);
            Destroy(other.gameObject);
        }

        else if (other.CompareTag("Player")) {
            Destroy(gameObject);
            Destroy(other.gameObject);
        }

    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("GameBoundary")) {
            Destroy(gameObject);
        }
    }
}
