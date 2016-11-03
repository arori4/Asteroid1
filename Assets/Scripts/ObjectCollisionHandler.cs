using UnityEngine;
using System.Collections;

[System.Serializable]
public class ObjectCollisionHandler : MonoBehaviour {

    public CanCollideWith collideDefinition;
    public float maxHealth;
    public float damageAmount;
    public GameObject[] explosionList;

    //drops
    public DropPair[] drops;
    public int maxDrops;

    float currentHealth;
    GameObject gameController;
    UIController ui;
    EnemySpawner enemySpawner;

    string lastColliderTag; //for keeping tab of score right now
    const float DAMAGE_DELAY = 0.5f; //period of time to stay uncollideable
    bool canCollide; //when something collides, there is a period of time that object can't collide again
    

	void Start () {
        //find first game object with tag 
        gameController = GameObject.FindWithTag("GameController"); 
        if (gameController == null) {
            Debug.Log("Cannot find 'GameController' script");
        }

        //Set current health
        currentHealth = maxHealth;

        //Set scripts from game handler
        ui = gameController.GetComponent<UIController>();
        enemySpawner = gameController.GetComponent<EnemySpawner>();

        //Make collisions happen
        canCollide = true;
    }
	
    void OnTriggerEnter(Collider other) {

        if (canCollide) {

            //handle collisions
            if (collideDefinition.asteroid) {
                if (other.CompareTag("Large Asteroid") || 
                    other.CompareTag("Small Asteroid")) {
                    dealDamage(other.transform.root.gameObject);
                }
            }

            if (collideDefinition.alien) {
                if (other.CompareTag("Alien")) {
                    dealDamage(other.transform.root.gameObject);
                }
            }

            if (collideDefinition.alienWeapon) {
                if (other.CompareTag("Alien Bolt")) {
                    dealDamage(other.transform.root.gameObject);
                }
            }

            if (collideDefinition.player) {
                if (other.CompareTag("Player")) {
                    dealDamage(other.transform.root.gameObject);
                }
            }

            if (collideDefinition.playerWeapon) {
                if (other.CompareTag("Player Bolt")) {
                    dealDamage(other.transform.root.gameObject);
                }
            }

            if (collideDefinition.powerup) {
                if (other.CompareTag("Powerup")) {
                    other.transform.root.gameObject.GetComponent<PowerUpHandler>().activate();
                    dealDamage(other.transform.root.gameObject);
                }
            }

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
        ObjectCollisionHandler otherCollider = other.GetComponent<ObjectCollisionHandler>();
        currentHealth -= otherCollider.damageAmount;
        otherCollider.currentHealth -= damageAmount;

        lastColliderTag = other.transform.root.tag;
        otherCollider.lastColliderTag = tag;
    }

    IEnumerator delayNextCollision() {
        canCollide = false;
        yield return new WaitForSeconds(DAMAGE_DELAY);
        canCollide = true;
    }


    public void addHealth(float health) {
        currentHealth = Mathf.Min(currentHealth + health, maxHealth);
    }

    public float GetCurrentHealth() {
        return currentHealth;
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
            if ( lastColliderTag.CompareTo("Player Bolt") == 0 ){ 
                int amount = GetComponent<EnemyScoreInfo>().score;
                ui.AddScore(amount);
            }

            //handle if player dies
            if (tag.CompareTo("Player") == 0) {
                enemySpawner.StopAllCoroutines();
                ui.GameOver();
            }

            //handle drops
            if (drops.Length > 0) {
                //calculate the maximum possible number of drops to do, regardless of user settings
                //find total drop frequencies in same loop
                int amountOfDrops = 0;
                int dropFrequencies = 0;
                for (int index = 0; index < drops.Length; index++) {
                    amountOfDrops += drops[index].numDrops;
                    dropFrequencies += drops[index].frequency;
                }
                amountOfDrops = Mathf.Max(amountOfDrops, maxDrops + 1);
                amountOfDrops = Random.Range(0, amountOfDrops);

                print(amountOfDrops);

                //instantiate drops
                while (amountOfDrops > 0) {
                    int chosenFrequency = Random.Range(0, dropFrequencies) + 1;
                    int chooseIndex = 0;
                    while (chosenFrequency > 0) {
                        chosenFrequency -= drops[chooseIndex++].frequency;
                    }
                    chooseIndex--; //correction to choose the correct one b/c it adds stuff

                    //choose game object
                    GameObject dropSpawned = drops[chooseIndex].obj;

                    //must instantiate drops near the destroyed object, but not all together
                    Vector3 spawnLocation = new Vector3(
                        Random.Range(-0.2f, 0.2f) + transform.position.x,
                        transform.position.y,
                        Random.Range(-0.2f, 0.2f) + transform.position.z);

                    Instantiate(dropSpawned, spawnLocation, transform.rotation);

                    amountOfDrops--;
                }


            }

            //finally kill object
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
    public bool powerup;

}

[System.Serializable]
public class DropPair {
    public GameObject obj;
    public int frequency;
    public int numDrops;
}
