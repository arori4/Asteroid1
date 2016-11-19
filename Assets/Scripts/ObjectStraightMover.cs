﻿using UnityEngine;
using System.Collections;

/**
 * Moves an object straight.
 * Has extra behavior on drop.
 * Movement is only for 'dumb' objects.
 */
public class ObjectStraightMover : MonoBehaviour {

    public Vector3 finalDirection;
    public float finalAngle;
    public float speed;

    public bool dropMoveAway;
    public float dropSpeed;
    public float dropTime = 1;
    public bool wasDropped = false;

    Vector3 currentVelocity;
    Vector3 finalVelocity;
    float velocityLerpCounter = 0;

    void OnEnable () {
        //Create final direction
        Vector3 resultantDirection = new Vector3(0, 0, 0);
        Quaternion finalRotation = Quaternion.Euler(0, Random.Range(-finalAngle, finalAngle), 0);
        finalVelocity = finalRotation * Vector3.Normalize(finalDirection) * speed;

        if (dropMoveAway && wasDropped) {
            //choose a random direction to go
            Vector3 newDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            resultantDirection = Vector3.Normalize(newDirection) * dropSpeed;
        }
        else {
            resultantDirection = Vector3.Normalize(finalDirection);
        }

        currentVelocity = resultantDirection * speed;
	}
	
    void Update() {
        currentVelocity = Vector3.Lerp(currentVelocity, finalVelocity, velocityLerpCounter);
        velocityLerpCounter += Time.deltaTime / dropTime;
        
        transform.position += currentVelocity * Time.deltaTime;
    }
    
}
