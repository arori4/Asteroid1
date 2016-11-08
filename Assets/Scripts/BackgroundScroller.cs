using UnityEngine;
using System.Collections;

public class BackgroundScroller : MonoBehaviour {

    public float speed;
    public float length;
    private Vector3 start;
    
    void Start() {
        start = transform.position;
    }
    
    void Update() {
        float newPosition = Mathf.Repeat(Time.time * speed, length);
        transform.position = start + Vector3.left * newPosition;
    }
}
