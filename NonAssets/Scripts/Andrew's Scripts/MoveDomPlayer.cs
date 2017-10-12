using UnityEngine;
using System.Collections;

public class MoveDomPlayer : MonoBehaviour {

	private Camera mainCam;
	// Use this for initialization
	void Start () {
		mainCam = Camera.main;
		if (mainCam == null) {
			Debug.LogError ("No camera tagged 'Main Camera' in scene");
		}
	}
	
	// Update is called once per frame
	// BUGS: When this script is re-enabled by DomSpotManager, update is never called again.....
	void Update () {
		if (Input.GetButtonDown("Fire1")) {
			Ray InputLocation = mainCam.ScreenPointToRay (Input.mousePosition);
			RaycastHit colliderHit;
			if (Physics.Raycast(InputLocation, out colliderHit)) {
				// Move NavMesh Agent to the destination
				GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>().SetTarget(colliderHit.point);
			}
		}

	}
}
