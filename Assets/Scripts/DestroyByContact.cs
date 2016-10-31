using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour {
    
    public CanCollideWith collideDefinition;

    public static GameObject[] asteroidLargeExplosion;
    public static GameObject[] asteroidSmallExplosion;
    public static GameObject playerExplosion;
    public static GameObject alienExplosion;

    GameObject gameController;

	// Use this for initialization
	void Start () {
        GameObject obj = GameObject.FindWithTag("GameController"); //find first game object with tag 
        if (obj == null) {
            Debug.Log("Cannot find 'GameController' script");
        }
    }
	
    void OnTriggerEnter(Collider other) {
        
        if (other.CompareTag("Large Asteroid") && collideDefinition.asteroid) {
            Destroy(gameObject);
            Destroy(other.transform.parent.gameObject);
            Instantiate(asteroidLargeExplosion[Random.Range(0, asteroidLargeExplosion.Length)], 
                transform.position, Quaternion.identity);
        }

        else if (other.CompareTag("Small Asteroid") && collideDefinition.asteroid) {
            Destroy(gameObject);
            Destroy(other.transform.parent.gameObject);
            Instantiate(asteroidSmallExplosion[Random.Range(0, asteroidSmallExplosion.Length)],
                transform.position, Quaternion.identity);
        }


        else if (other.CompareTag("Player") && collideDefinition.player) {
            Destroy(gameObject);
            Destroy(other.transform.parent.gameObject);
            Instantiate(playerExplosion, transform.position, Quaternion.identity);
        }

        else if (other.CompareTag("Bolt") && collideDefinition.bolt) {
            Destroy(gameObject);
            Destroy(other.transform.parent.gameObject);
        }

        else if (other.CompareTag("Alien") && collideDefinition.alien) {
            Destroy(gameObject);
            Destroy(other.transform.parent.gameObject);
            Instantiate(alienExplosion, transform.position, Quaternion.identity);
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
    public bool alien;

}
