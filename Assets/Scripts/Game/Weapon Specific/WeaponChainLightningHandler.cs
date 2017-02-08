using UnityEngine;
using System.Collections;

/**
 * When the chain lightning hits, it will chain hits to other nearby objects
 */
public class WeaponChainLightningHandler : MonoBehaviour {

    public int numHitTotal = 3;
    public float maxRadius = 3;
    public float jumpSpeed = 5f;
    public float damageAmount;

    public GameObject rod;
    public CanCollideWith collideDefinitions;
    public GameObject explosion;

    int hitsPerformed = 1;

    SphereCollider detector;
    Transform rootTransform;
    
    void OnEnable() {
        detector = GetComponent<SphereCollider>();
        detector.radius = 0;
        hitsPerformed = 1;

        rootTransform = transform.root;
	}
	
    void Update() {
        detector.radius += Time.deltaTime * jumpSpeed;

        //Destroy if the collider reaches max radius and hasn't found an object to jump to
        if (hitsPerformed > numHitTotal || detector.radius > maxRadius) {
            Pools.Terminate(gameObject);
        }
    }

    void OnTriggerEnter(Collider other) {
        
        //On collision, damage the next object and continue from there
        if (collideDefinitions.collidesWith(other)) {

            //initial damage is handled by the collision handler

            //find information between the two locations
            Vector3 direction = rootTransform.position - other.transform.position;
            Vector3 middle = (rootTransform.position + other.transform.position) / 2.0f;
            float factor = 1.5f;

            //Create the rod
            Transform rodTransform = rod.GetComponent<Transform>();
            rodTransform.position = middle;
            rodTransform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
            rodTransform.Rotate(new Vector3(90, 0, 0));
            GameObject newRod = Pools.Initialize(rod, rodTransform.position, rodTransform.rotation);
            newRod.transform.localScale = new Vector3(direction.magnitude * factor, 0.5f, 0.5f);

            //move this handler onto the other's location and reset it
            rootTransform.position = other.transform.position;
            detector.radius = 0;

            //damage the other element
            ObjectCollisionHandler handler = other.GetComponent<ObjectCollisionHandler>();
            //if no collider exists, then attempt to take from parent
            GameObject otherObject = other.gameObject;
            while (handler == null) {
                otherObject = otherObject.transform.parent.gameObject;
                handler = otherObject.GetComponentInParent<ObjectCollisionHandler>();
            }
            handler.Damage(damageAmount, tag);
            Pools.Initialize(explosion, rootTransform.position, rootTransform.rotation);

            //increase hits performed
            hitsPerformed++;
        }

    }

}


