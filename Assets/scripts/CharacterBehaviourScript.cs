using UnityEngine;
using System.Collections;
//using Wwise_IDs;

public class CharacterBehaviourScript : MonoBehaviour {
	int playerId;
	float 	maxMoveSpeed = 50f;
	float	negMaxMoveSpeed = -70f;
	float	maxMoveSpeedBase = 70f;
	float 	maxMoveSpeedpancakeBasedIncrement = 5.0f;
	float 	maxMoveSpeedAdditionIncrement = 9f;
	bool 	facingRight = true;
	bool 	jumpPressed = false;
	bool	supportJumpThisStep = false;
	bool	usedUpJumpSupport = false;
	float	jumpSpeedVelAdditionForLongJump = 8f;
	float	jumpSpeedSupportCap = 100f;
	float	jumpSpeedSupportAdded = 0f;
	float 	jumpSpeed = 60f;
	bool 	wasWalkinglaststep = false;
	bool 	jumpedThisStep = false;
	bool 	voluntaryMovement = false;
	int 	stackHeight = 0;
	const int 	maxStackHeight = 12;
	bool 	gameStopped = false;
	//ground check parameters:
	bool grounded = false;
	public Transform groundCheck;
	float groundRadius = 0.4f;
	public LayerMask whatIsGround;
	public GameObject bodyGameObject;

	//pancake selector parameters:
	float	nextSelectorMove = 0.0f;  //seconds
	float	selectorMoveRate = 0.2f;  //seconds
	int		selectedPancake = -1;     //the pancake in the stack that is selected
	bool	pancakeSelectorActive = false;
	float	attackSpeed = -1f;
	float	attackSpeed1 = -1f;
	float	attackSpeed2 = -1f;

	//pancake throw parameters:
	float	nextPancakeThrow = 0.1f;
	float	pancakeThrowRate = 0.2f;
	private Rigidbody2D[] RigidBodiesInStack = new Rigidbody2D[maxStackHeight];

	//hit parameters:
	bool hitThisStep = false;
	bool wasHitThisStep = false;
	int  hitByPlayer = 0;
	float[] hitThreshold;
	int nCollectablesToDrop = 0;
	int nCollectables = 10;
	float nextHitTime = 0f;
	float nextHitDelay = 0.5f;
		
	public delegate void instantiateCollectablesFunction(float x, float y, int id);
	public static event  instantiateCollectablesFunction instantiateCollectablesEvent;

	//int selectedPancake;
	//Color startcolor;
	//Color highlightColor;
	//float[][] eyeOffsets;
	
	//parameters for pancake selector movement:
	//private float selectorMoveRate = 0.2F;
	//private float nextSelectorMove = 0.0F;
	//parameters for pancake throwing (cannot throw one after another)
	//private float pancakeThrowRate = 0.2F;
	//private float nextPancakeThrow = 0.0F;
	
	
	
	//private Vector<Rigidbody2D> TopRigidBody2D;
	//private const int maxStackHeight = 6;
	//public int stackHeight;

	void OnEnable (){
		//subscribing this function to the endGameEvent emitted by UIController class
		//the structure of the function (no puts, no outputs), is defined by the UIControler class.
		UIController.stopGameEvent += stopGameForCharacter; 
		UIController.resumeGameEvent += resumeGameForCharacter; 
	}
	// Use this for initialization
	void Start () {
		if (gameObject.name == "Character1") {
			playerId = 1;
		}else{
			playerId =2;
		}
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ground"), LayerMask.NameToLayer("Player"), true);

		hitThreshold = new float[2];
		hitThreshold [0] = 4000;
		hitThreshold [1] = 2000;
		/*for (int i = 0; i < gameObject.transform.childCount - 1; i++) {
			if (gameObject.transform.GetChild (i).transform.name == "body") {
				bodyGameObject = gameObject.transform.GetChild (i).GetComponent<GameObject> ();
				break;
			}
		}*/
				

		/*bool foundFeet = false;
		bool foundPlate = false;
		Collider2D feetCollider  = gameObject.transform.GetChild (1).GetComponent<Collider2D> ();
		Collider2D plateCollider = gameObject.transform.GetChild (0).GetComponent<Collider2D> ();
		for (int i = 0; i < gameObject.transform.childCount; i++) {
			if (gameObject.transform.GetChild (i).name == "feet") {
				feetCollider = gameObject.transform.GetChild (i).GetComponent<Collider2D> ();
				foundFeet = true;
			}
			if (gameObject.transform.GetChild (i).name == "plate") {
				plateCollider = gameObject.transform.GetChild (i).GetComponent<Collider2D> ();
				foundPlate = true;
			}
		}
		if (foundFeet && foundPlate){
			Physics2D.IgnoreCollision(feetCollider, plateCollider);
		}*/


		//RigidBodiesInStack [0] = GetComponent<Rigidbody2D> ();
		//stackHeight = 0;
		//pancakeThickness = 2.0f;
		/*foreach (GameObject childOb in gameObject) {
			if (childOb.name == "eye1") {
				eye1 = childOb;
			}
			if (childOb.name == "eye2") {
				eye2 = childOb;
			}
		}*/
		//eye1 = GameObject.Find("Hero1/eye1");
		//eye2 = GameObject.Find("Hero1/eye2");
		/*for (int i = 0; i < gameObject.transform.childCount; i++) {
			if (gameObject.transform.GetChild (i).transform.name == "eye1") {
				eye1 = gameObject.transform.GetChild (i).gameObject;
			} else if (gameObject.transform.GetChild (i).transform.name == "eye2") {
				eye2 = gameObject.transform.GetChild (i).gameObject;
			} else if (gameObject.transform.GetChild (i).transform.name == "pancakeBase") {
				pancakeBase = gameObject.transform.GetChild (i).gameObject;
			}
		}

		eyeOffsets = new float[2][];
		for (int i = 0; i < 2; i++) {
			eyeOffsets[i] = new float[2];
		}
		eyeOffsets[0][0] = eye1.transform.localPosition.x;
		eyeOffsets[0][1] = eye1.transform.localPosition.y;
		eyeOffsets[1][0] = eye2.transform.localPosition.x;
		eyeOffsets[1][1] = eye2.transform.localPosition.y;

		selectedPancake = 0;
		 */
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Player" + playerId + "Jump") && grounded) {
			jumpPressed = true;
			usedUpJumpSupport = false; //this jump can be supported as long as player keeps pressing the jump key
			//it will be used up once the maximum velocity is reached, or the player started to slow down
			jumpSpeedSupportAdded = 0;
			//GetComponent<Rigidbody2D> ().gravityScale = jumpGravityScale;
			//gravityAltered = true;
		}
		if (!jumpPressed&& !usedUpJumpSupport && !grounded && Input.GetButton ("Player" + playerId + "Jump") ){
			/// I am mid air, I started jumping at least one step prior to this I am still pressing the jump button,
			/// the initiation of jump must be at least one step before, so that my velocity check below will have meaning
			if(GetComponent<Rigidbody2D> ().velocity.y > 0.01) {
				//I have not started moving down.
				//I should keep supportig this jump:
				supportJumpThisStep = true;
			}
			else{
				usedUpJumpSupport = true;
				//the payer is still mid air, and pressing the jump key, but the player started to fall down. From this point on the jump is used up.
			}
		}
	}

	void FixedUpdate () {
		//check jump velocity:
		adjustMaxForces ();
		//check if grouned:
		grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);
		//Horizontal movement:
		if (!gameStopped) {
			if (hitThisStep && hitByPlayer >0){
				Debug.Log ("calling drop, time: "+Time.time+" next hit time: "+nextHitTime);
				dropCollectable();
				wasHitThisStep = true;
			}
			float move = Input.GetAxis ("Player" + playerId + "Horizontal");

			//float attackSpeed = Input.GetAxis ("Trigger1");
			checkForVoluntaryMovement (move);
			if (voluntaryMovement) {
				//Debug.Log ("moving player: "+playerId+" from input: "+"Player" + playerId + "Horizontal");
				//updateFacingDirection (move);
			}
			//float xVel = move * maxMoveSpeed;
			//GetComponent<Rigidbody2D> ().velocity = new Vector2 (xVel, GetComponent<Rigidbody2D> ().velocity.y);
			float currX = GetComponent<Rigidbody2D> ().velocity.x;
			//this is added for solid stop, if you need wobbly stop remove this if loop:
			if (move == 0) {
				GetComponent<Rigidbody2D> ().velocity = new Vector2 (move, GetComponent<Rigidbody2D> ().velocity.y);
			}
			if ((currX >= 0 && currX < maxMoveSpeed) || (currX <= 0 && currX > negMaxMoveSpeed)) {
				float xVel = GetComponent<Rigidbody2D> ().velocity.x + move * maxMoveSpeedpancakeBasedIncrement;
				GetComponent<Rigidbody2D> ().velocity = new Vector2 (xVel, GetComponent<Rigidbody2D> ().velocity.y);
				if (voluntaryMovement){
					if(facingRight){
						if(xVel<0){
							Flip ();
						}
					}else if(xVel>0){
						Flip ();
					}
				}
				//Debug.Log ("move: "+move+" maxMoveSpeed: "+maxMoveSpeed+" curr xVel: "+xVel);
			}
			//jump:
			if (jumpPressed) {
				jumpPressed = false;
				GetComponent<Rigidbody2D> ().velocity = new Vector2 (GetComponent<Rigidbody2D> ().velocity.x, jumpSpeed);
				jumpedThisStep = true;
				//Debug.Log ("jumping player: "+playerId+" from input: "+"Player" + playerId + "Jump");
			}
			if (supportJumpThisStep){
				/*float newYVel = GetComponent<Rigidbody2D> ().velocity.y + jumpSpeedVelAdditionForLongJump;
				jumpSpeedSupportAdded = jumpSpeedSupportAdded+jumpSpeedVelAdditionForLongJump;
				if (jumpSpeedSupportAdded > jumpSpeedSupportCap){
					usedUpJumpSupport = true;
				}
				else{
					GetComponent<Rigidbody2D> ().velocity = new Vector2 (GetComponent<Rigidbody2D> ().velocity.x, newYVel);
				}
				supportJumpThisStep = false
				*/
				float newYVel = GetComponent<Rigidbody2D> ().velocity.y + (Time.deltaTime*240);
				jumpSpeedSupportAdded = jumpSpeedSupportAdded+jumpSpeedVelAdditionForLongJump;
				if (jumpSpeedSupportAdded > jumpSpeedSupportCap){
					usedUpJumpSupport = true;
				}
				else{
					GetComponent<Rigidbody2D> ().velocity = new Vector2 (GetComponent<Rigidbody2D> ().velocity.x, newYVel);
				}
				supportJumpThisStep = false;
			}
			//pancake throw with buttons:
			//checkPancakeThrowWithButtons();
			//pancake throw with trigger method:
			if (stackHeight>0){
				checkPancakeThrowWithTriggers();
			}
			//check if pancake stack is braking:
			bool stackFall = checkStackFall ();
		}
	}
	
	void Flip (){
		facingRight = !facingRight;
		Vector3 theScale = bodyGameObject.transform.localScale;
		theScale.x *= -1;
		bodyGameObject.transform.localScale = theScale;
		//flipFace ();
	}
	
	void checkForVoluntaryMovement (float move){
		voluntaryMovement = false;
		float moveThreshold = 0.01f;
		if (move > moveThreshold|| move < -moveThreshold){
			voluntaryMovement = true;
		}
	}

	public bool isFacingRight(){
		return facingRight;
	}

	public bool isWalking(){
		float vThreshold = 0.01f;
		if (grounded && voluntaryMovement){
			float vx = GetComponent<Rigidbody2D> ().velocity.x;
			if (vx > vThreshold || vx < -vThreshold) {
				wasWalkinglaststep = true;
				return true;
			}
			return false;
		} else {
			wasWalkinglaststep = false;
			return false;
		}
	}
	
	public bool wasWalking(){
		return wasWalkinglaststep;
	}
	
	public bool jumped(){
		return jumpedThisStep;
	}

	public bool wasHit(){
		return wasHitThisStep;
	}

	public void toggleJumpedThisStep(){
		if (jumpedThisStep){
			jumpedThisStep = false;
		} else{
			jumpedThisStep = true;
		}
	}

	public void toggleWasHit(){
		if (wasHitThisStep){
			wasHitThisStep = false;
		} else{
			wasHitThisStep = true;
		}
	}

	public void OnTriggerEnter2D (Collider2D other){
		//the player has collided with something.
		//Check if the collided object is a "bullet".
		//For now, only thrown pancakes, coming from other players are counted as such.
		//Then it means the player owning this pancake has been hit.
		GameObject collidedGameObject = other.gameObject;
		if (collidedGameObject.CompareTag("Collectable") ){
			//collided with a collectable, is it mine?
			checkCollectable(collidedGameObject);
		}else if (Time.time > nextHitTime){
			if (!hitThisStep && hitByattacker (other)) {
				Debug.Log ("calling check hit level, hitThisStep: " + hitThisStep+" time: "+Time.time+" next hit time: "+nextHitTime);
				checkHitLevel(other);
				//Debug.Log ("Player"+playerId+"I am hit!");
			}
		}
	}

	void checkCollectable (GameObject collidedGameObject){
		//Debug.Log ("collided with collectable");
		CollectableBehaviourScript script = collidedGameObject.GetComponentInParent <CollectableBehaviourScript> ();
		if (playerId == script.getOwnerID ()) {
			//collectable is mine!
			//Debug.Log ("collectable is mine");
			addToCollectables (script.getCollectableValue ());
			script.collected ();
		}
	}

	public void checkHitLevel (Collider2D other){
		float xVel = other.gameObject.GetComponent<Rigidbody2D> ().velocity.x;
		float yVel = other.gameObject.GetComponent<Rigidbody2D> ().velocity.y;
		float hitMag = xVel * xVel + yVel*yVel;
		//Debug.Log ("hitvel mag: " + hitMag);
		if (hitMag > hitThreshold [0]) {
			hitThisStep = true;
			nextHitTime = Time.time + nextHitDelay;
			nCollectablesToDrop = 2;
		} else if (hitMag > hitThreshold [1]) {
			hitThisStep = true;
			nextHitTime = Time.time + nextHitDelay;
			nCollectablesToDrop = 1;
		} else {
			hitThisStep = false;
			hitByPlayer = 0;
			nCollectablesToDrop = 0;
		}
	}

	void dropCollectable(){
		//Debug.Log ("dropping collectables: "+nCollectablesToDrop+" hitByPlayer: "+hitByPlayer+ " hitThisStep: " + hitThisStep+" time: "+Time.time+" next hit time: "+nextHitTime);
		float x = transform.position.x;
		float y = transform.position.y;
		while (nCollectablesToDrop>0 && nCollectables >0) {
			//instantiate Collectable
			if (instantiateCollectablesEvent != null){ //this checks if there are any subscribers to this event, if there are none, it is not emitted
				instantiateCollectablesEvent(x,y,hitByPlayer);
			}
			nCollectablesToDrop = nCollectablesToDrop-1;
			nCollectables = nCollectables -1;
		}
		hitThisStep = false;
		hitByPlayer = 0;
		nCollectablesToDrop = 0;
		//Debug.Log ("outside loop: "+nCollectablesToDrop+" hitByPlayer: "+hitByPlayer+ "hitThisStep: " + hitThisStep + " time: "+Time.time+" next hit time: "+nextHitTime);
	}

	public void addToCollectables(int n){
		nCollectables = nCollectables + n;
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
				int attackerId =script.getOwnerID();
				if (playerId != attackerId){
					//I am not the owner of this pancake, I am hit by an attacker!
					hitByPlayer = attackerId;
					return true;
				}
			}
		}
		return false;
	}

	public int getSizeOfStack (){
		return stackHeight;
	}

	public int getCollectableNumber(){
		return nCollectables;
	}

	public bool checkIfHasSpaceForStacking(){
		if (stackHeight < maxStackHeight) {
			return true;
		} else {
			return false;
		}
	}

	public void incrementStackSizeUp(){
		stackHeight++;
	}

	public void addRigidBodyToStack(Rigidbody2D addedRigidBody){
		RigidBodiesInStack [stackHeight-1] = addedRigidBody;
	}

	public void incrementStackSizeDown(){
		stackHeight--;
	}

	public void adjustMaxForces(){
		maxMoveSpeed = maxMoveSpeedBase-stackHeight*maxMoveSpeedpancakeBasedIncrement;
	}
	
	void movePancakeSelectorUp (){
		if (stackHeight > 0) {
			if (selectedPancake > -1) {
				//there is a selected pancake, I will de-selarc that one first
				RigidBodiesInStack [selectedPancake].gameObject.GetComponent<SpriteRenderer> ().enabled = false;
			}
			selectedPancake++;
			if (selectedPancake == stackHeight) {
				selectedPancake = 0;
			}
			RigidBodiesInStack [selectedPancake].gameObject.GetComponent<SpriteRenderer> ().enabled = true;
		}
	}

	void resetSelectedPancake(){
		if (selectedPancake > -1) {
			RigidBodiesInStack [selectedPancake].gameObject.GetComponent<SpriteRenderer> ().enabled = false;
			selectedPancake = -1;
		}
	}

	void throwPancake(int side){
		PancakeBehaviourScript pancakeScript = RigidBodiesInStack [selectedPancake].gameObject.GetComponentInParent<PancakeBehaviourScript> ();
		//turn highlight off:
		RigidBodiesInStack [selectedPancake].gameObject.GetComponent<SpriteRenderer> ().enabled = false;
		//reordering and re-attaching rigid bodies in stack:
		resortRigidBodiesInStack ();
		//resetting selector:
		selectedPancake = -1;
		//disabling collisions:
		//foreach (Rigidbody2D rb in RigidBodiesInStack) {
		for (int i=0; i<stackHeight; ++i){
			pancakeScript.disableCollisionsWith (RigidBodiesInStack[i]);
		}
		pancakeScript.disableCollisionsWith (GetComponent<Rigidbody2D> ());
		//throwing the pancake:
		if (side == 1 ){
			//throw pancake to right:
			if (attackSpeed1<0){
				pancakeScript.throwThisPancake (-1.0f*attackSpeed1,side);
			}
			else{
				pancakeScript.throwThisPancake (attackSpeed1,side);
			}
		}
		if (side == 2 ){
			pancakeScript.throwThisPancake (attackSpeed2,side);
		}
	}

	void checkPancakeThrowWithTriggers(){
		float currAttackSpeed1 = Input.GetAxis ("Player" + playerId + "Attack1");
		//Debug.Log ("currAttackSpeed From attack1: "+currAttackSpeed1+" attackSpeed: "+attackSpeed);
		float currAttackSpeed2 = Input.GetAxis ("Player" + playerId + "Attack2");
		//Debug.Log ("currAttackSpeed From attack2: "+currAttackSpeed2+" attackSpeed: "+attackSpeed);
		selectedPancake = 0;
		if (currAttackSpeed1 > attackSpeed1){
			attackSpeed1 = currAttackSpeed1;
		}
		if (currAttackSpeed2 > attackSpeed2){
			attackSpeed2 = currAttackSpeed2;
		}
		//checking throw to right
		if (attackSpeed1 > -0.9999 && currAttackSpeed1 < -0.9999) {
			//this means the attack speed has been modified after the a pancake has been thrown on this side, and then released
			//Do I have a pancake to throw, and has it been long enough since my last throw?
			if (stackHeight > 0 && Time.time > nextPancakeThrow) {
				//Debug.Log ("Throw pancake called");
				throwPancake (1);
				attackSpeed1 = -1f;
			}
		}
		//checking throw to left
		if (attackSpeed2 > -0.9999 && currAttackSpeed2 < -0.9999) {
			//this means the attack speed has been modified after the a pancake has been thrown on this side, and then released
			//Do I have a pancake to throw, and has it been long enough since my last throw?
			if (stackHeight > 0 && Time.time > nextPancakeThrow) {
				//Debug.Log ("Throw pancake called");
				throwPancake (2);
				attackSpeed2 = -1f;
			}
		}		
	}

	void checkPancakeThrowWithButtons(){
		if (Input.GetButton ("Player" + playerId + "Fire3") && Time.time > nextSelectorMove) {
			nextSelectorMove = Time.time + selectorMoveRate;
			movePancakeSelectorUp ();
		}
		if (Input.GetButtonUp ("Player" + playerId + "Fire3")) {
			resetSelectedPancake ();
		}
		if (Input.GetButtonDown ("Player" + playerId + "Fire1") && selectedPancake > -1 && Time.time > nextPancakeThrow) {
			nextPancakeThrow = Time.time + pancakeThrowRate;
			attackSpeed = 1f;
			throwPancake (1); //this is button throw, always a max speed
			attackSpeed = -1f;
		}
	}

	void resortRigidBodiesInStack(){
		if (selectedPancake < stackHeight - 1) {
			//Debug.Log ("selectedPancake " + selectedPancake);
			//Debug.Log ("stackHeight " + stackHeight);

			PancakeBehaviourScript pancakeScript = RigidBodiesInStack [selectedPancake + 1].gameObject.GetComponentInParent<PancakeBehaviourScript> ();
			pancakeScript.reConnectSpringsBasedOnThisPancake (RigidBodiesInStack [selectedPancake]);
			//the pancake is not the top pancake, spring sorting wil be necessarry:
		} else {
			//the pancake to be thrown is the top pancake, I need to make the pancake below the top pancake, 
			//if there is a pancake below this one:
			if(selectedPancake>0){
				PancakeBehaviourScript pancakeScript = RigidBodiesInStack [selectedPancake - 1].gameObject.GetComponentInParent<PancakeBehaviourScript> ();
				pancakeScript.toggleTopPancake();
			}
		}
		for (int i = selectedPancake; i<stackHeight-1; ++i) {
			RigidBodiesInStack [i] = RigidBodiesInStack [i + 1];
		}
		RigidBodiesInStack [stackHeight-1] = null;
		incrementStackSizeDown();
	}

	public bool collidedToPlateFromTop(Collider2D other, float y){
		if (other.CompareTag("Non-stickCollider")){
			//Debug.Log ("collided to feet");
			return false;
		}
		//Debug.Log ("plate y: " + transform.position.y + "pancake y: " + y);
		if (transform.position.y > y) {
			//the pancake is below me, it should not stack
			return false;
		}
		return true;
	}

	bool checkStackFall(){
		double threshold2 = 2.0 * 2.0;
		if (stackHeight > 0) {
			double x0 = RigidBodiesInStack[0].transform.position.x;
			double y0 = RigidBodiesInStack[0].transform.position.y;
			for (int i = 1; i<stackHeight; ++i) {
				double x1 = RigidBodiesInStack[i].transform.position.x;
				double y1 = RigidBodiesInStack[i].transform.position.y;
				double dx = x0-x1;
				double dy = y0-y1;
				double d2 = dx*dx + dy*dy;
				//Debug.Log ("pos0: "+x0+" "+y0+", pos1: "+x1+" "+y1);
				if (d2>threshold2){
					breakStack(i);
					//Debug.Log("BREAK!");
					return true;
				}
				x0 = x1;
				y0 = y1;
			}
		}
		return false;
	}

	void breakStack(int breakPoint){
		//if there is selected pancake, it will not be selected anymore: 
		if (selectedPancake > 0) {
			RigidBodiesInStack [selectedPancake].gameObject.GetComponent<SpriteRenderer> ().enabled = false;
			//resetting selector:
			selectedPancake = -1;
		}
		//disabling and removing pancakes from the rigid body list
		while (stackHeight>breakPoint) {
			PancakeBehaviourScript pancakeScript = RigidBodiesInStack [stackHeight - 1].gameObject.GetComponentInParent<PancakeBehaviourScript> ();
			pancakeScript.dropPancakeFromStack();
			RigidBodiesInStack [stackHeight - 1] = null;
			stackHeight--;
		}
		//setting the pancake below the break point to be the top pancake:
		PancakeBehaviourScript pancakeScriptNewTop = RigidBodiesInStack [breakPoint - 1].gameObject.GetComponentInParent<PancakeBehaviourScript> ();
		pancakeScriptNewTop.toggleTopPancake();

	}

	public int getPlayerId(){
		return playerId;
	}

	void stopGameForCharacter(){
		gameStopped = true;
	}

	void resumeGameForCharacter(){
		gameStopped = false;
	}
	//ancor parameters of pancake to pancake :
	// x = 0.05, -0.05; distance = 0.5; damping = 0.2; spring  1  distance = 0.2;
	// xpancake = 0.05/00.05, xplate: 0.02/-0.02, damping = 0.2, sprimg 1, distance = 0.2;
	
	/*

	void checkPancakeThrow (){
		if (Input.GetButton ("Fire3") && Time.time > nextSelectorMove) {
			nextSelectorMove = Time.time + pancakeThrowRate;
			movePancakeSelectorUp ();
		}
		if (Input.GetButtonUp ("Fire3")) {
			resetSelectedPancake();
		}
	}

	public void displayName(){
		Debug.Log(gameObject.name);
		//GetComponent<Rigidbody2D> ().velocity = new Vector2 (-10, GetComponent<Rigidbody2D> ().velocity.y);
		//Debug.Log(GetComponent<Rigidbody2D> ().velocity.x);
		//Debug.Log(GetComponent<Rigidbody2D> ().velocity.y);
	}

	public Rigidbody2D getRigidBodyForSprings(){
		//GetComponent<Rigidbody2D> ().velocity = new Vector2 (-10, GetComponent<Rigidbody2D> ().velocity.y);
		return RigidBodiesInStack[stackHeight];
	}




	public void	setRigidBodyForSprings(Rigidbody2D newTopRigidBody){
		stackHeight++;
		RigidBodiesInStack [stackHeight] = newTopRigidBody;
		//updateEyePosition ();
	}
		
	public void updateFace(){
		if (stackHeight > 1) {
			SinglePancake PancakeScript = RigidBodiesInStack [stackHeight - 1].gameObject.GetComponent<SinglePancake> ();
			PancakeScript.turnFaceOff ();
		} else if (stackHeight == 0) {
			eye1.SetActive (true);
			eye2.SetActive (true);
		} else{
			eye1.SetActive(false);
			eye2.SetActive (false);
		}
		if (!facingRight) {
			flipFace ();
		}
	}

	void flipFace(){
		if (stackHeight > 0) {
			SinglePancake PancakeScript = RigidBodiesInStack [stackHeight].gameObject.GetComponent<SinglePancake> ();
			PancakeScript.flipFace ();
		} 
	}

	void movePancakeSelectorUp(){
		if (selectedPancake < stackHeight) {
			//turn off only if a pancake was highlighted, which means the selected pancake must be 1 or above
			if (selectedPancake > 0){
				SinglePancake PancakeScript = RigidBodiesInStack [selectedPancake].gameObject.GetComponent<SinglePancake> ();
				PancakeScript.highlightOff ();
			}
			//Debug.Log ("turned off previous");
			SinglePancake PancakeScript1 = RigidBodiesInStack [selectedPancake+1].gameObject.GetComponent<SinglePancake> ();
			PancakeScript1.highlightOn ();
			selectedPancake++;
		}
		else if (stackHeight > 1 ){
			//I am at the top pancake, and I keep pressing, I want to get back to the bootom.
			//If the stack only has one throwable pancake, then I will not do anything, selector will stay where it is.
			// If the stack has more than one throwable pancake, the press action will bring the selector to the first one.
			//turning of the top pancake highlight
			SinglePancake PancakeScript = RigidBodiesInStack [selectedPancake].gameObject.GetComponent<SinglePancake> ();
			PancakeScript.highlightOff ();
			selectedPancake = 1;
			SinglePancake PancakeScript1 = RigidBodiesInStack [selectedPancake].gameObject.GetComponent<SinglePancake> ();
			PancakeScript1.highlightOn ();
			Debug.Log ("reset selector to bottom");
		}
	}
	
	void movePancakeSelectorDown(){
		Debug.Log ("trying to move down");
		if (selectedPancake > 0) {
			//RigidBodiesInStack [selectedPancake].GetComponent<GameObject> ().GetComponent<Renderer> ().material.color = startcolor;
			//RigidBodiesInStack [selectedPancake-2].GetComponent<GameObject> ().GetComponent<Renderer> ().material.color = highlightColor;
			SinglePancake PancakeScript = RigidBodiesInStack [selectedPancake].gameObject.GetComponent<SinglePancake> ();
			PancakeScript.highlightOff ();
			//turn on the new highlight if there is a pancake before you 
			if (selectedPancake > 1){
				PancakeScript = RigidBodiesInStack [selectedPancake-1].gameObject.GetComponent<SinglePancake> ();
				PancakeScript.highlightOn ();
			}
			selectedPancake--;
		}else {
			Debug.Log ("at bottom already OR stack too small");
		}
	}




	*/
}
