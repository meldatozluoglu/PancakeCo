using UnityEngine;
using System.Collections;

public class PancakeInitiator : MonoBehaviour {
	public Camera cam;
	bool gameStopped = false;
	//public GameObject pancake;

	private Vector3 SpawnPoint;

	//parameters for pancake release:
	private float pancakeBakeRate = 1.0F;
	private float nextPancakeRelease = 1.0F;
	float maxWidth;
	float spawnRangeXMax = 0.95f;
	float spawnRangeXMin = 0.05f; // ftom 5% Touch 95% OffMeshLink the TouchPhase of the screen Canvas spawn pancake
	float step = 0.0001f;

	void OnEnable (){
		if (cam == null ) {
			cam = Camera.main;
		}
		gameStopped = false;
		Vector3 upperCorner = new Vector3 (Screen.width, Screen.height, 0.0f);
		Vector3 targetWidth = cam.ScreenToWorldPoint (upperCorner);
		//I want spawn point 1 to be on the left corner, 5% into the game, and spaw point2 to be on right corner
		maxWidth = targetWidth.x;
		step = (spawnRangeXMax - spawnRangeXMin) / 40.0f;
		SpawnPoint.x = maxWidth * (0.5f);
		SpawnPoint.y = transform.position.y;		
		SpawnPoint.z = 0.0f;
		//subscribing this function to the endGameEvent emitted by UIController class
		//the structure of the function (no puts, no outputs), is defined by the UIControler class.
		UIController.stopGameEvent += stopGameForPancakeInitiator; 
		UIController.resumeGameEvent += resumeGameForPancakeInitiator; 

	}

 	void Start() {

	}

	void FixedUpdate(){
		if (!gameStopped && Time.time > nextPancakeRelease) {		
			//I want up to 50% noise on pancake bake rate, so it can be in hte rane 50% - 150% of base rate: 
			int n = Random.Range (50,150);
			nextPancakeRelease = Time.time + pancakeBakeRate * n/100.0f;
			n = Random.Range (-40, 40);
			float randomNumber = n * step;
			SpawnPoint.x = maxWidth * randomNumber;
			Instantiate(Resources.Load("PancakeRagDoll"),SpawnPoint, Quaternion.identity);
		}
	}

	void stopGameForPancakeInitiator(){
		gameStopped = true;
	}

	void resumeGameForPancakeInitiator(){
		gameStopped = false;
	}
}

