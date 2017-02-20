using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/**
 * Moves an object straight.
 * Has extra behavior on drop.
 * Movement is only for 'dumb' objects.
 * 
 * Only use on server object
 */
public class ObjectStraightMover : NetworkBehaviour{
    
    public Vector3 finalDirection;
    public float finalAngle;
    public float speed;

    [SyncVar]
    public bool dropMoveAway;
    [SyncVar]
    public float dropSpeed;
    public float dropTime = 1;
    public bool wasDropped = false;

    [SyncVar]
    Vector3 currentVelocity;
    [SyncVar]
    Vector3 finalVelocity;
    [SyncVar]
    float velocityLerpCounter;

    void OnEnable () {
        //Server computes these values
        if (!isServer) { return; }

        //Reset variables
        currentVelocity = Vector3.zero;
        finalVelocity = Vector3.zero;
        velocityLerpCounter = 0;

        //Create final direction
        Vector3 resultantDirection = Vector3.zero;
        SetFinalDirection(finalDirection);

        if (dropMoveAway && wasDropped) {
            //choose a random direction to go
            Vector3 newDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            resultantDirection = Vector3.Normalize(newDirection) * Random.Range(0f, dropSpeed);
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

    public void SetFinalDirection(Vector3 newFinalDirection) {
        if (!isServer) { return; }
        finalDirection = newFinalDirection;
        Quaternion finalRotation = Quaternion.Euler(0, Random.Range(-finalAngle, finalAngle), 0);
        finalVelocity = finalRotation * Vector3.Normalize(finalDirection) * speed;
    }
    
}
