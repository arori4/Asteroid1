using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Describes the movement of a friend ship.
 * Requires an ObjectCollisionHandler in the parent
 */
public class FriendMover : MonoBehaviour {

    public Boundary boundary;
    public float speed;
    public float tiltSide;
    public float tiltFront;

    public Vector2 dodgeTime;
    public Vector2 switchDirectionsTime;
    public Vector2 moveToMiddleTime;

    Transform rootTransform;
    Vector3 newVelocity;
    Vector3 currentVelocity;
    float turningTime;
    bool isDodgingTargets;
    bool isMovingToMiddle;

    //Dodging
    CanCollideWith dodgeDefinitions;
    List<GameObject> dodgeList = new List<GameObject>();

    void Start() {
        dodgeDefinitions = GetComponentInParent<ObjectCollisionHandler>().collideDefinition;
    }

	void OnEnable () {
        rootTransform = transform.root;

        StartCoroutine(RandomDirection());
        StartCoroutine(ChooseDodgeTarget());
        StartCoroutine(MoveToMiddle());
    }

    void Update() {
        //Calculate Velocity
        turningTime += Time.deltaTime * speed * 0.05f;
        currentVelocity = Vector3.Lerp(currentVelocity, newVelocity, turningTime);

        //Set position
        rootTransform.position += currentVelocity * Time.deltaTime;

        //Clamp position
        rootTransform.position = new Vector3(
            Mathf.Clamp(rootTransform.position.x, boundary.left, boundary.right),
            0,
            Mathf.Clamp(rootTransform.position.z, boundary.bottom, boundary.top));

        //Set pitch and yaw
        rootTransform.rotation = Quaternion.Euler(currentVelocity.z * tiltSide, 0.0f, currentVelocity.x * -tiltFront);
    }

    void OnTriggerEnter(Collider other) {
        //only evaluates for the detection collider
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

            yield return new WaitForSeconds(Random.Range(moveToMiddleTime.x, moveToMiddleTime.y));
            isMovingToMiddle = true;

            newVelocity = -transform.position;
            newVelocity = Vector3.Normalize(newVelocity) * speed;
            turningTime = 0f;
            yield return null;

            yield return new WaitForSeconds(Random.Range(1f, 2.5f));
            isMovingToMiddle = false;
        }
    }



    IEnumerator TargetEnemy() {
        yield return null;

    }

}
