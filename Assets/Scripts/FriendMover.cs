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

    public Vector2 switchTurningMethod;
    public Boundary boundary;
    public float tiltSide;
    public float tiltFront;

    Vector3 newVelocity;
    Vector3 scratch;

    bool isDodgingTargets;
    bool turningMethod;
    CanCollideWith dodgeDefinitions;
    Rigidbody rb;
    List<GameObject> dodgeList = new List<GameObject>();

	void Start () {
        dodgeDefinitions = GetComponentInParent<ObjectCollisionHandler>().collideDefinition;
        rb = GetComponentInParent<Rigidbody>();

        StartCoroutine(SwitchDirections());
	}
	
	void Update () {
	
	}

    void FixedUpdate() {
        //Turn ship
        if (turningMethod) {
            rb.velocity = Vector3.Lerp(rb.velocity, newVelocity, Time.deltaTime * speed / 5);
        }
        else {
            rb.velocity = Vector3.SmoothDamp(rb.velocity, newVelocity, ref scratch, Time.deltaTime * speed / 5);
        }

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
        if (other.gameObject.CompareTag("Friend Detector") && dodgeDefinitions.collidesWith(other)) {
            dodgeList.Add(other.transform.root.gameObject);
        }
    }

    IEnumerator SwitchDirections() {

        while (true) {
            yield return new WaitForSeconds(Random.Range(switchDirectionsTime.x, switchDirectionsTime.y));

            if (isDodgingTargets) {
                continue;
            }

            Vector3 newDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            yield return null;
            newVelocity = Vector3.Normalize(newDirection) * speed;
            yield return null;
        }
    }

    IEnumerator SwitchTurningMethod() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(switchTurningMethod.x, switchTurningMethod.y));
            turningMethod = !turningMethod;

            //Reset scratch vector for smoothDamp
            scratch.x = 0;
            scratch.y = 0;
            scratch.z = 0;
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
        }

    }

    IEnumerator TargetEnemy() {
        yield return null;

    }

}
