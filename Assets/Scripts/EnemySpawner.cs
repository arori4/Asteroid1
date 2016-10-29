using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

    public GameObject[] levelEnemies;
    public int[] frequencies;

    public float topSpawnLimit;
    public float bottomSpawnLimit;

    public Vector2 spawnConstraints;

    public int beginningWait;
    public int spawnWait;

    bool activated;

    // Use this for initialization
    void Start() {
        if (levelEnemies.Length != frequencies.Length) {
            activated = false;
            print("EnemySpawner: frequencies and elevel enemies not of same length");
        }
        else {
            activated = true;
        }

        StartCoroutine(SpawnWaves());

    }

    // Update is called once per frame
    void Update() {

    }

    //Coroutine
    IEnumerator SpawnWaves() {

        yield return new WaitForSeconds(beginningWait); //pause

        while (true) {
            //Choose a random hazard
            GameObject hazard = levelEnemies[Random.Range(0, levelEnemies.Length)];

            Vector3 spawnPosition = new Vector3(
                spawnConstraints.x, 
                spawnConstraints.y, 
                Random.Range(topSpawnLimit, bottomSpawnLimit));
            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(hazard, spawnPosition, spawnRotation);

            yield return new WaitForSeconds(spawnWait); //pause

            /*
            if (mGameOver) {
                mRestartText.text = "Press 'R' for Restart";
                mRestart = true;
                break;
            }
            */
        }
    }
    
}
