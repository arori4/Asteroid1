using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Describes the movement of a friend ship.
 * Requires an ObjectCollisionHandler in the parent
 */
public class FriendMover : MonoBehaviour {

    public Vector2 dodgeTime;
    public Vector2 switchDirectionsTime;
    public float speed;

    public Vector2 moveToMiddleDelay;
    public Boundary boundary;
    public float tiltSide;
    public float tiltFront;

    Vector3 newVelocity;
    Vector3 resultantVelocity;

    float turningTime;
    bool isDodgingTargets;
    bool isMovingToMiddle;
    CanCollideWith dodgeDefinitions;
    Rigidbody rb;
    List<GameObject> dodgeList = new List<GameObject>();

	void Start () {
        dodgeDefinitions = GetComponentInParent<ObjectCollisionHandler>().collideDefinition;
        rb = GetComponentInParent<Rigidbody>();

        StartCoroutine(RandomDirection());
        StartCoroutine(ChooseDodgeTarget());
        StartCoroutine(MoveToMiddle());
    }

    void FixedUpdate() {

        //clamp velocity
        turningTime += Time.deltaTime * speed * 0.05f;
        resultantVelocity = Vector3.Lerp(resultantVelocity, newVelocity, turningTime);

        rb.velocity = resultantVelocity;

        //Clamp position
        rb.position = new Vector3(
            Mathf.Clamp(rb.position.x, boundary.left, boundary.right),
            0,
            Mathf.Clamp(rb.position.z, boundary.bottom, boundary.top));

        //Intentionally siwtch z and x
        rb.rotation = Quaternion.Euler(rb.velocity.z * tiltSide, 0.0f, rb.velocity.x * -tiltFront);
    }

    void OnTriggerEnter(Collider other) {
        //only evaluate for the detection collider
        if (dodgeDefinitions.collidesWith(other)) {
            dodgeList.Add(other.transform.root.gameObject);
        }
    }

    IEnumerator RandomDirection() {

        while (true) {
            yield return new WaitForSeconds(Random.Range(switchDirectionsTime.x, switchDirectionsTime.y));

            if (isDodgingTargets) {
                continue;
            }
            if (isMovingToMiddle) {
                continue;
            }

            newVelocity = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            newVelocity = Vector3.Normalize(newVelocity) * speed;
            turningTime = 0f;
            yield return null;
        }
    }

    IEnumerator ChooseDodgeTarget() {
        
        while (true) {
            yield return new WaitForSeconds(Random.Range(dodgeTime.x, dodgeTime.y));

            //do not doge anything if the list is 0
            if (dodgeList.Count <= 0) {
                isDodgingTargets = false;
                continue;
            }
            if (isMovingToMiddle) {
                isDodgingTargets = false;
                continue;
            }

            isDodgingTargets = true;
            GameObject chosenTarget = null;
            do {
                chosenTarget = dodgeList[Random.Range(0, dodgeList.Count)];
                if (chosenTarget == null) {
                    dodgeList.Remove(chosenTarget);
                }
                if (dodgeList.Count == 0) {
                    break;
                }
                yield return null;
            } while (chosenTarget == null);

            //find vector towards object and go away from it
            if (chosenTarget != null) {
                newVelocity = (chosenTarget.transform.position - transform.position) * -1;
                newVelocity = Vector3.Normalize(newVelocity) * speed;
                turningTime = 0f;
            }
            yield return null;
        }

    }

    IEnumerator MoveToMiddle() {
        while (true) {

            yield return new WaitForSeconds(Random.Range(moveToMiddleDelay.x, moveToMiddleDelay.y));
            isMovingToMiddle = true;

            print(isMovingToMiddle);

            newVelocity = -transform.position;
            newVelocity = Vector3.Normalize(newVelocity) * speed;
            turningTime = 0f;
            yield return null;

            yield return new WaitForSeconds(Random.Range(1f, 2.5f));
            isMovingToMiddle = false;
            print(isMovingToMiddle);
        }
    }



    IEnumerator TargetEnemy() {
        yield return null;

    }

}
