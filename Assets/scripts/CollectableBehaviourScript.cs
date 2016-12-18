using UnityEngine;
using System.Collections;

public class CollectableBehaviourScript : MonoBehaviour {
	public SpriteRenderer ribbonRenderer;
	int ownerID;
	int collectableValue = 1;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void setMarkerColour(){
		float [] colour = new float[3];
		colour [0] = 1f;
		colour [1] = 1f;
		colour [2] = 1f;
		if (ownerID == 1) {
			colour [0] = 1f;
			colour [1] = 0.4f;
			colour [2] = 0.4f;
		} else if (ownerID == 2) {
			colour [0] = 0.4f;
			colour [1] = 0.4f;
			colour [2] = 1f;
		} else if (ownerID == 3) {
			colour [0] = 0.7f;
			colour [1] = 1f;
			colour [2] = 0.7f;
		} else if (ownerID == 4) {
			colour [0] = 0.5f;
			colour [1] = 0.5f;
			colour [2] = 0.5f;
		}
		ribbonRenderer.color = new Color (colour[0], colour[1], colour[2], 1f);
	}

	public int getOwnerID(){
		return ownerID;
	}

	public int getCollectableValue(){
		return collectableValue;
	}

	public void collected(){
		Object.Destroy(this.gameObject);
	}

	public void setOwnerId(int id){
		ownerID = id;
		setMarkerColour();
	}

	public void setInitialVel(float vx, float vy){
		GetComponent<Rigidbody2D> ().velocity = new Vector2 (vx, vy);	
	}
}
