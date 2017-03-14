using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DomSpotManager : MonoBehaviour {

	private GameObject mainCam;
	private Transform playerTransform;
	[SerializeField]
	private GameObject domPrefab;
	[SerializeField]
	private GameObject stringPrefab;

	// Current "GameObject" quad in the scene
	private GameObject currentObject;

	// A reference to the script attached to currentObject
	private DomSpot currSpot;

	// For changing user input based on what they are doing
	private enum State {placingString, placingDom, driving};

	// Current state of the player, either they are driving, placing a string, or
	// placing doms
	private State currState;

	// Float coefficient for string and dom translation speed
	private float moveRate = 2f;

	// Reference to the current DOM we are placing, used to destroy
	// The game object if the player exits the DOM placing state
	private GameObject currentDOM;

	// Use this for initialization
	void Start () {
		playerTransform = this.transform;
		currState = State.driving;
		currentDOM = null;
		//mainCam = Camera.main.gameObject;
		//if (mainCam == null) {
		//	Debug.LogError ("No main Camera in Scene");
		//}

	}
	
	// Update is called once per frame
	// Couldn't modularize this in other methods, since we need to 
	// use Update's Time.deltatime which only works in Update
	// Hence, the messy switch statement
	void Update () {

		switch (currState) {
		case State.driving:
			if (Input.GetKeyDown("space")) {
				// Enter place mode
				disableDriving ();
				if (currSpot.getString() != null) {
					// Quad already has a string, so enter DOM placing mode
					currState = State.placingDom;
				} else {
					// Begin placing a string for DOMS
					currState = State.placingString;
				}
			}
			break;
		case State.placingString:
			
			GameObject currString = currSpot.getString ();
			// Instantiate the string if it hasn't been yet
			if (currString == null) {
				GameObject tempString = (GameObject)Instantiate (stringPrefab);
				tempString.transform.SetParent (currentObject.transform);
				Vector3[] pos = new Vector3[2];
				pos [0] = currentObject.transform.position;
				pos [1] = pos [0] - new Vector3 (0.0f, 1.0f, 0.0f);
				tempString.GetComponent<ICLString> ().SetStringPositions (pos);
				currSpot.setString (tempString);
				currString = tempString;
			} 

			// Control length of the string
			if (UnityEngine.Input.GetKey (UnityEngine.KeyCode.S)) {
				// Move the string down
				float moveAmount = moveRate * Time.deltaTime;
				currString.GetComponent<ICLString> ().UpdateStringDepth (moveAmount);
			} else if (UnityEngine.Input.GetKey (UnityEngine.KeyCode.W)) {
				// Move the string up
				float moveAmount = -1f * moveRate * Time.deltaTime;
				currString.GetComponent<ICLString> ().UpdateStringDepth (moveAmount);
			} else if (UnityEngine.Input.GetKeyDown ("space")) {
				// Leave the string, begin placing DOMS
				currState = State.placingDom;
			}
			break;
		case State.placingDom:
			GameObject curString = currSpot.getString ();
			if (currentDOM == null) {
				currentDOM = curString.GetComponent<ICLString> ().AddDOM (currentObject.transform.position - new Vector3 (0, 1, 0));
			}
			if (UnityEngine.Input.GetKey (UnityEngine.KeyCode.S)) {
				// Move the DOM down
				currentDOM.transform.position -= new Vector3(0, moveRate * Time.deltaTime, 0);
				// Clamp the movement so the DOM doesn't fall off the string!
				if (currentDOM.transform.position.y < -1f * curString.GetComponent<ICLString>().stringDepth + 0.65f) {
					Vector3 newLoc = currentObject.transform.position;
					newLoc.y = -1f * curString.GetComponent<ICLString> ().stringDepth + 0.65f;
					currentDOM.transform.position = newLoc;
				}
			} else if (UnityEngine.Input.GetKey (UnityEngine.KeyCode.W)) {
				// Move the DOM up
				currentDOM.transform.position += new Vector3(0, moveRate * Time.deltaTime, 0);
				if (currentDOM.transform.position.y > currentObject.transform.position.y - 1) {
					Vector3 newLoc = currentObject.transform.position;
					newLoc.y -= 1.0f;
					currentDOM.transform.position = newLoc;
				}
			} else if (UnityEngine.Input.GetKeyDown ("space")) {
				// Place the DOM and repeat
				curString.GetComponent<ICLString> ().addDOMtoList(currentDOM);
				currentDOM = null;
			} else if (UnityEngine.Input.GetKeyDown(KeyCode.Q)) {
				// Exit and return to driving
				Destroy(currentDOM);
				currentDOM = null;
				enableDriving ();
				currState = State.driving;
			}
			break;
		}

	}

	public void SetDomSpot(Transform newSpot) {
		currentObject = newSpot.gameObject;
		currSpot = newSpot.gameObject.GetComponent<DomSpot> ();
	}
		
//	private bool PlaceDom() {
//		// TODO: Resource Check
//		if (false) {
//			return false;
//		}
//		//Intantiate DOM where it should be placed, as a child of the correct quad
//		Vector3 placeLocation = currentObject.transform.position;
//		placeLocation.y = (-2f * currSpot.incrementNumDoms());
//		GameObject addedDom = (GameObject)Instantiate(domPrefab, placeLocation, Quaternion.identity);
//		addedDom.transform.SetParent (currentObject.transform);
//		return true;
//	}

	private void disableDriving() {
		//GetComponent<UnityStandardAssets.Vehicles.Car.CarUserControl> ().enabled = false;
		GetComponent<MoveDomPlayer>().enabled = false;
	}

	private void enableDriving() {
		//GetComponent<UnityStandardAssets.Vehicles.Car.CarUserControl> ().enabled = true;
		GetComponent<MoveDomPlayer>().enabled = true;
		Debug.Log ("Driving Enabled");
	}
//	void OnGUI() {
//		GUI.Label (new Rect (10, 10, 200, 20), "Current: " + currentObject.name);
//		GUI.Label (new Rect (10, 30, 200, 20), "Level: " + (currSpot.getNumDoms ()).ToString());
//		GUI.Label(new Rect(10, 50, 200, 20), "Upgrade Cost: " + (currSpot.getNumDoms() * 250).ToString());
//	}
}
