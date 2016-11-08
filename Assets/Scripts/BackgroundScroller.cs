using UnityEngine;
using System.Collections;

public class BackgroundScroller : MonoBehaviour {

    public GameObject tile1;
    public Vector3 tile1Start;
    public GameObject tile2;
    public Vector3 tile2Start;

    public float tileSpeed = 0.3f;
    public float tileLength = 25;
    public Material[] backgrounds;

    
    void Start() {
        //choose a random background
        Material chosenBackground = backgrounds[Random.Range(0, backgrounds.Length)];

        //set for each tile
        tile1.GetComponent<Renderer>().material = chosenBackground;
        tile2.GetComponent<Renderer>().material = chosenBackground;
    }
    
    void Update() {
        float newPosition = Mathf.Repeat(Time.time * tileSpeed, tileLength);
        tile1.transform.position = tile1Start + Vector3.left * newPosition;
        tile2.transform.position = tile2Start + Vector3.left * newPosition;
    }
}
