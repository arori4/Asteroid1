using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControllerCollisionHandler : MonoBehaviour {

    public GameObject alienExplosion;
    public GameObject[] asteroidLargeExplosion;
    public GameObject[] asteroidSmallExplosion;
    public GameObject playerExplosion;

    public Text scoreText;
    int score;
    
    void Start() {
        score = 0;
    }

    /**
     * Only enumerate with objects that can score
     * TODO: maybe change this so that the handling is done outside
     */
    public void handleScore (GameObject one, GameObject two) {
        if (one.CompareTag("Large Asteroid") || two.CompareTag("Large Asteroid")) {
            score += getScoreFromTwoObjects(one, two);
        }

        if (one.CompareTag("Small Asteroid") || two.CompareTag("Small Asteroid")) {
            score += getScoreFromTwoObjects(one, two);
        }

        if (one.CompareTag("Alien") || two.CompareTag("Alien")) {
            score += getScoreFromTwoObjects(one, two);
        }

        //this sometimes happens
        if (one.CompareTag("Enemy Bolt") || two.CompareTag("Enemy Bolt")) {
            score += 0;
        }

        scoreText.text = "Score: " + score;
    }

    public void handleExplosion(GameObject one, GameObject two, Vector3 location) {

        if (one.CompareTag("Large Asteroid") || two.CompareTag("Large Asteroid")) {
            Instantiate(asteroidLargeExplosion[Random.Range(0, asteroidLargeExplosion.Length)],
                location, Quaternion.identity);
        }

        if (one.CompareTag("Small Asteroid") || two.CompareTag("Small Asteroid")) {
            Instantiate(asteroidSmallExplosion[Random.Range(0, asteroidSmallExplosion.Length)],
                location, Quaternion.identity);
        }


        if (one.CompareTag("Player") || two.CompareTag("Player")) {
            Instantiate(playerExplosion, location, Quaternion.identity);
        }

        if (one.CompareTag("Player Bolt") || two.CompareTag("Player Bolt")) {
        }

        if (one.CompareTag("Enemy Bolt") || two.CompareTag("Enemy Bolt")) {
        }


        if (one.CompareTag("Alien") || two.CompareTag("Alien")) {
            Instantiate(alienExplosion, location, Quaternion.identity);
        }
    }

    private int getScoreFromTwoObjects(GameObject obj1, GameObject obj2) {
        EnemyScoreInfo info = obj1.GetComponent<EnemyScoreInfo>();
        if (info != null) {
            return info.score;
        }
        else {
            return obj2.GetComponent<EnemyScoreInfo>().score;
        }
    }

}
