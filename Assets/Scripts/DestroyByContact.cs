using UnityEngine;
using System.Collections;

[System.Serializable]
public class DestroyByContact : MonoBehaviour {

    public CanCollideWith collideDefinition;

    GameObject gameController;
    CollisionHandler explosionHandler;
    
	void Start () {
        gameController = GameObject.FindWithTag("GameController"); //find first game object with tag 
        if (gameController == null) {
            Debug.Log("Cannot find 'GameController' script");
        }

        //Set explosion handler
        explosionHandler = gameController.GetComponent<CollisionHandler>();

    }
	
    void OnTriggerEnter(Collider other) {

        bool deathCollision = false;

        //handle score on player bolt
        if (other.gameObject.CompareTag("Player Bolt") || gameObject.CompareTag("Player Bolt")) {
            explosionHandler.handleScore(gameObject, other.gameObject);
        }

        if (other.CompareTag("Large Asteroid") && collideDefinition.asteroid) {
            Destroy(gameObject);
            Destroy(other.transform.root.gameObject);
            deathCollision = true;
        }

        else if (other.CompareTag("Small Asteroid") && collideDefinition.asteroid) {
            Destroy(gameObject);
            Destroy(other.transform.root.gameObject);
            deathCollision = true;
        }

        else if (other.CompareTag("Player") && collideDefinition.player) {
            Destroy(gameObject);
            Destroy(other.transform.root.gameObject);
            deathCollision = true;
        }

        else if (other.CompareTag("Player Bolt") && collideDefinition.bolt) {
            Destroy(gameObject);
            Destroy(other.transform.root.gameObject);
            deathCollision = true;

        }

        else if (other.CompareTag("Enemy Bolt") && collideDefinition.bolt) {
            Destroy(gameObject);
            Destroy(other.transform.root.gameObject);
            deathCollision = true;
        }

        else if (other.CompareTag("Alien") && collideDefinition.alien) {
            Destroy(gameObject);
            Destroy(other.transform.root.gameObject);
            deathCollision = true;
        }


        if (deathCollision == true) {
            explosionHandler.handleExplosion(gameObject, other.gameObject, other.transform.position);
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
