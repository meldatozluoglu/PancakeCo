using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameLauncherScript : MonoBehaviour {
	public GameObject loadingImage;
	public Text selectedLevelText;
	public GameObject gameObjectForMusicPlayer;
	bool soundBankLoaded;

	int mainMenuLevelId = 1;
	int selectedLevel;
	int numberOfLevels = 3;
	string[] levelNames = new string[2];

	private float logoScreenDisplayTime = 1f;
	private float startTime;
	private bool gameLoaded = false;
	/*IEnumerator Wait(){
		waitActive = true;
		//Debug.Log ("inside wait");
		yield return new WaitForSeconds (3.0f);
		waitedAlready = true;
		waitActive = false;
	}*/

	void Start () {
		int levelOffset = 2; // 1 for game luancher scene, 1 for main menu scene
		levelNames [0] = "Back\nGarden";
		levelNames [1] = "GrandMa's\nkitchen";
		selectedLevel = 2; //level 1 is game launcher, level 2 is main menu, consecutive levels are the following game levels
		selectedLevelText.text = levelNames [selectedLevel - levelOffset]; //the offset is 2
		startTime = Time.time;
		//if (!waitActive && !waitedAlready) {
		//	StartCoroutine (Wait ());  
		//}
		//if (waitedAlready) {
		//	StopCoroutine ( Wait() );
		//}
	}

	void FixedUpdate(){
		if (!gameLoaded && Time.time > startTime + logoScreenDisplayTime) {
			gameLoaded = true;
			LoadLevel (mainMenuLevelId); 
		}
	}

	public void IncrementLevelSelector(int increment){
		selectedLevel += increment;
		if (selectedLevel < 2) {
			selectedLevel = numberOfLevels;
		}
		if (selectedLevel > numberOfLevels) {
			selectedLevel = 2;		
		}
		Debug.Log ("increment was: "+increment+" selected level is "+selectedLevel);
		Debug.Log ("levelNames[0]: " + levelNames [0] + " levelNames [1]: " + levelNames [1]);
		selectedLevelText.text = levelNames [selectedLevel-2];
		Debug.Log ("currName: " + levelNames [selectedLevel-2] + " from text: " + selectedLevelText.text);
	}

	void LoadLevel(int level){
		loadingImage.SetActive (true);
		Application.LoadLevel(level);
	}

	public void LoadLevel(){
		loadingImage.SetActive (true);
		Application.LoadLevel(selectedLevel);

	}

	public void quitGame(){
		Application.Quit ();
	}
}
