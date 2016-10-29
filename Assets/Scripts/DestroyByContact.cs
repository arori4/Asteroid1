using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour {

    /* All collisions are triggers */

    public GameObject explosion;

    GameObject gameController;

	// Use this for initialization
	void Start () {
        GameObject obj = GameObject.FindWithTag("GameController"); //find first game object with tag 
        if (obj == null) {
            Debug.Log("Cannot find 'GameController' script");
        }
    }
	
    void OnTriggerEnter(Collider other) {
        
        //destroy if coming into contact with collider
        if (other.CompareTag("Asteroid")) {
            Destroy(gameObject);
            Destroy(other.gameObject);
        }

        else if (other.CompareTag("Player")) {
            Destroy(gameObject);
            Destroy(other.gameObject);
        }

        //Create explosion
        if (explosion != null) {
            Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);
        }

    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("GameBoundary")) {
            Destroy(gameObject);
        }
    }
}
