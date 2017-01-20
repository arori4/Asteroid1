using UnityEngine;
using System.Collections;

public class EnemyScoreInfo : MonoBehaviour {

    public int score;

    void Start() {
        if (score == 0) {
            print("GameObject EnemyScoreInfo instantiated with a score of 0.");
        }
    }
}
