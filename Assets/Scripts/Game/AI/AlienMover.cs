using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/**
 * Defines AI for alien movement
 */
public class AlienMover : NetworkBehaviour {

    public Vector3 initialDirection = Vector3.left;
    public float speed;
    public float tiltForward;
    public float tiltSide;
    public Vector2 smoothingBounds;

    public MoveBackwards moveBackwards;
    public MoveSideways moveSideways;
    public MoveToCenter moveToCenter;
    public TargetPlayer targetPlayer;

    Vector3 currentVelocity; //calculated on client

    [SyncVar]
    float targetX;
    [SyncVar]
    float targetZ;
    [SyncVar]
    float smoothing;
    [SyncVar]
    Transform player;

    void OnEnable () {
        Vector3 normalizedVelocity = Vector3.Normalize(initialDirection);
        currentVelocity = normalizedVelocity * speed;

        targetX = -speed;
        targetZ = 0;
        smoothing = Random.Range(smoothingBounds.x, smoothingBounds.y);

        //Run coroutines only on server, and sync target to player
        if (!isServer) { return; }

        if (moveBackwards.activated) {
            StartCoroutine(Backwards());
        }
        if (moveSideways.activated) {
            StartCoroutine(Sideways());
        }
        if (moveToCenter.activated) {
            StartCoroutine(Center());
        }
        if (targetPlayer.activated) {
            StartCoroutine(TargetPlayer());
        }
    }

    void OnDisable() {
        StopAllCoroutines();
    }

    void Update() {
        //Set movement
        currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, targetX, Time.deltaTime * smoothing);
        currentVelocity.z = Mathf.MoveTowards(currentVelocity.z, targetZ, Time.deltaTime * smoothing);

        //move the alien
        transform.position += currentVelocity * Time.deltaTime;

        //Set pitch and yaw
        transform.rotation = Quaternion.Euler(currentVelocity.z * tiltSide, currentVelocity.x * -tiltForward, 0.0f);
    }


    private IEnumerator Sideways() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(
                moveSideways.frequencyConstraints.x,
                moveSideways.frequencyConstraints.y));

            //Move the target to a random place sideways
            targetZ = Random.Range(-moveSideways.movementConstraints, moveSideways.movementConstraints);

            if (moveSideways.straightEveryOther) {
                yield return new WaitForSeconds(Random.Range(
                    moveSideways.frequencyConstraints.x,
                    moveSideways.frequencyConstraints.y));
                targetZ = 0; //move straight
            }
        }
    }

    private IEnumerator Center() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(
                moveToCenter.frequencyConstraints.x,
                moveToCenter.frequencyConstraints.y));

            //Move the target to the center
            targetZ = 0;
                
        }
    }

    private IEnumerator Backwards() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(
                moveBackwards.frequencyConstraints.x,
                moveBackwards.frequencyConstraints.y));

            //Move the target to a random place backwards OR forwards
            targetX = Random.Range(-moveBackwards.movementConstraints, moveBackwards.movementConstraints);

            yield return new WaitForSeconds(Random.Range(
                moveBackwards.frequencyConstraints.x,
                moveBackwards.frequencyConstraints.y));
            targetX = -speed; //move straight forward
        }
    }

    private IEnumerator TargetPlayer() {
        //Get the player
        yield return null;
        player = GameObject.FindWithTag("Player").transform;
        Vector3 playerPosition = player.position;

        yield return new WaitForSeconds(Random.Range(
            targetPlayer.startTimeRange.x,
            targetPlayer.startTimeRange.y));

        while (true) {
            //update position
            playerPosition = player.position;
            
            //track the player
            targetZ = playerPosition.z;
            yield return null;
        }
    }

}


[System.Serializable]
public class Setting {
    public bool activated;
    public Vector2 startTimeRange;
}


[System.Serializable]
public class MoveBackwards : Setting {
    public Vector2 frequencyConstraints;
    public float movementConstraints;
}

[System.Serializable]
public class MoveSideways : Setting {
    public Vector2 frequencyConstraints;
    public float movementConstraints;
    public bool straightEveryOther;
}

[System.Serializable]
public class TargetPlayer : Setting {
    public float speed;
}

[System.Serializable]
public class MoveToCenter : Setting {
    public Vector2 frequencyConstraints;
    public float movementConstraints;
}