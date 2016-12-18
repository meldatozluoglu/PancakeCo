using UnityEngine;
using System.Collections;

public class PancakeMainRigidBodyBehaviourScript : MonoBehaviour {
	PancakeBehaviourScript parentsScript;


// Use this for initialization
	void Start () {
		//setting up the pointer to parent's script
		parentsScript = GetComponentInParent<PancakeBehaviourScript> ();

		//ignoring collisions with sibling rigid bodies for the trigger circleCollider
		CircleCollider2D circle = gameObject.GetComponent<CircleCollider2D> ();
		//the trigger signals start working before the start function is called.
		//If I call is on collision enter before Start() is finalised, I get an error such that parentScript is not set. 
		//I enable the trigger collider ONLY after I set the script pointer.
		circle.enabled = true; 
		GameObject parentGameObject = this.transform.parent.gameObject;
		for (int i = 0; i < parentGameObject.transform.childCount - 1; i++) {
			string name = parentGameObject.transform.GetChild (i).transform.name;
			if (name == "rigidBody1"  || name == "rigidBody2" ){
				BoxCollider2D siblingBox = parentGameObject.transform.GetChild (i).GetComponent<BoxCollider2D> ();
				Physics2D.IgnoreCollision(circle, siblingBox);
			}
		}
		//setting the initial colour of the material:
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		//if (!parentsScript.isStacked()) {
		//	transform.rotation = Quaternion.identity;
		//}
	}

	void OnTriggerEnter2D (Collider2D other){
		//calling the parent for triggerring:
		GameObject collidedGameObject = other.gameObject;
		if (other.CompareTag ("Ground")) {
			parentsScript.checkForDeathSignal ();
		} 
		else if (other.CompareTag ("Player") || other.CompareTag ("SinglePancakeConnectRigidBody")) {
			parentsScript.triggerCollisionFromMainChild (other);
		}
	}



}
