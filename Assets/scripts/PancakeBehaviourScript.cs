using UnityEngine;
using System.Collections;

public class PancakeBehaviourScript : MonoBehaviour {
	//pointer to the rigid body that will do stacking:
	Rigidbody2D rigidBodyOfInterest;
	//pointers to the rigid bodies on sides (used in throwing):
	Rigidbody2D sideRigidBody1;
	Rigidbody2D	sideRigidBody2;
	//pointers to the owner and script of owner
	private GameObject owner;
	private CharacterBehaviourScript ownerScript;
	int ownerID;
	//Spring parameters:
	float springPancakeToPancakeDistance = 1.9f;
	float springPancakeToPlateDistance = 1.9f;
	float springPancakePlacement = 0.8f;
	float springPlatePlacement = 1f;
	float sideSpringPancakePlacement = 0f;
	float sideSpringPlatePlacement = 2.4f;
	float springPancakeToPancakeFrequency = 25.0f;
	float springPancakeToPlateFrequency = 25.0f;
	float springPancakeToPancakeDamping = 1f;
	float springPancakeToPlateDamping = 1f;
	float nonStackedravityScale = 0.1f;

	bool stacked = false;
	bool thrown = false;
	bool dropped = false;
	bool stackingToPlate = false;
	bool topPancake = false;
	public bool flippedAttached = false;

	//for deleting the object when hit the ground:

	bool pancakeDeletionUsed = true;
	bool deathTimerActive = false;
	float deathTime = 1000.0f; 
	float deathTimeBuffer = 10.0f; //time between the pancake hitting the ground, and being destoryed

	//throw parametera:
	float	minthrowSpeed = 20f; //this is the minimum velocity of pancake throw
	float 	throwSpeed = 200f; //this will be added on top of minimum speed through scaling with the trigger input
	//highlight parameters:
	//public SpriteRenderer highlightSprite;

	void Start () {
		//set the poitner to the rigid body of interest:
		for (int i = 0; i < gameObject.transform.childCount - 1; i++) {
			if (gameObject.transform.GetChild (i).transform.name == "rigidBody0") {
				rigidBodyOfInterest = gameObject.transform.GetChild (i).GetComponent<Rigidbody2D> ();
			}
			if (gameObject.transform.GetChild (i).transform.name == "rigidBody1") {
				sideRigidBody1 = gameObject.transform.GetChild (i).GetComponent<Rigidbody2D> ();
			}
			if (gameObject.transform.GetChild (i).transform.name == "rigidBody2") {
				sideRigidBody2 = gameObject.transform.GetChild (i).GetComponent<Rigidbody2D> ();
			}
		}
		//highlightSprite = rigidBodyOfInterest.gameObject.GetComponent<SpriteRenderer> ();
		//highlightSprite.enabled = false;
		//set the pointer to the selection sprite renderer:
		/*foreach (GameObject childOb in gameObject) {
			if (childOb.name == "rigidBody0") {
				rigidBodyOfInterest = childOb;
				break;
			}
		}*/
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (deathTimerActive) {
			checkForDeletion();
		}
	}
	
	public void triggerCollisionFromMainChild (Collider2D other){
		//This is trigger by the child that can stick to other pancakes.
		//Debug.Log ("in collision trigger through child");
		if (canStack ()) {
			//Debug.Log ("can stack");
			//Debug.Log (gameObject.name);
			if (collidedToStackAble (other)) {
				//Debug.Log ("collided object is stackable");
				adjustParametersUponStack (other);
				activateSprings (other.gameObject.GetComponentInParent<Rigidbody2D> ());
				ownerScript.addRigidBodyToStack (rigidBodyOfInterest);
			}
		} else {
			//the pancake has collided with something, it cannot stack.
			//If this is a stacked pancake, chack if the collided object is a "bullet".
			//For now, only thrown pancakes, coming from other players are counted as such.
			//Then it means the player owning this pancake has been hit.
			if (stacked){
				if (hitByattacker(other)){
					ownerScript.checkHitLevel(other);
					//Debug.Log ("Pancake owned by Player"+ownerID+": I am hit! (mid)");
				}
			}
		}
	}

	public void triggerCollisionFromSideChild (Collider2D other){
		//This is triggered by the childs on the sides, they cannot stick to other pancakes, they need
		//to check if they are hit by an attacker.
		//Debug.Log ("in collision trigger through child");
		if (stacked){
			if (hitByattacker(other)){
				ownerScript.checkHitLevel(other);
				//Debug.Log ("Pancake owned by Player"+ownerID+": I am hit! (side)");
			}
		}
	}

	bool canStack(){
		if (thrown) {
			return false;
		}
		if (stacked) {
			return false;
		}
		if (dropped) {
			return false;
		}
		return true;
	}

	bool hitByattacker(Collider2D other){
		//Debug.Log ("collidedwith:");
		//Debug.Log (other.tag);
		GameObject collidedGameObject = other.gameObject;
		if (collidedGameObject.CompareTag ("SinglePancakeConnectRigidBody") || collidedGameObject.CompareTag ("SinglePancakeSideRigidBody")) {
			PancakeBehaviourScript script = collidedGameObject.GetComponentInParent<PancakeBehaviourScript> ();
			//Debug.Log ("is pancake stacked?");
			//Debug.Log (script.isStacked ());
			if (script.isThrown ()) {
				//the pancake I collided with was thrown
				//is it my own thrown pancake?
				if (ownerID != script.getOwnerID()){
					//owner of this panccake is not same as my owner, I am hit by an attacker!
					return true;
				}
			}
		}
		return false;
	}

	public bool isStacked(){
		return stacked;
	}

	public bool isFlippedAttached(){
		return flippedAttached;
	}

	public bool isStackingToPlate(){
		return stackingToPlate;
	}

	public bool isThrown(){
		return thrown;
	}

	bool isTopPancake(){
		return topPancake;
	}
	//bool amIHitInCorrectRigidBody2D(GameObject other){
	//	Rigidbody2D parentRigidBody = other.gameObject.GetComponentInParent<Rigidbody2D> ();
	//	return (parentRigidBody == rigidBodyOfInterest);
	//}

	public Rigidbody2D getSideRigidBody1(){
		return sideRigidBody1;
	}

	public Rigidbody2D getSideRigidBody2(){
		return sideRigidBody2;
	}
	void adjustParametersUponStack(Collider2D other){
		if (stackingToPlate) {
			//Debug.Log ("stackingToPlate");
			//increment the stack size
			//CharacterBehaviourScript script = other.GetComponent<CharacterBehaviourScript>();
			//script.incrementStackSizeUp();
		} else {
			//toggle top pancake options
			PancakeBehaviourScript script = other.GetComponentInParent<PancakeBehaviourScript> ();
			script.toggleTopPancake();
		}
		ownerScript.incrementStackSizeUp();
		toggleTopPancake();	
		stacked = true;
		gameObject.layer = LayerMask.NameToLayer ("StackedPancake");
		foreach (Transform child in gameObject.transform) {
			child.gameObject.layer = LayerMask.NameToLayer ("StackedPancake");
		}
		rigidBodyOfInterest.gravityScale = 1.0f;
		sideRigidBody1.gravityScale = 1.0f;
		sideRigidBody2.gravityScale = 1.0f;

		//Debug.Log ("isTopPancakeAfterStack?");
		//Debug.Log (topPancake);
	}
	
	public void toggleTopPancake(){
		if (topPancake) {
			topPancake = false;
		} else {
			topPancake = true;
		}
	}

	void activateSideSprings (Rigidbody2D collidedRigidbody){
		SpringJoint2D springJoint1 = sideRigidBody1.gameObject.GetComponent <SpringJoint2D> ();
		SpringJoint2D springJoint2 = sideRigidBody2.gameObject.GetComponent <SpringJoint2D> ();
		if (stackingToPlate) {
			//the pancake is stacking to plate, need to adjust spring attachment parameters (dafaults are for other pancakes).
			springJoint1.connectedAnchor = new Vector2 (-1.0f*sideSpringPlatePlacement, 0);
			springJoint1.dampingRatio = springPancakeToPlateDamping;
			springJoint1.frequency = springPancakeToPlateFrequency;
			springJoint2.connectedAnchor = new Vector2 (sideSpringPlatePlacement, 0);
			springJoint2.dampingRatio = springPancakeToPlateDamping;
			springJoint2.frequency = springPancakeToPlateFrequency;
		} 
		if(flippedAttached){
			Vector2 v1 = springJoint1.connectedAnchor;
			springJoint1.connectedAnchor = new  Vector2(-1.0f*v1[0], 0);
			Vector2 v2 = springJoint2.connectedAnchor;
			springJoint2.connectedAnchor = new  Vector2(-1.0f*v2[0], 0);
		}
		if (stackingToPlate) {
			springJoint1.connectedBody = collidedRigidbody;
			springJoint1.enabled = true;
			springJoint2.connectedBody = collidedRigidbody;
			springJoint2.enabled = true;
		} else {
			PancakeBehaviourScript script = collidedRigidbody.gameObject.GetComponentInParent<PancakeBehaviourScript> ();
			bool pancakeBelowIsFlipped = script.isFlippedAttached ();
			if (flippedAttached && pancakeBelowIsFlipped) {
				springJoint1.connectedBody = script.getSideRigidBody1 ();
				springJoint2.connectedBody = script.getSideRigidBody2 ();
			} else if (flippedAttached || pancakeBelowIsFlipped) {
				//one of the pancakes is flipped, I need to attach side 1 to side 2 and vice versa
				springJoint1.connectedBody = script.getSideRigidBody2 ();
				springJoint2.connectedBody = script.getSideRigidBody1 ();
			} else {
				//nothing is flipped:
				springJoint1.connectedBody = script.getSideRigidBody1 ();
				springJoint2.connectedBody = script.getSideRigidBody2 ();
			}	
			springJoint1.enabled = true;
			springJoint2.enabled = true;
		}
	}
	
	void activateSprings(Rigidbody2D collidedRigidbody){
		SpringJoint2D[] springJoints = rigidBodyOfInterest.gameObject.GetComponents <SpringJoint2D> ();
		bool foundFirst = false;
		foreach (SpringJoint2D springJoint in springJoints) {
			if (stackingToPlate){
				//the pancake is stacking to plate, need to adjust spring attachment parameters (dafaults are for other pancakes).
				Vector2 v = springJoint.connectedAnchor;
				springJoint.connectedAnchor = new Vector2(v[0]/springPancakePlacement*springPlatePlacement, 0);
				foundFirst = true;
				springJoint.dampingRatio = springPancakeToPlateDamping;
				springJoint.frequency = springPancakeToPlateFrequency;
				springJoint.distance = springPancakeToPlateDistance;
			}
			else{
				//attaching to another pancake, if the pancake below is flipper during attachemnt, I should rotate the 
				// attachment (same as if this pancake is flipped).
				//if this pancake is also flipped, the rotation will be repeated, 
				//briging the connectin to norma n=once again in the below if clause
				PancakeBehaviourScript script = collidedRigidbody.gameObject.GetComponentInParent<PancakeBehaviourScript> ();
				if(script.isFlippedAttached()){
					Vector2 v = springJoint.connectedAnchor;
					springJoint.connectedAnchor = new  Vector2(-1.0f*v[0], 0);
				}
			}
			float zRot = (rigidBodyOfInterest.transform.rotation.eulerAngles.z % 360 ) ;
			//Debug.Log ("zRot:");
			//Debug.Log (zRot);
			if ((zRot > 90 && zRot < 270.0) ||  (zRot < -90 && zRot > -270 )) { 
				//the colliding PancakeBehaviourScript is upside down, 
				//I will flip the springPlatePlacement connections, such that the springs will still be cross:
				Vector2 v = springJoint.connectedAnchor;
				springJoint.connectedAnchor = new  Vector2(-1.0f*v[0], 0);
				flippedAttached = true;
			}
			springJoint.connectedBody = collidedRigidbody;
			springJoint.enabled = true;
		}
		activateSideSprings (collidedRigidbody);
	}
	
	bool collidedToStackAble(Collider2D other){
		//Debug.Log ("collidedwith:");
		//Debug.Log (other.tag);
		GameObject collidedGameObject = other.gameObject;
		if (collidedGameObject.CompareTag("Player") ) {
			//collided with plate:
			CharacterBehaviourScript script = collidedGameObject.GetComponentInParent <CharacterBehaviourScript> ();
			if (!script.collidedToPlateFromTop(other,rigidBodyOfInterest.transform.position.y)){
				//the collision must be from above, and to the plate, not to the feet!
				return false;
			}
			if (script.getSizeOfStack () > 0) {
				//the pile has other pancakes, I do not sack on this pancake 
				return false;
			} else {
				if (script.checkIfHasSpaceForStacking ()) {
					stackingToPlate = true;
					ownerScript = script;
					ownerID = ownerScript.getPlayerId();
					return true;
				} else {
					//the stack is full, there can be no more pancakes
					return false;
				}
			}
		} else if ( collidedGameObject.CompareTag("SinglePancakeConnectRigidBody")) {
			//Debug.Log ("collided with singePancake");
			//the collision is not with the base, it is with another pancake:
			//did I hit the stackable pancake rigid body?
			PancakeBehaviourScript script = collidedGameObject.GetComponentInParent<PancakeBehaviourScript> ();
			//Debug.Log ("is pancake stacked?");
			//Debug.Log (script.isStacked ());
			if (script.isStacked ()) {
				//Debug.Log ("pancake is stacked");
				//pancake is stacked:
				if (script.isTopPancake ()) {		
					//Debug.Log("Pancake is top pancake");
					//pancake is top pancake, 
					if (collidedGameObject.transform.position.y < rigidBodyOfInterest.transform.position.y){
						//the collision is from above
						if (script.getOwnerScript().checkIfHasSpaceForStacking()){
							//the owner plate of the pancake it collided with has space:
							//can stack now:
							ownerScript = script.getOwnerScript();
							ownerID = ownerScript.getPlayerId();
					 		return true;
						}
					}
				}
			}
		} 
		return false;
	}

	public void checkForDeathSignal(){
		//Debug.Log("WeaponNum = " + WeaponNum.ToStiring());
		if (!isStacked ()) {
			deathTimerActive = true;
			deathTime = Time.time+deathTimeBuffer;
			//Debug.Log ("initiating timer for deat signal, time of death" + deathTime +" current time: "+ Time.time);
		}
	}

	void checkForDeletion(){
		//Debug.Log ("checking if time is up!, time of death" + deathTime +" current time: "+ Time.time);
		if (Time.time > deathTime) {
			if (!isStacked ()){
				//kill the pancake
				if(pancakeDeletionUsed){
					Object.Destroy(this.gameObject);
				}
			}
			else{
				//this pancane cannot die!
				deathTimerActive = false;
				deathTime = 1000.0f;
			}
		}
	}

	CharacterBehaviourScript getOwnerScript(){
		return ownerScript;
	}

	public int getOwnerID(){
		return ownerID;
	}

	public void reConnectSpringsBasedOnThisPancake(Rigidbody2D reAttachmentBase){
		SpringJoint2D[] ownSpringJoints = rigidBodyOfInterest.gameObject.GetComponents <SpringJoint2D> ();
		SpringJoint2D[] baseSpringJoints = reAttachmentBase.gameObject.GetComponents <SpringJoint2D> ();
		for (int i =0; i<2; ++i) {
			Vector2 v = baseSpringJoints [i].connectedAnchor;
			ownSpringJoints [i].connectedAnchor = new Vector2 (v [0], v [1]);
			ownSpringJoints [i].dampingRatio = baseSpringJoints [i].dampingRatio;
			ownSpringJoints [i].frequency = baseSpringJoints [i].frequency;
			ownSpringJoints [i].distance = baseSpringJoints [i].distance;
			ownSpringJoints[i].connectedBody = baseSpringJoints[i].connectedBody;
		}
		PancakeBehaviourScript scriptOfBase = reAttachmentBase.gameObject.GetComponentInParent <PancakeBehaviourScript> ();
		if (scriptOfBase.isFlippedAttached ()) {
			flippedAttached = true;
		}
		if (scriptOfBase.isStackingToPlate() ) {
			stackingToPlate = true;
		}
		SpringJoint2D ownSideJoint1 = sideRigidBody1.gameObject.GetComponent <SpringJoint2D> ();
		SpringJoint2D ownSideJoint2 = sideRigidBody2.gameObject.GetComponent <SpringJoint2D> ();
		SpringJoint2D baseSideJoint1 = scriptOfBase.getSideRigidBody1().gameObject.GetComponent <SpringJoint2D> ();
		SpringJoint2D baseSideJoint2 = scriptOfBase.getSideRigidBody2().gameObject.GetComponent <SpringJoint2D> ();

		Vector2 v1 = baseSideJoint1.connectedAnchor;
		ownSideJoint1.connectedAnchor = new Vector2 (v1 [0], v1 [1]);
		Vector2 v2 = baseSideJoint2.connectedAnchor;
		ownSideJoint2.connectedAnchor = new Vector2 (v2 [0], v2 [1]);
		ownSideJoint1.dampingRatio = baseSideJoint1.dampingRatio;
		ownSideJoint2.dampingRatio = baseSideJoint2.dampingRatio;
		ownSideJoint1.frequency = baseSideJoint1.frequency;
		ownSideJoint2.frequency = baseSideJoint2.frequency;
		ownSideJoint1.distance = baseSideJoint1.distance;
		ownSideJoint2.distance = baseSideJoint2.distance; 
		ownSideJoint1.connectedBody = baseSideJoint1.connectedBody;
		ownSideJoint2.connectedBody = baseSideJoint2.connectedBody;
	}

	public void throwThisPancake(float attackSpeed, int side){
		//first disable the springs:
		SpringJoint2D[] ownSpringJoints = rigidBodyOfInterest.gameObject.GetComponents <SpringJoint2D> ();
		ownSpringJoints[0].enabled = false;
		ownSpringJoints[1].enabled = false;
		SpringJoint2D sideSpringJoints1 = sideRigidBody1.gameObject.GetComponent <SpringJoint2D> ();
		SpringJoint2D sideSpringJoints2 = sideRigidBody2.gameObject.GetComponent <SpringJoint2D> ();
		sideSpringJoints1.enabled = false;
		sideSpringJoints2.enabled = false;

		//then reset stackable boolean:
		stacked = false;
		//adjust gravity
		rigidBodyOfInterest.gravityScale = nonStackedravityScale;
		sideRigidBody1.gravityScale = nonStackedravityScale;
		sideRigidBody2.gravityScale = nonStackedravityScale;
		gameObject.layer = LayerMask.NameToLayer ("SinglePancake");
		foreach (Transform child in gameObject.transform) {
			child.gameObject.layer = LayerMask.NameToLayer ("SinglePancake");
		}
		thrown = true;
		//add velocity:
		//attackSpeed is coming from a trigger ranging from -1 - +1, so it is scaled to 2.
		//(attackSpeed+1)*0.5 is scaled in (0 - 1).
		//I want everything to be thrown with a minimal velocity, and the rest added on top with this scale
		//float vx = minthrowSpeed + (attackSpeed+1)*0.5f*throwSpeed;
		//Debug.Log ("inside pancake throw script: attackSpeed - "+attackSpeed);
		float vx = 0.0f;
		if (side == 2) {
			vx = minthrowSpeed + (attackSpeed + 1) * 0.5f * throwSpeed;
			vx = -1.0f * vx;
		} else {
			vx = minthrowSpeed + (attackSpeed+1)*0.5f*throwSpeed;
		}
		//disable friction:
		rigidBodyOfInterest.GetComponent<BoxCollider2D> ().sharedMaterial.friction = 0.0f;
		sideRigidBody1.GetComponent<BoxCollider2D> ().sharedMaterial.friction = 0.0f;
		sideRigidBody2.GetComponent<BoxCollider2D> ().sharedMaterial.friction = 0.0f;
		//give elocity:
		rigidBodyOfInterest.velocity = new Vector2 (vx, 0);
		sideRigidBody1.velocity = new Vector2 (vx, 0);
		sideRigidBody2.velocity = new Vector2 (vx, 0);
		//increase mass for effective collision:
		rigidBodyOfInterest.mass *= 2.0f;
		sideRigidBody1.mass *= 2.0f;
		sideRigidBody2.mass *= 2.0f;
	}

	//public void highlightOff(){
//		//Debug.Log ("inHighlightOff");
//		highlightSprite.enabled = false;
//	}
	
//	public void highlightOn(){
//		//Debug.Log ("inHighlightOn");
//		highlightSprite.enabled = true;
//	}
	public void disableCollisionsWith(Rigidbody2D other){
		for (int i = 0; i < gameObject.transform.childCount; i++) {
			//Debug.Log ("own child name: "+gameObject.transform.GetChild (i).name);
			//for all 2D colliders in all my children:
			Collider2D[] allOwn2Dcolliders = gameObject.transform.GetChild (i).GetComponents<Collider2D> ();
			//for all 3D collidersd:
			Collider[] allOwn3Dcolliders = gameObject.transform.GetChild (i).GetComponents<Collider> ();
			for (int j = 0; j < other.transform.root.childCount; j++) {
				//Debug.Log ("   other child name: "+other.transform.root.GetChild (j).name);
				//for all 2D colliders in the others children:
				Collider2D[] allOther2Dcolliders = other.transform.root.GetChild (j).GetComponents<Collider2D> ();
				Collider[] allOther3Dcolliders = other.transform.root.GetChild (j).GetComponents<Collider> ();
				foreach (Collider2D coll2Down in allOwn2Dcolliders){
					foreach (Collider2D coll2Dother in allOwn2Dcolliders){
						Physics2D.IgnoreCollision(coll2Down, coll2Dother);
					}
				}
				//foreach (Collider coll3Down in allOther3Dcolliders){
				//	foreach (Collider coll3Dother in allOther3Dcolliders){
				//		Physics.IgnoreCollision(coll3Down, coll3Dother);
				//	}
				//}
			}
		}
	}

	public void dropPancakeFromStack(){
		//first disable the springs:
		SpringJoint2D[] ownSpringJoints = rigidBodyOfInterest.gameObject.GetComponents <SpringJoint2D> ();
		ownSpringJoints[0].enabled = false;
		ownSpringJoints[1].enabled = false;
		SpringJoint2D sideSpringJoints1 = sideRigidBody1.gameObject.GetComponent <SpringJoint2D> ();
		SpringJoint2D sideSpringJoints2 = sideRigidBody2.gameObject.GetComponent <SpringJoint2D> ();
		sideSpringJoints1.enabled = false;
		sideSpringJoints2.enabled = false;

		//then reset stackable boolean:
		stacked = false;
		//adjust gravity
		rigidBodyOfInterest.gravityScale = nonStackedravityScale;
		sideRigidBody1.gravityScale = nonStackedravityScale;
		sideRigidBody2.gravityScale = nonStackedravityScale;
		gameObject.layer = LayerMask.NameToLayer ("SinglePancake");
		foreach (Transform child in gameObject.transform) {
			child.gameObject.layer = LayerMask.NameToLayer ("SinglePancake");
		}
		dropped = true;
	}
}
