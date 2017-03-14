using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableOrbit : MonoBehaviour {

	private float distance;
	[SerializeField]
	private float sensitivity = 0.5f;
	[SerializeField]
	private Transform arrayTrans;

	void Start() {
		distance = Vector3.Distance (this.transform.position, arrayTrans.position);
	}

	// Checks for user camera input and updates accordingly
	void Update () {

		// If the user right-clicks, track mouse movement and pan camera
		if (Input.GetMouseButton(1)) {

			float xDistance = Input.GetAxis ("Mouse X");
			float yDistance = Input.GetAxis ("Mouse Y");

			// Move camera
			Vector3 nextPos = this.transform.position;
			nextPos -= (this.transform.up * yDistance) * distance * sensitivity;
			nextPos -= (this.transform.right * xDistance) * distance * sensitivity;
			nextPos -= this.transform.forward * (distance - Vector3.Distance(arrayTrans.position, this.transform.position));
			this.transform.localPosition = nextPos;

			// Rotate camera to look at the array centerpoint
			this.transform.rotation = Quaternion.LookRotation (arrayTrans.position - this.transform.localPosition);
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
		this.transform.rotation = Quaternion.LookRotation (arrayTrans.position - this.transform.localPosition);
	}
}

