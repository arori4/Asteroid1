using UnityEngine;
using System.Collections;

/**
 * Defines AI for alien movement
 */
public class AlienMover : MonoBehaviour {

    public Vector3 initialDirection = Vector3.left;
    public float speed;
    public float tiltForward;
    public float tiltSide;
    public Vector2 smoothingBounds;

    public MoveBackwards moveBackwards;
    public MoveSideways moveSideways;
    public TargetPlayer targetPlayer;
    
    float targetX;
    float targetZ;
    float smoothing;
    Vector3 currentVelocity;

    void Start () {
        Vector3 normalizedVelocity = Vector3.Normalize(initialDirection);
        currentVelocity = normalizedVelocity * speed;

        targetX = -speed;
        targetZ = 0;
        smoothing = Random.Range(smoothingBounds.x, smoothingBounds.y);

        if (moveBackwards.activated) {
            StartCoroutine(Backwards());
        }
        if (moveSideways.activated) {
            StartCoroutine(Sideways());
        }
        if (targetPlayer.activated) {
            StartCoroutine(TargetPlayer());
        }
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


    IEnumerator Sideways() {
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

    IEnumerator Backwards() {
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

    IEnumerator TargetPlayer() {
        yield return new WaitForSeconds(Random.Range(
            targetPlayer.startTimeRange.x,
            targetPlayer.startTimeRange.y));

        while (true) {
            if (targetPlayer.perfectTrack) {
                targetZ = targetPlayer.player.transform.position.z;
            }
            else {
                targetZ = Mathf.Lerp(targetPlayer.player.transform.position.z, transform.position.z, smoothing);
            }
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
    public GameObject player;
    public float speed;
    public bool perfectTrack;
}