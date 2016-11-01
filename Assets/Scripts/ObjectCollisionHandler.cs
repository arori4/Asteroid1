using UnityEngine;
using System.Collections;

[System.Serializable]
public class ObjectCollisionHandler : MonoBehaviour {

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

        if (collideDefinition.asteroid) {
            if (other.CompareTag("Large Asteroid") || other.CompareTag("Small Asteroid")) {
                Destroy(gameObject);
                Destroy(other.transform.root.gameObject);
                deathCollision = true;
            }
        }

        if (collideDefinition.alien) {
            if (other.CompareTag("Alien")) {
                Destroy(gameObject);
                Destroy(other.transform.root.gameObject);
                deathCollision = true;
            }
        }

        if (collideDefinition.alienWeapon) {
            if (other.CompareTag("Alien Bolt")) {
                Destroy(gameObject);
                Destroy(other.transform.root.gameObject);
                deathCollision = true;
            }
        }

        if (collideDefinition.player) {
            if (other.CompareTag("Player")) {
                Destroy(gameObject);
                Destroy(other.transform.root.gameObject);
                deathCollision = true;
            }
        }
        
        if (collideDefinition.playerWeapon) {
            if (other.CompareTag("Player Bolt")) {
                Destroy(gameObject);
                Destroy(other.transform.root.gameObject);
                deathCollision = true;
            }
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

    public bool alien;
    public bool alienWeapon;
    public bool asteroid;
    public bool player;
    public bool playerWeapon;

}
