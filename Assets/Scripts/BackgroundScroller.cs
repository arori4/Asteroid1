using UnityEngine;
using System.Collections;

public class BackgroundScroller : MonoBehaviour {

    public GameObject tile1;
    public Vector3 tile1Start;
    public GameObject tile2;
    public Vector3 tile2Start;

    public float tileSpeed = 0.3f;
    public float tileLength = 25;
    public BackgroundDef[] backgrounds;

    public Lights lights;

    public Stars stars;

    
    void Start() {
        //Chooose number
        int chosenIndex = Random.Range(0, backgrounds.Length);

        //choose a random background
        Material chosenBackground = backgrounds[chosenIndex].material;

        //set for each tile
        tile1.GetComponent<Renderer>().material = chosenBackground;
        tile2.GetComponent<Renderer>().material = chosenBackground;

        //set lighting colors
        lights.mainLight.color = backgrounds[chosenIndex].mainLightColor;
        lights.light1.color = backgrounds[chosenIndex].light1Color;
        lights.light2.color = backgrounds[chosenIndex].light2Color;
        lights.light3.color = backgrounds[chosenIndex].light3Color;

        //set star colors
        stars.mainStars.startColor = backgrounds[chosenIndex].mainLightColor;
        stars.stars1.startColor = backgrounds[chosenIndex].light1Color;
        stars.stars2.startColor = backgrounds[chosenIndex].light2Color;
        stars.stars3.startColor = backgrounds[chosenIndex].light3Color;
    }
    
    void Update() {
        float newPosition = Mathf.Repeat(Time.time * tileSpeed, tileLength);
        tile1.transform.position = tile1Start + Vector3.left * newPosition;
        tile2.transform.position = tile2Start + Vector3.left * newPosition;
    }
}

[System.Serializable]
public struct BackgroundDef {

    public Material material;
    public Color mainLightColor;
    public Color light1Color;
    public Color light2Color;
    public Color light3Color;

}

[System.Serializable]
public struct Lights {

    public Light mainLight;
    public Light light1;
    public Light light2;
    public Light light3;
    
}

[System.Serializable]
public struct Stars {

    public ParticleSystem mainStars;
    public ParticleSystem stars1;
    public ParticleSystem stars2;
    public ParticleSystem stars3;

}