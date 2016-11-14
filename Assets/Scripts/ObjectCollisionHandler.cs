using UnityEngine;
using System.Collections;

[System.Serializable]
public class ObjectCollisionHandler : MonoBehaviour {

    public CanCollideWith collideDefinition;
    public GameObject[] explosionList;
    public float maxHealth;
    public float damageAmount;

    //drops
    public DropPair[] drops;
    public DropPair[] alwaysDrops;
    public int maxDrops;

    float currentHealth;
    GameObject gameController;
    UIController ui;
    ObjectSpawner objectSpawner;

    string lastColliderTag = ""; //for keeping tab of score right now


    void Start() {
        //find first game object with tag 
        gameController = GameObject.FindWithTag("GameController");

        //Set current health
        currentHealth = maxHealth;
        if (maxHealth <= 0) {
            print("Object health initialized to " + maxHealth + ", by default will set to 10.");
            currentHealth = maxHealth = 10;
        }

        //Set scripts from game handler
        ui = gameController.GetComponent<UIController>();
        objectSpawner = gameController.GetComponent<ObjectSpawner>();
    }

    void OnTriggerEnter(Collider other) {

        //ignore missile detector
        if (CompareTag("Player Missile Detector")) {
            return;
        }

        if (collideDefinition.collidesWith(other)) {
            if (other.CompareTag("Powerup")) {
                other.transform.root.gameObject.GetComponent<PowerUpHandler>().activate();
            }
            dealDamage(other.transform.root.gameObject);
        }

    }


    void OnTriggerExit(Collider other) {
        if (other.CompareTag("GameBoundary")) {
            Destroy(gameObject);
        }
    }


    /*
     * Deals damage to both objects on collision, by definition
     * Handles death when needed
     */
    private void dealDamage(GameObject other) {
        //Get reference to collider
        ObjectCollisionHandler otherCollider = other.GetComponent<ObjectCollisionHandler>();

        //if no collider exists, then don't deal any damage
        if (otherCollider == null) {
            return;
        }

        //Different behavior if hitting or is the player
        if (tag.CompareTo("Player") == 0) {
            currentHealth -= gameObject.GetComponent<PlayerWeapons>().Hit(otherCollider.damageAmount);
        }
        else {
            currentHealth -= otherCollider.damageAmount;
        }

        if (other.tag.CompareTo("Player") == 0) {
            otherCollider.currentHealth -= other.GetComponent<PlayerWeapons>().Hit(damageAmount);
        }
        else {
            otherCollider.currentHealth -= damageAmount;
        }

        //set tags for possible scoring
        lastColliderTag = other.transform.root.tag;
        otherCollider.lastColliderTag = tag;
    }


    void Update() {
        //kill when current health <= 0
        if (currentHealth <= 0) {

            //create explosion if it exists
            if (explosionList.Length > 0) {
                Instantiate(explosionList[Random.Range(0, explosionList.Length)],
                    transform.position, transform.rotation);
            }

            //handle score
            if (lastColliderTag.CompareTo("Player Weapon") == 0 ||
                lastColliderTag.CompareTo("Player Missile Detector") == 0) { //easy fix for now

                EnemyScoreInfo scoreInfo = GetComponent<EnemyScoreInfo>();
                
                int amount = scoreInfo.score;
                ui.AddScore(amount);
            }

            //handle if player dies
            if (tag.CompareTo("Player") == 0) {
                objectSpawner.StopAllCoroutines();
                ui.GameOver();
            }

            //handle drops
            if (drops.Length > 0 || alwaysDrops.Length > 0) {
                //calculate the maximum possible number of drops to do, regardless of user settings
                //find total drop frequencies in same loop
                int amountOfDrops = 0;
                int dropFrequencies = 0;
                for (int index = 0; index < drops.Length; index++) {
                    amountOfDrops += drops[index].numDrops;
                    dropFrequencies += drops[index].frequency;
                }
                amountOfDrops = Mathf.Min(amountOfDrops, maxDrops + 1);
                amountOfDrops = Random.Range(0, amountOfDrops);

                //instantiate essential drops
                int amountOfAlwaysDrops = 0;
                for (int index = 0; index < alwaysDrops.Length; index++) {
                    amountOfAlwaysDrops += alwaysDrops[index].numDrops;

                    for (int inner = 0; inner < alwaysDrops[index].numDrops; inner++) {
                        //must instantiate drops near the destroyed object, but not all together
                        Vector3 spawnLocation = new Vector3(
                            Random.Range(-0.2f, 0.2f) + transform.position.x,
                            transform.position.y,
                            Random.Range(-0.2f, 0.2f) + transform.position.z);

                        GameObject dropSpawned = alwaysDrops[index].obj;
                        GameObject newObj = Instantiate(dropSpawned, spawnLocation, transform.rotation) as GameObject;

                        //If object is a straight mover, then make sure that it goes in a random direction
                        ObjectStraightMover straightMover = newObj.GetComponent<ObjectStraightMover>();
                        if (straightMover != null) {
                            straightMover.wasDropped = true;
                        }
                    }
                }

                //Adjust amount of drops to always drops
                amountOfDrops -= amountOfAlwaysDrops;

                //instantiate non-essential drops
                while (amountOfDrops > 0) {
                    int chosenFrequency = Random.Range(0, dropFrequencies) + 1;
                    int chooseIndex = 0;
                    while (chosenFrequency > 0) {
                        chosenFrequency -= drops[chooseIndex++].frequency;
                    }
                    chooseIndex--; //correction to choose the correct one b/c it adds stuff

                    //choose game object only if there are enough
                    GameObject dropSpawned;
                    if (drops[chooseIndex].numDrops > 0) {
                        dropSpawned = drops[chooseIndex].obj;
                        drops[chooseIndex].numDrops--;
                    }
                    else { //if no more exist, then continue
                        amountOfDrops--;
                        continue;
                    }

                    //must instantiate drops near the destroyed object, but not all together
                    Vector3 spawnLocation = new Vector3(
                        Random.Range(-0.2f, 0.2f) + transform.position.x,
                        transform.position.y,
                        Random.Range(-0.2f, 0.2f) + transform.position.z);

                    GameObject newObj = Instantiate(dropSpawned, spawnLocation, transform.rotation) as GameObject;

                    //If object is a straight mover, then make sure that it goes in a random direction
                    ObjectStraightMover straightMover = newObj.GetComponent<ObjectStraightMover>();
                    if (straightMover != null) {
                        straightMover.wasDropped = true;
                    }

                    amountOfDrops--;
                }
            }

            //finally kill object
            Destroy(gameObject);
        }
    }


    /*
     * Auxilary functions 
     */

    public void addHealth(float health) {
        currentHealth = Mathf.Min(currentHealth + health, maxHealth);
    }

    public float GetCurrentHealth() {
        return currentHealth;
    }

    public void damage(float amount, string otherTag) {
        currentHealth -= amount;
        lastColliderTag = otherTag;
    }

}

[System.Serializable]
public class CanCollideWith {

    public bool alien;
    public bool alienWeapon;
    public bool asteroid;
    public bool mine;
    public bool player;
    public bool playerWeapon;
    public bool powerup;

    public bool collidesWith(Collider other) {

        if (asteroid) {
            if (other.CompareTag("Large Asteroid") ||
                other.CompareTag("Small Asteroid") ||
                other.CompareTag("Asteroid")) {
                return true;
            }
        }

        if (alien) {
            if (other.CompareTag("Alien")) {
                return true;
            }
        }

        if (alienWeapon) {
            if (other.CompareTag("Alien Weapon")) {
                return true;
            }
        }

        if (mine) {
            if (other.CompareTag("Mine")) {
                return true;
            }
        }

        if (player) {
            if (other.CompareTag("Player")) {
                return true;
            }
        }

        if (playerWeapon) {
            if (other.CompareTag("Player Weapon")) {
                return true;
                //player missile detector is not enumerated here b/c it is not a collision
            }
        }

        if (powerup) {
            if (other.CompareTag("Powerup")) {
                return true;
            }
        }

        return false;
    }

}

[System.Serializable]
public class DropPair {
    public GameObject obj;
    public int frequency;
    public int numDrops;
}
