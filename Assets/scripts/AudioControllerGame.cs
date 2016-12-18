using UnityEngine;
using System.Collections;

public class AudioControllerGame : AudioMaster {
	//public GameObject gameObjectForMusicPlayer;

	void OnLevelWasLoaded (){
		int currLevel = Application.loadedLevel;
		string desiredState;
		string desiredGameLevel;
		if (currLevel == 1) { //main menu:
			desiredGameLevel = "Game_Music";
			desiredState = "Music_level2";
		} else {
			desiredGameLevel = "Game_Music";
			desiredState = "Music_level1";
		}
		uint stateGroupId = AkSoundEngine.GetIDFromString (desiredGameLevel);
		uint stateId = AkSoundEngine.GetIDFromString (desiredState);
		//Debug.Log ("state group id :" + stateGroupId + "stete id :" + stateId);
		//AkSoundEngine.SetState(stateGroupId,stateId);
		AkSoundEngine.SetState(desiredGameLevel, desiredState);
		//Debug.Log ("currLeveL: "+currLevel+" desiredState: "+desiredState);
	}

	void Start () {
		//soundBankLoaded = false;

	}
	
	// Update is called once per frame
	void Update () {
	}
}
