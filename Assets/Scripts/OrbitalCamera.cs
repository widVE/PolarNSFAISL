using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalCamera : MonoBehaviour {

	private StretchArrayManager str;
	private float distance;
	private Vector3 centerPointPos = new Vector3 (0, 0, 0);
	[SerializeField]
	private float sensitivity = 0.5f;
	void Start() {
		str = GameObject.Find("DOMArray").GetComponent<StretchArrayManager> ();
		if (str == null) {
			Debug.Log ("Couldn't find the StretchArrayProto Gameobject");
		}
		centerPointPos = str.getArrayCenterpoint ();
		distance = Vector3.Distance (this.transform.position, centerPointPos);
	}
		
	// Checks for user camera input and updates accordingly
	void Update () {

		// Update the centerpoint in case the array moved
		centerPointPos = str.getArrayCenterpoint ();

		// If the user right-clicks, track mouse movement and pan camera
		if (Input.GetMouseButton(1)) {

			float xDistance = Input.GetAxis ("Mouse X");
			float yDistance = Input.GetAxis ("Mouse Y");

			// Move camera
			Vector3 nextPos = this.transform.position;
			nextPos -= (this.transform.up * yDistance) * distance * sensitivity;
			nextPos -= (this.transform.right * xDistance) * distance * sensitivity;
			nextPos -= this.transform.forward * (distance - Vector3.Distance(centerPointPos, this.transform.position));
			this.transform.localPosition = nextPos;

			// Rotate camera to look at the array centerpoint
			this.transform.rotation = Quaternion.LookRotation (centerPointPos - this.transform.localPosition);
		} 



		// If the user scrolls the mouse wheel change the level of zoom by changing the distance
		// between the camera and array centerpoint
		if (Input.GetAxis("Mouse ScrollWheel") != 0f) {
			this.transform.position += this.transform.forward * Input.GetAxis ("Mouse ScrollWheel") * distance;
			distance -= (Input.GetAxis ("Mouse ScrollWheel") * distance);
			if (distance <= 1) {
				distance = 1f;
			}
		}
	}

	public void UpdateLook() {
		// Rotate camera to look at the array centerpoint
		centerPointPos = str.getArrayCenterpoint ();
		this.transform.rotation = Quaternion.LookRotation (centerPointPos - this.transform.localPosition);
	}
}

