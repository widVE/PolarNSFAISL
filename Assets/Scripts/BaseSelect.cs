using UnityEngine;
using System.Collections;

public class BaseSelect : MonoBehaviour {

	private enum state {atBase, atReset, translatingToBase, translatingToReset, rotatingToBase, rotatingToReset};
	private Camera mainCam;
	private Vector3 originalCamPosition;
	private Quaternion originalCamRotation, rotationDirection;
	private GameObject objectHit;
	private Vector3 translationVector, rotationVector;
	private Vector3 destination;
	private state currState;
	private float rotationSpeed = 1f;

	private GameObject baseCanvas;

	// Use this for initialization
	void Start () {
		mainCam = this.gameObject.GetComponent<Camera> ();
		if (mainCam == null) {
			Debug.LogError("Scene Camera not found - Is this attached to the Scene Camera?");
		}
		currState = state.atReset;
		objectHit = null;
		originalCamPosition = mainCam.transform.position;
		originalCamRotation = mainCam.transform.rotation;
	}

	void Update () {

		switch (currState) {
		case state.translatingToBase: 
			Debug.Log ("Translating To Base, Distance is: " + Vector3.Distance (mainCam.transform.position, objectHit.transform.position));
			if (Vector3.Distance (mainCam.transform.position, destination) < 50) {
				//translationVector = objectHit.transform.position - mainCam.transform.position;
				//rotationDirection = Quaternion.LookRotation(translationVector);
				//currState = state.rotatingToBase;
				currState = state.atBase;
				baseCanvas.SetActive (true);
			} else {
				mainCam.transform.Translate (translationVector * Time.deltaTime, Space.World);
				mainCam.transform.rotation = Quaternion.Slerp (mainCam.transform.rotation, rotationDirection, rotationSpeed * Time.deltaTime);
			}
			break;

		case state.translatingToReset:
			Debug.Log ("Translating To Reset, Distance is: " + Vector3.Distance (mainCam.transform.position, originalCamPosition));
			if (Vector3.Distance (mainCam.transform.position, originalCamPosition) < 100) {
				currState = state.atReset;
			} else {
				mainCam.transform.Translate (translationVector * Time.deltaTime, Space.World);
				mainCam.transform.rotation = Quaternion.Slerp (mainCam.transform.rotation, originalCamRotation, 3f * rotationSpeed * Time.deltaTime);
			}
			break;

		case state.atReset:
			//Debug.Log ("At Reset");
			if (Input.GetButtonDown("Fire1")) {
				Debug.Log ("Mouse Clicked");
				Ray InputLocation = mainCam.ScreenPointToRay (Input.mousePosition);
				RaycastHit colliderHit;
				if (Physics.Raycast(InputLocation, out colliderHit)) {
					objectHit = colliderHit.transform.gameObject;
					if (objectHit.name == "Terrain") {
						objectHit = null;
						return;
					}
					Debug.Log ("Raycast hit object: " + objectHit.name);

					baseCanvas = objectHit.transform.GetChild (0).gameObject;
					destination = objectHit.transform.position - Vector3.forward * 80f;
					translationVector = destination - mainCam.transform.position;
					rotationVector = objectHit.transform.position - destination;
					rotationDirection = Quaternion.LookRotation (rotationVector);
					//mainCam.transform.Translate (translationVector * Time.deltaTime, Space.World);
					//mainCam.transform.rotation = Quaternion.Slerp (mainCam.transform.rotation, rotationDirection, rotationSpeed * Time.deltaTime);
					currState = state.translatingToBase;
				}
			}

			break;
		case state.atBase:
			Debug.Log ("At Base");
			if (Input.GetButtonDown("Fire2")) {
				translationVector = originalCamPosition - mainCam.transform.position;
				currState = state.translatingToReset;
				baseCanvas.SetActive (false);
			}
			break;

		default:
			Debug.LogError("You shouldn't be here...");
			break;
		}
	}
}
