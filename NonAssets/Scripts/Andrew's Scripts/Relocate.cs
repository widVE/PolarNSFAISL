using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// -UNUSED CLASS-
/// Was part of the stretch array implementation, was used to be able to drag
/// the corners of a 'stretchable' array around, to expand/reposition it, with
/// all internal doms moving accordingly
/// </summary>
public class Relocate : MonoBehaviour {

	// These arrays are sets of points are alligned with THIS point in world coordinates
	// That is, all transforms in xSyncs are other points that share the same x coordinate with
	// this point
	[SerializeField]
	private Transform[] xSyncs = new Transform[3];
	[SerializeField]
	private Transform[] ySyncs = new Transform[3];
	[SerializeField]
	private Transform[] zSyncs = new Transform[3];

	// Reference to the Stretchable Dom array transform, as points need to be moved in this coordinate system
	[SerializeField]
	private Transform parentTransform;

	// position values used to check if a point has moved
	// (mostly for efficiency, no need to call UpdatePositions every frame)
	private Vector3 oldPos = new Vector3 (0, 0, 0);
	private Vector3 newPos = new Vector3 (0, 0, 0);

	// Distanced from the camera to the point, used to set the z value when dragging a point
	private float sphDist = 0f;

	// Reference to the manager object on the parent transform
	private StretchArrayManager str;


	/// <summary>
	/// Start - set variables
	/// </summary>
	void Start() {
		str = this.transform.parent.GetComponent<StretchArrayManager> ();
		if (str == null) {
			Debug.LogError ("Couldn't find StretchArrayManager");
		}

		oldPos = this.transform.position;
	}

	/// <summary>
	/// Updates the position of this point
	/// </summary>
	/// <param name="newPos">The new position to assign this point to</param>
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

	/// <summary>
	/// Unity function, called every frame that the mouse is clicked down
	/// Used for updated positions if the mouse is clicked on a point
	/// </summary>
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

		// if the position of this point changed, update all other points' positions that should be aligned with this one
		if (newPos != oldPos) {
			this.transform.position = newPos;
			// Update all synced points
			UpdatePositions();
			oldPos = newPos;
		}
	}

	/// <summary>
	/// Unity function - Called when the MouseDrag ends and the mouse button is released
	/// Used to update the camera so it's pointed at the center of the array, since the array
	/// center may move as the cornerpoints move
	/// </summary>
	void OnMouseUp() {
		Camera.main.GetComponent<OrbitalCamera> ().UpdateLook ();
	}

	/// <summary>
	/// Updates all the positions of other points in the array to correspond with this point's new position
	/// That is, this function is called when this point is being dragged, and needs to align all others
	/// </summary>
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
