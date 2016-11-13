using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectSpawner : MonoBehaviour {

    //game mode
    const int LEVEL_MODE = 0;
    const int SURVIVAL_MODE = 1;
    public int mode = 0;

    //level
    public int level = 1;

    //Enemies
    public Pair[] enemies;
    Pair[] currentLevelEnemies;
    public Material[] asteroidMaterials;

    //Spawn Limits
    public float topSpawnLimit;
    public float bottomSpawnLimit;
    public Vector2 spawnConstraints;
    public int beginningWait;
    public Vector2 waitBetweenEnemies;

    //Large Text
    public CanvasGroup largeTextCanvas;
    public Text largeText;

    //Cache
    int totalFrequencies;
    int numEnemies = 10;
    bool initialRecalculationFinished;

    void Start() {
        //initiate game mode and level
        if (PlayerPrefs.GetInt("Mode") == LEVEL_MODE) {
            mode = LEVEL_MODE;
            level = PlayerPrefs.GetInt("Level");
            numEnemies = 20 + 5 * level;
            largeText.text = "Level " + level;
        }
        else if (PlayerPrefs.GetInt("Mode") == SURVIVAL_MODE) {
            mode = SURVIVAL_MODE;
            level = 1;
            numEnemies = 100000000;
            largeText.text = "Survive";
            StartCoroutine(IncreaseLevels());
        }
        else {
            print("Mode not set, game will be set to survival mode by default.");

            mode = SURVIVAL_MODE;
            level = 1;
            numEnemies = 100000000;
            largeText.text = "Survive";
            StartCoroutine(IncreaseLevels());
        }

        currentLevelEnemies = new Pair[0];

        //Show text and fade it
        largeTextCanvas.alpha = 1;
        StartCoroutine(FadeOutText(largeTextCanvas, 0.4f));

        //Start level
        initialRecalculationFinished = false;
        StartCoroutine(RecalculateFrequencies());
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves() {
        yield return new WaitForSeconds(beginningWait); //pause

        while (true) {
            if (!initialRecalculationFinished) {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            //Choose a random enemy
            int chosenFrequency = Random.Range(0, totalFrequencies) + 1;
            int chooseIndex = 0;
            while (chosenFrequency > 0) {
                chosenFrequency -= currentLevelEnemies[chooseIndex++].frequency;
                yield return null;
            }
            chooseIndex--; //correction to choose the correct one b/c it adds stuff
            GameObject enemyToSpawn = currentLevelEnemies[chooseIndex].obj;

            if (enemyToSpawn.CompareTag("Large Asteroid") ||
                enemyToSpawn.CompareTag("Small Asteroid")) {
                SpawnRngMaterial(enemyToSpawn, asteroidMaterials);
            }
            else {
                SpawnInWave(enemyToSpawn);
            }
            numEnemies--;

            yield return new WaitForSeconds(Random.Range(waitBetweenEnemies.x, waitBetweenEnemies.y)); //pause


            //advance when no more enemies exist
            if (numEnemies <= 0) {
                yield return new WaitForSeconds(5);

                largeTextCanvas.alpha = 1;
                largeText.text = "Clear";
                //TODO: special effect

                StartCoroutine(FadeOutText(largeTextCanvas, 0.3f));

                GetComponent<UIController>().AdvanceLevel();

                yield return new WaitForSeconds(20);
            }

        }
    }

    /**
     * Increases the level
     * Only for survival mode
     */
    IEnumerator IncreaseLevels() {
        while (true) {
            yield return new WaitForSeconds(20);
            level++;
            PlayerPrefs.SetInt("Level", level);
            StartCoroutine(RecalculateFrequencies());
        }
    }

    IEnumerator RecalculateFrequencies() {
        //initialize enemies for level appropriately
        int numEnemyTypes = 0;
        for (int index = 0; index < enemies.Length; index++) {
            if (enemies[index].levelAppearance <= level) {
                numEnemyTypes++;
            }
            yield return null;
        }
        Pair[] newLevels = new Pair[numEnemyTypes];
        int addIndex = 0;
        for (int index = 0; index < enemies.Length; index++) {
            if (enemies[index].levelAppearance <= level) {
                newLevels[addIndex++] = enemies[index];
            }
            yield return null;
        }

        //initialize frequency generator
        int newTotalFrequency = 0;
        for (int index = 0; index < newLevels.Length; index++) {
            newTotalFrequency += newLevels[index].frequency;
            yield return null;
        }

        //Send variables back
        initialRecalculationFinished = true;
        totalFrequencies = newTotalFrequency;
        currentLevelEnemies = newLevels;
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
    private void SpawnRngMaterial(GameObject obj, Material[] materials) {
        GameObject spawnedObj = SpawnInWave(obj);
        spawnedObj.GetComponentInChildren<Renderer>().material =
            materials[Random.Range(0, materials.Length)];
    }

    private GameObject SpawnInWave(GameObject obj) {

        Vector3 spawnPosition = new Vector3(
            spawnConstraints.x,
            spawnConstraints.y,
            Random.Range(topSpawnLimit, bottomSpawnLimit));

        Quaternion spawnRotation = Quaternion.identity;

        GameObject spawnedObj = Instantiate(obj, spawnPosition, spawnRotation) as GameObject;

        return spawnedObj;
    }


}

[System.Serializable]
public struct Pair {

    public GameObject obj;
    public int frequency;
    public int levelAppearance;

}