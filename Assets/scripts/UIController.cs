using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
	float timeLeft;
	public Text timerText;
	public Text gameOverText;
	public Text[] playerStackTexts = new Text[2];
	bool gameStopped = false;
	CharacterBehaviourScript[] characterScripts = new CharacterBehaviourScript[2];

	
	public delegate void stopGameFunction();
	public static event  stopGameFunction stopGameEvent;
	public delegate void resumeGameFunction();
	public static event  resumeGameFunction resumeGameEvent;
	
	void OnEnable (){
		timeLeft = 60f;
		gameStopped = false;
		//UIController should be enabled after the level is loaded and characters initiated, through script OnLevelChange()::initiateCharacters();
		characterScripts[0] = GameObject.Find("Character1").GetComponent<CharacterBehaviourScript> ();
		characterScripts[1] = GameObject.Find("Character2").GetComponent<CharacterBehaviourScript> ();

		//subscribing this function to the endGameEvent emitted by UIController class
		//the structure of the function (no puts, no outputs), is defined by the UIControler class.
		UIController.stopGameEvent += stopGameForUIController; 
		UIController.resumeGameEvent += resumeGameForUIController; 

	}

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate(){
		//if (Screen.width != 1024) {
		//	Screen.SetResolution(1024, 768, false);
			// set to true if you want a fullscreen game
		//}
		if (!gameStopped) {
			timeLeft -= Time.deltaTime;
			timerText.text = "Time Left : \n" + Mathf.RoundToInt (timeLeft);
			int[] stackSizes = new int[2];
			int[] nCollectables = new int[2];
			for (int i=0; i<2; ++i) {
				stackSizes [i] = characterScripts [i].getSizeOfStack ();
				nCollectables [i] = characterScripts [i].getCollectableNumber ();
				int score = stackSizes [i] * nCollectables [i];
				playerStackTexts [i].text = "Player " + (i+1) + " : \n" + stackSizes [i] + " X " + nCollectables [i] + " \n" + score;
				if (stackSizes [i] < 2) {
					playerStackTexts [i].fontSize = 20;
				} else if (stackSizes [i] < 4) {
					playerStackTexts [i].fontSize = 30;
				} else if (stackSizes [i] > 4) {
					playerStackTexts [i].fontSize = 40;
				}
			}
		}
		bool limitedTimeMode = false;
		if (timeLeft <= 0 && limitedTimeMode) {
			if (stopGameEvent != null){ //this checks if there are any subscribers to this event, if there are none, it is not emitted
				stopGameEvent();
			}
		}

		//int stack2 = characterScript2.
		//player1Stack.text = "Player 1 stack size: \n" + stack1;
		//player2Stack.text = "Player 2 stack size: \n" + stack2;
		//if (stack1 < 2) {
		//	player1Stack.fontSize = 11;
		//} else if (stack1 < 4) {
	//		player1Stack.fontSize = 14;

	//	} else {
	//		player1Stack.fontSize = 16;
	//	}
	}
	void stopGameForUIController(){
		gameOverText.gameObject.SetActive (true);
		gameStopped = true;
	}

	void resumeGameForUIController (){
		gameOverText.gameObject.SetActive (false);
		gameStopped = false;

	}

	public void addTimeToCurrentLevel(){
		timeLeft += 10f;
		resumeGameEvent();
	}
	public void goBackToMainMenu(){
		Application.LoadLevel(1); //level 0 is game launcher, level 1 is main menu
	}

}
