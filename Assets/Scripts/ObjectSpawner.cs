using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour {

    //game mode
    const int LEVEL_MODE = 0;
    const int SURVIVAL_MODE = 1;
    public int mode = 0;
    public int level = 1;
    public float survivalLevelTime = 15f;

    //Spawn Limits
    public Vector2 spawnVerticalLimits = new Vector2(-4.35f, 4.35f);
    public Vector2 spawnOtherLimits;
    public float beginningWait;

    //Enemies
    public List<SpawnObjDef> enemies;
    List<SpawnObjDef> currentLevelEnemies = new List<SpawnObjDef>();
    int totalEnemyFrequencies;
    //Enemy Weapons
    public List<SpawnObjDef> enemyWeapons;
    List<SpawnObjDef> currentLevelWeapons = new List<SpawnObjDef>();
    int totalWeaponFrequencies;

    int numEnemiesLeft;
    int maxEnemiesSpawnAtTime;
    Vector2 enemyWaitTime;

    //Asteroids
    public GameObject[] largeAsteroids;
    public GameObject[] smallAsteroids;
    public Material[] asteroidMaterials;
    bool continueSpawningAsteroids;
    int maxAsteroidsSpawnAtTime;
    float asteroidSpeed;
    Vector2 asteroidSizeRatio;
    Vector2 asteroidWaitTime;

    //UI Elements
    public CanvasGroup largeTextCanvas;
    public Text largeText;

    //Locks
    bool recalculationFinished;

    void Start() {

        //Initialize variables
        enemyWaitTime = new Vector2(1, 2);
        asteroidWaitTime = new Vector2(1, 2);
        asteroidSizeRatio = new Vector2(7, 4);

        //initiate game mode and level
        if (PlayerPrefs.GetInt("Mode") == LEVEL_MODE) {
            mode = LEVEL_MODE;
            SetLevel(PlayerPrefs.GetInt("Level"));
            largeText.text = "Level " + level;
        }
        else if (PlayerPrefs.GetInt("Mode") == SURVIVAL_MODE) {
            mode = SURVIVAL_MODE;
            SetLevel(1);
            largeText.text = "Survive";
            StartCoroutine(IncreaseLevels());
        }
        else {
            print("Mode is invalid, game will be set to survival mode by default. " + mode);
            mode = SURVIVAL_MODE;
            SetLevel(1);
            largeText.text = "Survive";
            StartCoroutine(IncreaseLevels());
        }

        //Show text and fade it
        largeTextCanvas.alpha = 1;
        StartCoroutine(FadeOutText(largeTextCanvas, 0.4f));

        //Start level
        recalculationFinished = false;
        continueSpawningAsteroids = true;
        StartCoroutine(RecalculateFrequencies());
        StartCoroutine(SpawnEnemiesCoroutine());
        StartCoroutine(SpawnAsteroidsCoroutine());
    }

    private void SetLevel(int newLevel) {
        //set level
        level = newLevel;

        //check level
        if (level <= 0) {
            print("level is less than 0, by default will set back to 1");
            level = 1;
        }

        //Set base numenemies left
        if (mode == LEVEL_MODE) {
            numEnemiesLeft = 5 * level;
        }
        else if (mode == SURVIVAL_MODE) {
            numEnemiesLeft = 100000000;
        }

        //level dependent attributes
        asteroidSpeed = 1 + level * 0.2f;

        //attributes for each level
        if (level >= 1) {
            enemyWaitTime = new Vector2(2.5f, 4f);
            asteroidWaitTime = new Vector2(1f, 1.5f);
            maxAsteroidsSpawnAtTime = 1;
            maxEnemiesSpawnAtTime = 1;
        }
        if (level >= 5) {
            enemyWaitTime = new Vector2(2f, 3f);
            asteroidWaitTime = new Vector2(0.75f, 1.25f);
            maxAsteroidsSpawnAtTime = 2;
        }
        if (level >= 7) {
            enemyWaitTime = new Vector2(1.5f, 2.5f);
            asteroidWaitTime = new Vector2(0.5f, 1.25f);
        }
        if (level >= 10) {
            enemyWaitTime = new Vector2(1f, 1.5f);
            asteroidWaitTime = new Vector2(0.45f, 1f);
            maxAsteroidsSpawnAtTime = 3;
            maxEnemiesSpawnAtTime = 2;
        }
        if (level >= 15) {
            enemyWaitTime = new Vector2(0.5f, 1.5f);
            asteroidWaitTime = new Vector2(0.25f, 0.5f);
            maxAsteroidsSpawnAtTime = 4;
            maxEnemiesSpawnAtTime = 4;
        }
        if (level >= 20) {
            enemyWaitTime = new Vector2(0.5f, 1f);
            asteroidWaitTime = new Vector2(0.25f, 0.35f);
            maxAsteroidsSpawnAtTime = 5;
            maxEnemiesSpawnAtTime = 5;
        }

    }

    private IEnumerator SpawnEnemiesCoroutine() {
        yield return new WaitForSeconds(beginningWait);

        while (numEnemiesLeft > 0) {
            if (!recalculationFinished) {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            //Chose an amount of enemies to spawn at a time
            int numEnemiesToSpawn = (int)Random.Range(1, maxEnemiesSpawnAtTime);

            for (int index = 0; index < numEnemiesToSpawn; index++) {

                GameObject enemyToSpawn = ChooseRandomFromFrequency(currentLevelEnemies, totalEnemyFrequencies);
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

                    for (int gunIndex = 0; gunIndex < guns.Length; gunIndex++) {
                        //Choose random gun
                        GameObject gunToSet = ChooseRandomFromFrequency(currentLevelWeapons, totalWeaponFrequencies);
                        yield return null;

                        guns[gunIndex].ChangeWeapon(gunToSet);
                        yield return null;
                    }
                }

                yield return new WaitForSeconds(Random.Range(enemyWaitTime.x, enemyWaitTime.y)); //pause
            }

        }
        //advance when no more enemies exist
        continueSpawningAsteroids = false;
        yield return new WaitForSeconds(5);

        largeTextCanvas.alpha = 1;
        largeText.text = "Clear";
        //TODO: special effect

        StartCoroutine(FadeOutText(largeTextCanvas, 0.3f));

        GetComponent<UIController>().AdvanceLevel(); //level increase is handled here

        yield return new WaitForSeconds(20);
    }


    /**
     * Chooses a random object based on the pair list given.
     */
    private GameObject ChooseRandomFromFrequency(List<SpawnObjDef> pairs, int totalFrequency) {
        int chosenFrequency = Random.Range(0, totalFrequency) + 1;
        int chooseIndex = 0;
        while (chosenFrequency > 0) {
            chosenFrequency -= pairs[chooseIndex++].frequency;
        }
        chooseIndex--; //correction to choose the correct one b/c it adds stuff

        return pairs[chooseIndex].obj;
    }


    /**
     * Coroutine for spawning asteroids.
     */
    private IEnumerator SpawnAsteroidsCoroutine() {
        yield return new WaitForSeconds(beginningWait);

        while (continueSpawningAsteroids) {
            if (!recalculationFinished) { //do not start if there is no array calculated
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            //Chose an amount of asteroids to spawn at a time
            int numAsteroidsToSpawn = Random.Range(1, maxAsteroidsSpawnAtTime);

            for (int index = 0; index < numAsteroidsToSpawn; index++){

                //Choose a random asteroid based on ratios
                GameObject enemyToSpawn = null;
                float randomSizeSToL = Random.Range(0, asteroidSizeRatio.x + asteroidSizeRatio.y);
                if (randomSizeSToL < asteroidSizeRatio.x) { //small asteroid chosen
                    enemyToSpawn = smallAsteroids[Random.Range(0, smallAsteroids.Length)];
                }
                else { //large asteroid chosen
                    enemyToSpawn = largeAsteroids[Random.Range(0, largeAsteroids.Length)];
                }
                yield return null;

                //Spawn asteroid
                GameObject spawnedEnemy = SpawnRngMaterial(enemyToSpawn, asteroidMaterials);
                spawnedEnemy.GetComponent<ObjectStraightMover>().speed = asteroidSpeed;
                yield return null;
            }


            //Wait for new asteroid to create
            yield return new WaitForSeconds(Random.Range(asteroidWaitTime.x, asteroidWaitTime.y));
        }
    }

    /**
     * Increases the level
     * Only for survival mode
     */
    private IEnumerator IncreaseLevels() {
        while (true) {
            yield return new WaitForSeconds(survivalLevelTime);
            SetLevel(level + 1);
            PlayerPrefs.SetInt("Level", level);
            StartCoroutine(RecalculateFrequencies());
        }
    }

    private IEnumerator RecalculateFrequencies() {
        //ENEMIES
        //Initialize needed variables
        List<SpawnObjDef> enemiesThisLevel = new List<SpawnObjDef>();
        int newEnemyFrequency = 0;
        yield return null;

        //initialize enemies for level appropriately
        for (int index = 0; index < enemies.Count; index++) {
            if (enemies[index].levelAppearance <= level) {
                enemiesThisLevel.Add(enemies[index]);
                newEnemyFrequency += enemies[index].frequency;
            }
            yield return null;
        }

        //Send variables back
        totalEnemyFrequencies = newEnemyFrequency;
        currentLevelEnemies = enemiesThisLevel;

        //ENEMY WEAPONS
        //Initialize needed variables
        List<SpawnObjDef> weaponsThisLevel = new List<SpawnObjDef>();
        int newWeaponFrequency = 0;
        yield return null;

        //initialize enemies for level appropriately
        for (int index = 0; index < enemyWeapons.Count; index++) {
            if (enemyWeapons[index].levelAppearance <= level) {
                weaponsThisLevel.Add(enemyWeapons[index]);
                newWeaponFrequency += enemyWeapons[index].frequency;
            }
            yield return null;
        }

        //Send variables back
        totalWeaponFrequencies = newWeaponFrequency;
        currentLevelWeapons = weaponsThisLevel;

        //finally, let recalculation be finished
        recalculationFinished = true;
    }

    /**
     * Fades the canvas out to an alpha of 0.
     * Canvas will be transparent
     */
    private IEnumerator FadeOutText(CanvasGroup canvas, float smoothing) {
        yield return new WaitForSeconds(0.8f);

        while (canvas.alpha > 0) {
            canvas.alpha -= Time.deltaTime * smoothing;
            yield return null;
        }
    }

    /**
     * Spawns an object with a random material
     */
    private GameObject SpawnRngMaterial(GameObject obj, Material[] materials) {
        GameObject spawnedObj = SpawnInWave(obj);
        spawnedObj.GetComponentInChildren<Renderer>().material =
            materials[Random.Range(0, materials.Length)];

        return spawnedObj;
    }

    private GameObject SpawnInWave(GameObject obj) {

        Vector3 spawnPosition = new Vector3(spawnOtherLimits.x, spawnOtherLimits.y,
            Random.Range(spawnVerticalLimits.x, spawnVerticalLimits.y));
        GameObject spawnedObj = Pools.Initialize(obj, spawnPosition, Quaternion.identity);

        return spawnedObj;
    }
}

[System.Serializable]
public class SpawnObjDef {

    public GameObject obj;
    public int frequency;
    public int levelAppearance;
}
