using UnityEngine;
using System.Collections;

public class CollectableInitiator : MonoBehaviour {
	bool gameStopped = false;

	//parameters for collectable release:
	private Vector3 SpawnPoint;
	private Vector3 SpawnVel;
	//private float randomPrecision = 10.0f;
	private float maxVel;

	void OnEnable (){
		gameStopped = false;
		//subscribing this function to the endGameEvent emitted by UIController class
		//the structure of the function (no puts, no outputs), is defined by the UIControler class.
		UIController.stopGameEvent += stopGameForCollectableInitiator; 
		UIController.resumeGameEvent += resumeGameForCollectableInitiator; 
		CharacterBehaviourScript.instantiateCollectablesEvent += instantiateCollectables;

		//maxVel = 100f/randomPrecision;
		maxVel = 50f;
	}

 	void Start() {

	}

	void FixedUpdate(){
			//I want up to 50% noise on pancake bake rate, so it can be in hte rane 50% - 150% of base rate: 
			//int n = Random.Range (50,150);
			//n = Random.Range (-40, 40);
			//float randomNumber = n * step;
			//SpawnPoint.x = maxWidth * randomNumber;
			//Instantiate(Resources.Load("PancakeRagDoll"),SpawnPoint, Quaternion.identity);

	}
	void instantiateCollectables(float x, float y, int id){
		//Debug.Log ("instantiation for colleactables called, input was : "+ id);
		SpawnPoint.x = x;
		SpawnPoint.y = y;
		Debug.Log ("instantiate called for player: "+id);
		GameObject newCollectable = Instantiate (Resources.Load ("Collectable"), SpawnPoint, Quaternion.identity) as GameObject;
		CollectableBehaviourScript script = newCollectable.GetComponent <CollectableBehaviourScript> ();
		script.setOwnerId (id);

		Vector2 velDir = new Vector2 (Random.Range (-100,100), Random.Range (50,100));
		velDir = velDir.normalized;
		float velMag = maxVel; //add noise here if you wish
		//now I have the vector for velocity direction.ß
		script.setInitialVel (velDir.x*velMag, velDir.y*velMag);
		Debug.Log (" velocity: "+velDir.x*velMag+" "+velDir.y*velMag);

	}

	void stopGameForCollectableInitiator(){
		gameStopped = true;
	}

	void resumeGameForCollectableInitiator(){
		gameStopped = false;
	}
}

