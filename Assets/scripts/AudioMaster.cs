using UnityEngine;
using System.Collections;

public class AudioMaster : MonoBehaviour {
	uint bankId = 0;

	// Use this for initialization
	protected void loadBank () {
		AkSoundEngine.LoadBank ("pancake_co_sb", AkSoundEngine.AK_DEFAULT_POOL_ID, out bankId);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void playEvent(string eventName){
		AkSoundEngine.PostEvent (eventName, gameObject);
	}

	public void stopEvent(string eventName){
		uint eventId = AkSoundEngine.GetIDFromString (eventName);
		AkSoundEngine.ExecuteActionOnEvent (eventId, AkActionOnEventType.AkActionOnEventType_Stop, gameObject, 0, AkCurveInterpolation.AkCurveInterpolation_Sine);
	}

	public void pauseEvent(string eventName){
		uint eventId = AkSoundEngine.GetIDFromString (eventName);
		AkSoundEngine.ExecuteActionOnEvent (eventId, AkActionOnEventType.AkActionOnEventType_Pause, gameObject, 0, AkCurveInterpolation.AkCurveInterpolation_Sine);
	}

	public void resumeEvent(string eventName){
		uint eventId = AkSoundEngine.GetIDFromString (eventName);
		AkSoundEngine.ExecuteActionOnEvent (eventId, AkActionOnEventType.AkActionOnEventType_Resume, gameObject, 0, AkCurveInterpolation.AkCurveInterpolation_Sine);
	}
	//AkSoundEngine.AkState
}
