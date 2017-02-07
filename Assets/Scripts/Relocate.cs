using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relocate : MonoBehaviour {

	//TODO: Setup sync vars
	[SerializeField]
	private Transform[] xSyncs = new Transform[3];
	[SerializeField]
	private Transform[] ySyncs = new Transform[3];
	[SerializeField]
	private Transform[] zSyncs = new Transform[3];

	[SerializeField]
	private Transform parentTransform;

	private Vector3 oldPos = new Vector3 (0, 0, 0);
	private Vector3 newPos = new Vector3 (0, 0, 0);
	private float sphDist = 0f;
	private StretchArrayManager str;

	void Start() {
		str = this.transform.parent.GetComponent<StretchArrayManager> ();
		if (str == null) {
			Debug.LogError ("Couldn't find StretchArrayManager");
		}

		oldPos = this.transform.position;
	}

	public void setNewPosition(Vector3 newPos) {
		// First find which coordinates to move, then change them
		Vector3 nextPos = this.transform.position;

		// Copy over x
		if ((oldPos.x - this.transform.position.x) < 0.0001) {
			
			nextPos.x = newPos.x;
		}

		// Copy over y
		if ((oldPos.y - this.transform.position.y) < 0.0001) {
			
			nextPos.y = newPos.y;
		}

		// Copy over z
		if ((oldPos.z - this.transform.position.z) < 0.0001) {
			nextPos.z = newPos.z;
		}

		this.transform.position = nextPos;
	}

	void OnMouseDrag() {

		// Get the hit distance using raycast
		// Get Raycast, check if we clicked a point

			
		Ray InputLocation = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit colliderHit;
		if (Physics.Raycast(InputLocation, out colliderHit)) {
			if (colliderHit.transform.gameObject.tag != "Point") {
				return;
			}
			sphDist = Vector3.Distance (colliderHit.transform.position, Camera.main.transform.position);

		}



		// Have the transform, now need to move it
		Vector3 temp = Input.mousePosition;
		temp.z = Camera.main.WorldToScreenPoint(this.transform.position).z;
		newPos = Camera.main.ScreenToWorldPoint(temp);

		if (newPos != oldPos) {
			this.transform.position = newPos;
			// Update all synced points
			UpdatePositions();
			oldPos = newPos;
		}
	}

	void OnMouseUp() {
		Camera.main.GetComponent<OrbitalCamera> ().UpdateLook ();
	}

	private void UpdatePositions() {

		Vector3 newLocPos = parentTransform.InverseTransformPoint (newPos);

		// Updates the x position on every other point aligned along the x direction with the clicked-on point
		foreach (Transform curr in xSyncs) {
			Vector3 nextLocPos = parentTransform.InverseTransformPoint(curr.position);
			nextLocPos.x = newLocPos.x;
			curr.position = parentTransform.TransformPoint(nextLocPos);
		}

		// Updates the y position on every other point aligned along the y direction with the clicked-on point
		foreach (Transform curr in ySyncs) {
			Vector3 nextLocPos = parentTransform.InverseTransformPoint(curr.position);
			nextLocPos.y = newLocPos.y;
			curr.position = parentTransform.TransformPoint(nextLocPos);
		}

		// Updates the z position on every other point aligned along the z direction with the clicked-on point
		foreach (Transform curr in zSyncs) {
			Vector3 nextLocPos = parentTransform.InverseTransformPoint(curr.position);
			nextLocPos.z = newLocPos.z;
			curr.position = parentTransform.TransformPoint(nextLocPos);
		}

		// Tell the Manager to update all DOM positions since the cornerpoints moved
		str.AlignAllDoms();
	}

}
