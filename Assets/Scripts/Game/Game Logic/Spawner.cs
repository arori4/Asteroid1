using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Handles spawning and level
 */
public class Spawner : MonoBehaviour {

    //Constants
    const int LEVEL_MODE = 0;
    const int SURVIVAL_MODE = 1;

    [Header("Spawn Location")]
    public Vector2 spawnVerticalLocation = new Vector2(-4.5f, 4.5f);
    public Vector2 spawnXZLocation;
    public float spawnBeginningWait;

    //Enemies
    [Header("Spawn Definitions")]
    public SpawnClass enemyClass;
    public SpawnClass enemyWeaponClass;
    int numEnemiesLeft;
    int maxEnemiesSpawnAtTime;
    //Asteroids
    public SpawnClass asteroidClass;
    public List<Material> asteroidMaterials;
    bool continueSpawningAsteroids;
    int maxAsteroidsSpawnAtTime;
    float asteroidSpeed;
    //Powerups
    public SpawnClass powerupClass;

    //Locks
    bool recalculationFinished;

    [Header("Network Manager")]
    public CustomNetworkManager manager;

    
    public void StartSpawning(int startlevel, int mode) {
        SetLevelProperties(startlevel, mode);

        recalculationFinished = false;
        continueSpawningAsteroids = true;
        StartCoroutine(SpawnEnemiesCoroutine());
        StartCoroutine(SpawnAsteroidsCoroutine());
        StartCoroutine(SpawnPowerupsCoroutine());
    }

    /**
     * Sets the level, and level properties
     */
    public void SetLevelProperties(int newLevel, int mode) {

        //check level
        if (newLevel <= 0) {
            print("level is less than 0, by default will set back to 1");
            newLevel = 1;
        }

        //Set base numenemies left
        if (mode == LEVEL_MODE) {
            numEnemiesLeft = 5 + 3 * newLevel;
        }
        else if (mode == SURVIVAL_MODE) {
            numEnemiesLeft = 100000000;
        }

        //level dependent attributes
        asteroidSpeed = 1 + newLevel * 0.2f;

        //attributes for each level
        if (newLevel >= 1) {
            enemyClass.SetWaitTime(4.0f, 5.0f);
            asteroidClass.SetWaitTime(1.5f, 2.5f);
            powerupClass.SetWaitTime(5f, 10f);
            maxAsteroidsSpawnAtTime = 1;
            maxEnemiesSpawnAtTime = 1;
        }
        if (newLevel >= 5) {
            enemyClass.SetWaitTime(3.0f, 4.0f);
            asteroidClass.SetWaitTime(1.25f, 2.0f);
        }
        if (newLevel >= 7) {
            enemyClass.SetWaitTime(2.5f, 3.25f);
            asteroidClass.SetWaitTime(1.25f, 1.75f);
            maxAsteroidsSpawnAtTime = 2;
        }
        if (newLevel >= 10) {
            enemyClass.SetWaitTime(2.5f, 3.0f);
            asteroidClass.SetWaitTime(1.0f, 1.5f);
            powerupClass.SetWaitTime(4.5f, 9f);
            maxEnemiesSpawnAtTime = 2;
        }
        if (newLevel >= 15) {
            enemyClass.SetWaitTime(2.0f, 2.5f);
            asteroidClass.SetWaitTime(0.75f, 1.25f);
            maxAsteroidsSpawnAtTime = 3;
        }
        if (newLevel >= 20) {
            enemyClass.SetWaitTime(1.75f, 2.0f);
            asteroidClass.SetWaitTime(0.65f, 1.0f);
            powerupClass.SetWaitTime(4f, 8f);
        }
        if (newLevel >= 25) {
            enemyClass.SetWaitTime(1.5f, 1.75f);
            asteroidClass.SetWaitTime(0.5f, 0.85f);
            maxAsteroidsSpawnAtTime = 4;
            maxEnemiesSpawnAtTime = 3;
        }

        StartCoroutine(RecalculateFrequencies(newLevel));
    }

    private IEnumerator SpawnEnemiesCoroutine() {
        yield return new WaitForSeconds(spawnBeginningWait);

        while (numEnemiesLeft > 0) {
            if (!recalculationFinished) {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            //Chose an amount of enemies to spawn at a time
            int numEnemiesToSpawn = (int)Random.Range(1, maxEnemiesSpawnAtTime);

            for (int index = 0; index < numEnemiesToSpawn; index++) {

                //Choose enemy to spawn
                GameObject enemyToSpawn = enemyClass.NextObject();
                yield return null;

                //Spawn enemy
                GameObject newEnemy = SpawnInWave(enemyToSpawn);
                numEnemiesLeft--;
                yield return null;

                //Set different enemy guns if alien
                if (newEnemy.CompareTag("Alien")) {
                    AIWeapons weapons = newEnemy.GetComponent<AIWeapons>();
                    GunDefinition[] guns = null;
                    if (weapons != null) {
                        guns = weapons.guns;
                    }
                    else {
                        weapons = newEnemy.GetComponentInChildren<AIWeapons>();
                        guns = weapons.guns;
                    }
                    //TODO: correct implementation

                    for (int gunIndex = 0; gunIndex < guns.Length; gunIndex++) {
                        //Choose random gun
                        GameObject gunToSet = enemyWeaponClass.NextObject();
                        yield return null;

                        guns[gunIndex].ChangeWeapon(gunToSet);
                        yield return null;
                    }
                }

                yield return new WaitForSeconds(enemyClass.NextWaitTime()); //pause
            }
        }

        //advance when no more enemies exist
        continueSpawningAsteroids = false;
        yield return new WaitForSeconds(5);

        manager.AdvanceLevel();
    }


    /**
     * Coroutine for spawning asteroids.
     */
    private IEnumerator SpawnAsteroidsCoroutine() {
        yield return new WaitForSeconds(spawnBeginningWait);

        while (continueSpawningAsteroids) {
            if (!recalculationFinished) {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            //Chose an amount of asteroids to spawn at a time
            int numAsteroidsToSpawn = Random.Range(1, maxAsteroidsSpawnAtTime);
            for (int index = 0; index < numAsteroidsToSpawn; index++){
                //Spawn asteroid
                GameObject spawnedEnemy = SpawnInWave(asteroidClass.NextObject(), asteroidMaterials);
                spawnedEnemy.GetComponent<ObjectStraightMover>().speed = asteroidSpeed;
                yield return null;
            }

            //Wait for new asteroid to create
            yield return new WaitForSeconds(asteroidClass.NextWaitTime());
        }
    }


    /**
     * Coroutine for spawning powerups
     */
    private IEnumerator SpawnPowerupsCoroutine() {
        yield return new WaitForSeconds(powerupClass.NextWaitTime());

        while (numEnemiesLeft > 0) {
            if (!recalculationFinished) {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            //Only spawn one powerup at a time
            GameObject powerupToSpawn = powerupClass.NextObject();
            yield return null;

            //Spawn powerup
            SpawnInWave(powerupToSpawn);
            yield return new WaitForSeconds(powerupClass.NextWaitTime());
        }
    }

    private IEnumerator RecalculateFrequencies(int level) {
        enemyClass.Recalculate(level);
        yield return null;
        enemyWeaponClass.Recalculate(level);
        yield return null;
        asteroidClass.Recalculate(level);
        yield return null;
        powerupClass.Recalculate(level);
        yield return null;

        recalculationFinished = true;
    }

    /**
     * Spawn handling
     */
    private GameObject SpawnInWave(GameObject obj) {

        Vector3 spawnPosition = new Vector3(spawnXZLocation.x, spawnXZLocation.y,
            Random.Range(spawnVerticalLocation.x, spawnVerticalLocation.y));
        GameObject spawnedObj = Pools.Initialize(obj, spawnPosition, Quaternion.identity);
        
        return spawnedObj;
    }
    private GameObject SpawnInWave(GameObject obj, List<Material> materials) {
        GameObject spawnedObj = SpawnInWave(obj);

        spawnedObj.GetComponentInChildren<Renderer>().material =
            materials[Random.Range(0, materials.Count)];

        return spawnedObj;
    }

    [ContextMenu("Sort by Level Appearance")]
    void SortEnemies() {
        enemyClass.originalList.Sort((x, y) => (x.LevelAppearance() - y.LevelAppearance()));
        enemyWeaponClass.originalList.Sort((x, y) => (x.LevelAppearance() - y.LevelAppearance()));
        asteroidClass.originalList.Sort((x, y) => (x.LevelAppearance() - y.LevelAppearance()));
        powerupClass.originalList.Sort((x, y) => (x.LevelAppearance() - y.LevelAppearance()));
        print("Sorted enemy lists");
    }
}

[System.Serializable]
public class SpawnObjDef {

    public GameObject obj;
    public Vector2 levappFreq;

    public int LevelAppearance() {
        return (int)levappFreq.x;
    }

    public int Frequency() {
        return (int)levappFreq.y;
    }
}

[System.Serializable]
public class SpawnClass {

    public List<SpawnObjDef> originalList;

    Vector2 waitTime = new Vector2();
    List<SpawnObjDef> currentList = new List<SpawnObjDef>();
    int totalFrequency = 0;

    /**
     * Sets the wait time for the next object
     */
    public void SetWaitTime(float min, float max) {
        waitTime.x = min;
        waitTime.y = max;
    }

    /**
     * Chooses a random object based on the pair list given.
     */
    public GameObject NextObject() {
        int chosenFrequency = Random.Range(0, totalFrequency) + 1;
        int chooseIndex = 0;
        while (chosenFrequency > 0) {
            chosenFrequency -= currentList[chooseIndex++].Frequency();
        }
        chooseIndex--; //correction to choose the correct one b/c it adds stuff

        return currentList[chooseIndex].obj;
    }

    /**
     * Recalculates frequencies for the list
     */
    public void Recalculate(int currentLevel) {
        //Clear old list
        currentList.Clear();
        totalFrequency = 0;

        //Sequentially add objects to the current list
        for (int index = 0; index < originalList.Count; index++) {
            if (originalList[index].LevelAppearance() <= currentLevel) {
                currentList.Add(originalList[index]);
                totalFrequency += originalList[index].Frequency();
            }
        }
    }

    /**
     * Returns the next wait time
     */
    public float NextWaitTime() {
        return Random.Range(waitTime.x, waitTime.y);
    }

}