using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour {
    
    public CanCollideWith collideDefinition;

    public GameObject asteroidExplosion;
    public GameObject playerExplosion;

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
        if (other.CompareTag("Asteroid") && collideDefinition.asteroid) {
            Destroy(gameObject);
            Destroy(other.gameObject);
            Instantiate(asteroidExplosion, transform.position, Quaternion.identity);
        }

        else if (other.CompareTag("Player") && collideDefinition.player) {
            Destroy(gameObject);
            Destroy(other.gameObject);
            Instantiate(playerExplosion, transform.position, Quaternion.identity);
        }

        else if (other.CompareTag("Bolt") && collideDefinition.bolt) {
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

[System.Serializable]
public class CanCollideWith {

    public bool asteroid;
    public bool player;
    public bool bolt;

}
