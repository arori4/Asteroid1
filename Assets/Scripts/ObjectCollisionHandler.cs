using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Handles all collisions.
 * Also has behavior for death.
 */
public class ObjectCollisionHandler : MonoBehaviour {

    //collision and game health
    public CanCollideWith collideDefinition;
    public float maxHealth = 10;
    public float damageAmount;

    //contact
    public GameObject[] contactEffectList;
    public GameObject[] contactSoundList;

    //death
    public GameObject[] explosionEffectList;
    public GameObject[] explosionSoundList;

    //drops
    public DropPair[] alwaysDrops;
    public DropPair[] sometimesDrops;
    public int maxNonEssentialDrops;

    //States
    bool dropCalculationDone;
    bool startedDeathCoroutine;
    float currentHealth;
    string lastColliderTag = ""; //for keeping tab of score right now
    List<GameObject> dropList = new List<GameObject>();

    //Scripts
    GameObject gameController;
    UIController ui;
    Spawner objectSpawner;

    //Constants
    const float MAX_X_COLLIDE = 10f;


    void Start() {
        //find first game object with tag 
        gameController = GameObject.FindWithTag("GameController");

        //Set scripts from game handler
        ui = gameController.GetComponent<UIController>();
        objectSpawner = gameController.GetComponent<Spawner>();
    }

    void OnEnable() {
        //Set starting variables
        dropCalculationDone = false;
        startedDeathCoroutine = false;
        currentHealth = maxHealth;
        lastColliderTag = "";
        dropList.Clear();

        //calculate drops
        StartCoroutine(CalculateDropsCoroutine());
    }

    void OnTriggerEnter(Collider other) {

        //ignore detectors
        if (CompareTag("Player Missile Detector")) {
            return;
        }
        if (CompareTag("Friend Detector")) {
            return;
        }

        //only collide if definitions say so, and if on the screen
        if (collideDefinition.collidesWith(other) && transform.position.x < MAX_X_COLLIDE) {
            //special consideration for powerups
            if (other.CompareTag("Powerup")) {
                other.transform.root.gameObject.GetComponent<PowerUpHandler>().activate();
            }
            dealDamage(other.transform.root.gameObject);

            //create contact effects, if any
            if (contactEffectList.Length > 0) {
                Pools.Initialize(
                    contactEffectList[Random.Range(0, contactEffectList.Length)],
                    other.transform.position, Quaternion.identity);
            }
            if (contactSoundList.Length > 0) {
                GameObject audio = Pools.Initialize(
                    contactSoundList[Random.Range(0, contactSoundList.Length)],
                    other.transform.position, Quaternion.identity);
                audio.GetComponent<AudioSource>().Play();
            }
        }

    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("GameBoundary")) {
            gameObject.SetActive(false);
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

        //set tags for possible scoring
        lastColliderTag = other.transform.root.tag;
    }


    void Update() {
        //kill when current health <= 0
        if (currentHealth <= 0 && !startedDeathCoroutine) {
            StartCoroutine(DeathCoroutine());
            startedDeathCoroutine = true;
        }
    }


    public void AddHealth(float health) {
        currentHealth = Mathf.Min(currentHealth + health, maxHealth);
    }

    public float GetCurrentHealth() {
        return currentHealth;
    }

    public void Damage(float amount, string otherTag) {
        currentHealth -= amount;
        lastColliderTag = otherTag;
    }


    private IEnumerator DeathCoroutine() {
        //handle score
        if (lastColliderTag.CompareTo("Player Weapon") == 0 ||
            lastColliderTag.CompareTo("Player Missile Detector") == 0) { //easy fix for now
            EnemyScoreInfo scoreInfo = GetComponent<EnemyScoreInfo>();
            if (scoreInfo == null) {
                print("Score info is null for object " + name);
            }
            else {
                int amount = scoreInfo.score;
                ui.AddScore(amount);
            }
        }

        //handle if player dies. more stuff right now is handled by other scripts
        if (tag.CompareTo("Player") == 0) {
            objectSpawner.StopAllCoroutines();
            ui.GameOver();
        }

        //stop if calculations aren't done
        while (!dropCalculationDone) {
            yield return null;
        }

        //drop everything
        for (int index = 0; index < dropList.Count; index++) {
            Vector3 spawnLocation = new Vector3(
                Random.Range(-0.4f, 0.4f) + transform.position.x,
                transform.position.y,
                Random.Range(-0.4f, 0.4f) + transform.position.z
            );
            yield return null;

            GameObject dropSpawned = dropList[index];
            GameObject newObj = Pools.Initialize(dropSpawned, spawnLocation, transform.rotation);
            //DO NOT YIELD HERE, NEED TO INITIALIZE STRAIGHT MOVER

            //If object is a straight mover, then make sure that it goes in a random direction
            ObjectStraightMover straightMover = newObj.GetComponent<ObjectStraightMover>();
            if (straightMover != null) {
                straightMover.wasDropped = true;
            }
            yield return null;
        }

        //Play explosion, if there are any
        if (explosionEffectList.Length > 0) {
            Pools.Initialize(
                explosionEffectList[Random.Range(0, explosionEffectList.Length)],
                transform.position, Quaternion.identity);
        }
        yield return null;

        //Play death sound, if there are any
        if (explosionSoundList.Length > 0) {
            GameObject audio = Pools.Initialize(
                explosionSoundList[Random.Range(0, explosionSoundList.Length)],
                transform.position, Quaternion.identity);
            audio.GetComponent<AudioSource>().Play();
        }

        //finally kill object
        gameObject.SetActive(false);
    }
    

    /**
     * Helper coroutine to calculate drops
     */
    private IEnumerator CalculateDropsCoroutine() {

        //add in all essential drops
        for (int outer = 0; outer < alwaysDrops.Length; outer++) {
            for (int inner = 0; inner < alwaysDrops[outer].numDrops; inner++) {
                dropList.Add(alwaysDrops[outer].obj);
                yield return null;
            }
            yield return null;
        }

        //find total drop frequencies in same loop
        int numNonEssentialDrops = 0;
        int totalDropFrequencies = 0;
        for (int index = 0; index < sometimesDrops.Length; index++) {
            numNonEssentialDrops += sometimesDrops[index].numDrops;
            totalDropFrequencies += sometimesDrops[index].frequency;
            yield return null;
        }
        //calculate the maximum possible number of drops to do, regardless of user settings
        numNonEssentialDrops = Random.Range(0, Mathf.Min(numNonEssentialDrops, maxNonEssentialDrops));

        //add nonessential drops to drop list
        while (numNonEssentialDrops > 0) {
            int chosenFrequency = Random.Range(0, totalDropFrequencies) + 1;
            int chooseIndex = 0;
            while (chosenFrequency > 0) {
                chosenFrequency -= sometimesDrops[chooseIndex].frequency;
                chooseIndex++;
                yield return null;
            }
            chooseIndex--; //correction to choose the correct one b/c it adds stuff

            //choose game object only if there are enough
            //note this biases towards rarity. however, with enough drops, this will not occur as often
            if (sometimesDrops[chooseIndex].numDrops > 0) {
                dropList.Add(sometimesDrops[chooseIndex].obj);
                sometimesDrops[chooseIndex].numDrops--;
            }

            //if no more exist, then continue
            numNonEssentialDrops--;
            yield return null;
        }

        //finally, indicate that routine is done
        dropCalculationDone = true;
    }

} //end class

[System.Serializable]
public class CanCollideWith {

    public bool alien;
    public bool alienWeapon;
    public bool asteroid;
    public bool barrier;
    public bool friend;
    public bool mine;
    public bool player;
    public bool playerWeapon;
    public bool powerup;

    public bool collidesWith(Collider other) {

        if (asteroid) {
            if (other.CompareTag("Asteroid")) {
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

        if (barrier) {
            if (other.CompareTag("Barrier")) {
                return true;
            }
        }

        if (friend) {
            if (other.CompareTag("Friend")) {
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