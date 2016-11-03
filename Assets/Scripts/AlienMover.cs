using UnityEngine;
using System.Collections;

public class AlienMover : MonoBehaviour {

    public Vector3 initialDirection = Vector3.left;
    public float speed;
    public float tiltForward;
    public float tiltSide;
    public Boundary boundary;
    public Vector2 smoothingBounds;

    public MoveBackwards moveBackwards;
    public MoveSideways moveSideways;
    public TargetPlayer targetPlayer;

    Rigidbody rb;
    float targetX;
    float targetZ;
    float smoothing;

    void Start () {
        rb = GetComponent<Rigidbody>();
        Vector3 normalizedVelocity = Vector3.Normalize(initialDirection);
        rb.velocity = normalizedVelocity * speed;

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

            print (targetX);

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


    void FixedUpdate() {
        float maneuverX = Mathf.MoveTowards(rb.velocity.x, targetX, Time.deltaTime * smoothing);
        float maneuverZ = Mathf.MoveTowards(rb.velocity.z, targetZ, Time.deltaTime * smoothing);
        rb.velocity = new Vector3(maneuverX, 0, maneuverZ);

        //clamp inside bounds
        rb.position = new Vector3(
            Mathf.Clamp(rb.position.x, boundary.left, boundary.right),
            0,
            Mathf.Clamp(rb.position.z, boundary.bottom, boundary.top)
        );

        rb.rotation = Quaternion.Euler(rb.velocity.z * tiltSide, rb.velocity.x * -tiltForward, 0.0f);
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