﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

/**
 * Handles all collisions.
 * Also has behavior for death.
 */
public class ObjectCollisionHandler : NetworkBehaviour {
    
    const float MAX_X_COLLIDE = 10f;

    [Header("Collision")]
    public CanCollideWith collideDefinition;
    string lastColliderTag = ""; //for keeping tab of score right now
    
    [Header("Health")]
    public float maxHealth = 10;
    public float damageAmount;
    [SyncVar]
    float currentHealth;
    
    [Header("Contact Settings")]
    public GameObject[] contactEffectList;
    public GameObject[] contactSoundList;
    
    [Header("Death Settings")]
    public GameObject[] explosionEffectList;
    public GameObject[] explosionSoundList;
    
    [Header("Drop Settings")]
    public List<DropPair> alwaysDrops;
    public List<DropPair> sometimesDrops;
    public int maxNonEssentialDrops;
    List<GameObject> dropList = new List<GameObject>();

    //States
    bool dropCalculationDone;
    bool startedDeathCoroutine;

    //Controlller
    static CustomNetworkManager networkManager;

    void OnEnable() {
        //Set starting variables
        dropCalculationDone = false;
        startedDeathCoroutine = false;
        currentHealth = maxHealth;
        lastColliderTag = "";

        if (!isServer) { return; }

        dropList.Clear();
        StartCoroutine(CalculateDropsCoroutine());
    }

    void Start() {
        if (isServer) {
            networkManager = GameObject.FindGameObjectWithTag("GameController")
                .GetComponent<CustomNetworkManager>();
        }
    }

    void OnTriggerEnter(Collider other) {
        //ignore detectors
        if (CompareTag("Player Detector")) {
            return;
        }
        if (CompareTag("Friend Detector")) {
            return;
        }

        //only collide if definitions say so, on the screen, and not already dead
        if (collideDefinition.collidesWith(other) && 
            transform.position.x < MAX_X_COLLIDE &&
            !startedDeathCoroutine) {
            
            //special consideration for powerups
            if (other.CompareTag("Powerup") && isServer) {
                other.transform.parent.gameObject.GetComponent<PowerUpHandler>().CmdActivate(gameObject);
            }
            DealDamage(other.gameObject);

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

            //kill when current health <= 0
            if (isServer && currentHealth <= 0 && !startedDeathCoroutine) {
                startedDeathCoroutine = true;
                StartCoroutine(DeathCoroutine());
            }
        }

    }

    //TODO: replace with a script that checks if out of bounds
    void OnTriggerExit(Collider other) {
        if (isServer && other.CompareTag("GameBoundary")) {
            Pools.Terminate(gameObject);
        }
    }


    /*
     * Deals damage to both objects on collision, by definition
     * Handles death when needed
     */
    private void DealDamage(GameObject other) {
        //Get reference to collider
        ObjectCollisionHandler otherCollider = other.GetComponent<ObjectCollisionHandler>();

        //if no collider exists, then attempt to take from parent
        if (otherCollider == null && other.transform.parent == null) {
            return;
        }
        while (otherCollider == null) {
            other = other.transform.parent.gameObject;
            otherCollider = other.GetComponentInParent<ObjectCollisionHandler>();
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

    }


    public void AddHealth(float health) {
        if (!isServer) {
            return;
        }

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
        //print(gameObject + " " + gameObject.GetComponent<NetworkIdentity>().netId  + " started death coroutine ");
        //only run on server
        //handle score
        if (lastColliderTag.CompareTo("Player Weapon") == 0 ||
            lastColliderTag.CompareTo("Player Missile Detector") == 0) { //easy fix for now
            EnemyScoreInfo scoreInfo = GetComponent<EnemyScoreInfo>();
            if (scoreInfo == null) {
                print("Score info is null for object " + name);
            }
            else {
                int amount = scoreInfo.score;
                networkManager.AddScore(amount);
            }
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

        //If the player died, notifiy the network manager
        if (tag.CompareTo("Player") == 0) {
            networkManager.PlayerKilled();
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

        //finally kill object
        Pools.Terminate(gameObject);
    }
    

    /**
     * Helper coroutine to calculate drops
     */
    private IEnumerator CalculateDropsCoroutine() {

        //add in all essential drops
        for (int outer = 0; outer < alwaysDrops.Count; outer++) {
            int numAlwaysDropsToAdd = (int) Random.Range(alwaysDrops[outer].minMaxFreq.x,
                alwaysDrops[outer].minMaxFreq.y);
            for (int inner = 0; inner < numAlwaysDropsToAdd; inner++) {
                dropList.Add(alwaysDrops[outer].obj);
                yield return null;
            }
        }

        //Clone drop list
        List<DropPair> clonedSometimesDrops = new List<DropPair>(sometimesDrops);
        int numNonEssentialDrops = 0;
        int totalDropFrequencies = 0;
        yield return null;

        //Calculate amount of non essential drops and total frequency
        for (int index = 0; index < clonedSometimesDrops.Count; index++) {
            numNonEssentialDrops += (int)clonedSometimesDrops[index].minMaxFreq.y;
            totalDropFrequencies += (int)clonedSometimesDrops[index].minMaxFreq.z;
            yield return null;
        }

        //calculate the maximum possible number of drops to do, regardless of user settings
        numNonEssentialDrops = Random.Range(0, Mathf.Min(numNonEssentialDrops, maxNonEssentialDrops));

        //add nonessential drops to drop list
        while (numNonEssentialDrops > 0) {
            int chosenFrequency = Random.Range(0, totalDropFrequencies) + 1;
            int chooseIndex = 0;
            while (chosenFrequency > 0) {
                chosenFrequency -= (int)clonedSometimesDrops[chooseIndex].minMaxFreq.z;
                chooseIndex++;
                yield return null;
            }
            chooseIndex--; //correction to choose the correct one b/c it adds stuff

            //add object
            DropPair chosenPair = clonedSometimesDrops[chooseIndex];
            dropList.Add(chosenPair.obj);
            chosenPair.minMaxFreq.y--;

            //remove object from list if maximum dropped is reached
            if (chosenPair.minMaxFreq.y <= 0) {
                totalDropFrequencies -= (int)chosenPair.minMaxFreq.z;
                clonedSometimesDrops.Remove(chosenPair);
            }

            //if no more exist, then continue
            numNonEssentialDrops--;
            yield return null;
        }

        //finally, indicate that routine is done, and make sure death corutine is now false
        dropCalculationDone = true;
        startedDeathCoroutine = false;
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
    public Vector3 minMaxFreq;
}