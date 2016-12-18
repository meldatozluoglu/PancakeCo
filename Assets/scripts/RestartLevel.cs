using UnityEngine;
using System.Collections;

public class RestartLevel : MonoBehaviour {

	public void restartLevel(){
		Application.LoadLevel (Application.loadedLevel);
	}

}
