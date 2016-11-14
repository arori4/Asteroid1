using UnityEngine;
using System.Collections;

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

	void Start () {
        detector = GetComponent<SphereCollider>();
        detector.radius = 0;

        rootTransform = transform.root;
	}
	
    void Update() {
        detector.radius += Time.deltaTime * jumpSpeed;


        //Destroy if the collider reaches max radius and hasn't found an object to jump to
        if (hitsPerformed > numHitTotal || detector.radius > maxRadius) {
            Destroy(gameObject);
        }

        //print(rootTransform.position + " " + detector.radius);
    }

    void OnTriggerEnter(Collider other) {
        
        //On collision, damage the next object and continue from there
        if (collideDefinitions.collidesWith(other)) {

            //damage is handled by the collision handler

            //find information between the two locations
            Vector3 dir = rootTransform.position - other.transform.position;
            Vector3 mid = (rootTransform.position + other.transform.position) / 2.0f;
            float factor = 1.5f; //change if not using a quad

            Transform rodTransform = rod.GetComponent<Transform>();

            rodTransform.position = mid;
            rodTransform.rotation = Quaternion.FromToRotation(Vector3.right, dir);
            rodTransform.Rotate(new Vector3(90, 0, 0));

            //Create the rod
            GameObject newRod = Instantiate(rod, rodTransform.position, rodTransform.rotation) as GameObject;
            newRod.transform.localScale = new Vector3(dir.magnitude * factor, 0.5f, 0.5f);

            //move this handler onto the other's location and reset it
            rootTransform.position = other.transform.position;
            detector.radius = 0;

            //damage the other elemtn
            ObjectCollisionHandler handler = other.transform.root.gameObject.GetComponent<ObjectCollisionHandler>();
            handler.damage(damageAmount, tag);
            Instantiate(explosion, rootTransform.position, rootTransform.rotation);

            //increase hits performed
            hitsPerformed++;
        }

    }

}


