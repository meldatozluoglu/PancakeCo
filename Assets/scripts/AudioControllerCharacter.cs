using UnityEngine;
using System.Collections;

public class AudioControllerCharacter : AudioMaster {
	//float MoveSoundFireRate= 0.25f;
	//float nextMoveSoundFire;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		CharacterBehaviourScript characterScript = gameObject.GetComponent<CharacterBehaviourScript> ();

		bool wasWalking = characterScript.wasWalking ();
		bool isWalking = characterScript.isWalking ();
		if (isWalking && !wasWalking ) {//Time.time > nextMoveSoundFire) {
			//playEvent ("Play_Walk_1");
			playEvent ("Walk_player1");
			//nextMoveSoundFire = Time.time + MoveSoundFireRate;
		} else if (!isWalking) {
			//stopEvent ("Play_Walk_1");
			stopEvent ("Walk_player1");
		}
		bool jumped = characterScript.jumped();
		if (jumped) {
			playEvent ("Jump_player1");
			characterScript.toggleJumpedThisStep();
		}
		bool wasHit = characterScript.wasHit();
		if (wasHit) {
			playEvent ("LowHit_player1");
			characterScript.toggleWasHit();
		}
	}
}
