using UnityEngine;
using System.Collections;

/**
 * Handles stats
 * Currently only handles score
 */
public class GameSave : MonoBehaviour {
    
    public int[] highScores;
    public int[] levels;

	void Start () {

        highScores = new int[10];
        levels = new int[10];

        //load all of the player high scores
        for (int index = 0; index < 10; index++) {
            string highScoreKey = "HighScore" + index;
            if (PlayerPrefs.HasKey(highScoreKey)) {
                highScores[index] = PlayerPrefs.GetInt(highScoreKey);
            }
            else {
                PlayerPrefs.SetInt(highScoreKey, 0);
            }

            string levelKey = "Level" + index;
            if (PlayerPrefs.HasKey(levelKey)) {
                levels[index] = PlayerPrefs.GetInt(levelKey);
            }
            else {
                PlayerPrefs.SetInt(levelKey, 0);
            }
        }

	}

    public void SaveHighScore(int score, int level) {

        int index = 9;
        while (index >= 0 && score > highScores[index]) {
            index--;
        }
        index++;

        //record high score if index is less than 10
        if (index < 10) {
            for (int scoreLowered = 9; scoreLowered > index; scoreLowered--) {
                highScores[scoreLowered] = highScores[scoreLowered - 1];
                levels[scoreLowered] = levels[scoreLowered - 1];
            }
            highScores[index] = score;
            levels[index] = level;
        }

        //write all the scores
        for (int index2 = 0; index2 < 10; index2++) {
            string highScoreKey = "HighScore" + index2;
            PlayerPrefs.SetInt(highScoreKey, highScores[index2]);

            string levelKey = "Level" + index2;
            PlayerPrefs.SetInt(levelKey, levels[index2]);
        }

    }

    public void recordAdditionInt(string name, int amount) {
        if (!PlayerPrefs.HasKey(name)) {
            PlayerPrefs.SetInt(name, 0);
        }

        PlayerPrefs.SetInt(name, PlayerPrefs.GetInt(name) + amount);
    }
}
