using UnityEngine;
using System.Collections;

public class OnLevelChange : MonoBehaviour {

	// Use this for initialization
	void OnLevelWasLoaded (int level) {
		GameObject gameObjectForCanvas;
		bool activityToggle = true;
		bool deactivateLoadingScreen = false;
		bool deactivateGameOverSceen = false;
		if (level > 1) {
			//loading a game level:
			initiateCharacters((level-2));
		}
		if (level == 1) {
			//main menu is launched
			//the toggle defines the user interface for the game levels, hence it is false for main menu
			activityToggle = false;
			//the Canvas has a loading screen activated inside the MainMenuItems, this is toggeld on upon loading any level
			//if the loaded level is a game level, the "MainMenuItems" are de-activated, hence the loading screen is deactivated through its parent
			//if the current loaded screen is main menu, then the loading scrren should be actively de-activated:
			deactivateLoadingScreen = true;
			deactivateGameOverSceen = true;
		}
		for (int i = 0; i < gameObject.transform.childCount; i++) {
			//Debug.Log ("child Name: "+gameObject.transform.GetChild (i).transform.name );
			if (gameObject.transform.GetChild (i).transform.name == "GameControllers") {
				//gameObjectForGameControlers = gameObject.transform.GetChild (i).gameObject;
				gameObject.transform.GetChild (i).gameObject.SetActive (activityToggle);
				//Debug.Log ("assigned game controller activity setting to: "+activityToggle);
			}
			if (gameObject.transform.GetChild (i).transform.name == "Canvas") {
				gameObjectForCanvas = gameObject.transform.GetChild (i).gameObject;
				for (int j = 0; j < gameObjectForCanvas.transform.childCount; j++) {
					if (gameObjectForCanvas.transform.GetChild (j).transform.name == "SharedLevelItems") {
						//toggling the interface elements shared between all levels:
						gameObjectForCanvas.transform.GetChild (j).gameObject.SetActive (activityToggle);
						if (deactivateGameOverSceen){
							//Debug.Log ("need to deactivate loading screen");
							GameObject gameObjectForSharedLevelItems;
							gameObjectForSharedLevelItems = gameObjectForCanvas.transform.GetChild (j).gameObject;
							for (int k = 0; k < gameObjectForSharedLevelItems.transform.childCount; k++) {
								if (gameObjectForSharedLevelItems.transform.GetChild (k).transform.name == "GameOverText") {
									gameObjectForSharedLevelItems.transform.GetChild (k).gameObject.SetActive (false);
									//Debug.Log ("assigned LoadingScreen activity setting to: "+false);
									break;
								}
							}
						}
						//Debug.Log ("assigned SharedLevelItems activity setting to: "+activityToggle);
					}else if (gameObjectForCanvas.transform.GetChild (j).transform.name == "MainMenuItems") {
						//toggling the main manu items the other way around
						gameObjectForCanvas.transform.GetChild (j).gameObject.SetActive(!activityToggle);
						//Debug.Log ("assigned MainMenuItems activity setting to: "+!activityToggle);
						if (deactivateLoadingScreen){
							//Debug.Log ("need to deactivate loading screen");
							GameObject gameObjectForMainMenuItems;
							gameObjectForMainMenuItems = gameObjectForCanvas.transform.GetChild (j).gameObject;
							for (int k = 0; k < gameObjectForMainMenuItems.transform.childCount; k++) {
								if (gameObjectForMainMenuItems.transform.GetChild (k).transform.name == "LoadingScreen") {
									gameObjectForMainMenuItems.transform.GetChild (k).gameObject.SetActive (false);
									//Debug.Log ("assigned LoadingScreen activity setting to: "+false);
									break;
								}
							}
						}
					}else if (gameObjectForCanvas.transform.GetChild (j).transform.name == "GameLauncherItems") {
						if (level == 0) {
							//game launcher scene is loaded, this should not have happened here, but enabling the image just in case!
							gameObjectForCanvas.transform.GetChild (j).gameObject.SetActive(true);
						}else{		
							//game launcher info image is toggled off for any loaded scene
							gameObjectForCanvas.transform.GetChild (j).gameObject.SetActive(false);
						}
					}
				}
			}
		}
	}
	
	void initiateCharacters(int currLevel){

		Vector3 SpawnPoint;
		SpriteRenderer[] renderer;
		float[][][] CharacterPositions;// [level][player][posx][posy][posz]
		float[][] CharacterColours;// [player][r][g][b]

		CharacterPositions = new float[2][][];	
		CharacterColours = new float[4][];

		for (int i = 0; i < 2; i++) {
			CharacterPositions[i] = new float[4][];
			for (int j = 0; j < 4; j++) {
				CharacterPositions[i][j] = new float[3];
				CharacterColours[j] = new float[3];
			}
		}
		Debug.Log ("initiating characters for level: " + currLevel);
		CharacterColours [0] [0] = 1f;
		CharacterColours [0] [1] = 0.4f;
		CharacterColours [0] [2] = 0.4f;

		CharacterColours [1] [0] = 0.4f;
		CharacterColours [1] [1] = 0.4f;
		CharacterColours [1] [2] = 1f;

		CharacterColours [2] [0] = 0.7f;
		CharacterColours [2] [1] = 1.0f;
		CharacterColours [2] [2] = 0.7f;
		CharacterColours [3] [0] = 0.5f;
		CharacterColours [3] [1] = 0.5f;
		CharacterColours [3] [2] = 0.5f;
		//level 0 pos:
		CharacterPositions [0] [0] [0] = -40f;
		CharacterPositions [0] [0] [1] = -15f;
		CharacterPositions [0] [0] [2] = 0f;
		CharacterPositions [0] [1] [0] = 30f;
		CharacterPositions [0] [1] [1] = -15f;
		CharacterPositions [0] [1] [2] = 0f;
		CharacterPositions [0] [2] [0] = -40f;
		CharacterPositions [0] [2] [1] = -15f;
		CharacterPositions [0] [2] [2] = 0f;
		CharacterPositions [0] [3] [0] = 30f;
		CharacterPositions [0] [3] [1] = -15f;
		CharacterPositions [0] [3] [2] = 0f;
		//level 1 pos:
		CharacterPositions [1] [0] [0] = -32f;
		CharacterPositions [1] [0] [1] = -15f;
		CharacterPositions [1] [0] [2] = 0f;
		CharacterPositions [1] [1] [0] = 25f;
		CharacterPositions [1] [1] [1] = -15f;
		CharacterPositions [1] [1] [2] = 0f;
		CharacterPositions [1] [2] [0] = 53f;
		CharacterPositions [1] [2] [1] = -15f;
		CharacterPositions [1] [2] [2] = 0f;
		CharacterPositions [1] [3] [0] = -25f;
		CharacterPositions [1] [3] [1] = -15f;
		CharacterPositions [1] [3] [2] = 0f;

		int numberOfPlayers = 2;
		for (int i = 0; i < numberOfPlayers; i++) {
			int id = i + 1;
			SpawnPoint.x = CharacterPositions [currLevel] [i] [0];
			SpawnPoint.y = CharacterPositions [currLevel] [i] [1];
			SpawnPoint.z = CharacterPositions [currLevel] [i] [2];
			Instantiate (Resources.Load ("Character"), SpawnPoint, Quaternion.identity);
			GameObject character = GameObject.Find ("Character(Clone)").gameObject;
			character.name = "Character" + id;
			//renderer = character.GetComponentsInChildren  <SpriteRenderer> ();
			//renderer[0].color = new Color (CharacterColours [i] [0], CharacterColours [i] [1], CharacterColours [i] [2], 1f);
		}
	}
}
