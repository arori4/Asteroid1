using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

    public Pair[] levelEnemies;

    public float topSpawnLimit;
    public float bottomSpawnLimit;

    public Vector2 spawnConstraints;
    public int beginningWait;
    public Vector2 waitBetweenEnemies;

    int totalFrequencies;
    
    void Start() {
        //initialize frequency generator
        for (int index = 0; index < levelEnemies.Length; index++) {
            totalFrequencies += levelEnemies[index].frequency;
        }

        if (totalFrequencies == 0) {
            print("Total frequencies is 0");
        }

        StartCoroutine(SpawnWaves());
    }
    
    IEnumerator SpawnWaves() {
        yield return new WaitForSeconds(beginningWait); //pause

        while (true) {
            //Choose a random enemy
            int chosenFrequency = Random.Range(0, totalFrequencies) + 1;
            int chooseIndex = 0;
            while (chosenFrequency > 0) {
                chosenFrequency -= levelEnemies[chooseIndex++].frequency;
                yield return null;
            }
            chooseIndex--; //correction to choose the correct one b/c it adds stuff

            GameObject enemySpawned = levelEnemies[chooseIndex].obj;

            Vector3 spawnPosition = new Vector3(
                spawnConstraints.x, 
                spawnConstraints.y, 
                Random.Range(topSpawnLimit, bottomSpawnLimit));
            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(enemySpawned, spawnPosition, spawnRotation);

            yield return new WaitForSeconds(Random.Range(waitBetweenEnemies.x, waitBetweenEnemies.y)); //pause
            
        }
    }

}

[System.Serializable]
public class Pair {

    public GameObject obj;
    public int frequency;

}