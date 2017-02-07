using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the movement of all the Doms within the stretchable array
/// </summary>
public class StretchArrayManager : MonoBehaviour {

	// The 8 cornerpoints of the array
	private List<Transform> points = new List<Transform>();

	// List of DOMS within the array
	private List<Transform> doms = new List<Transform> ();

	// Reference to the 4 points used for alignment in the 3 dimensions
	[SerializeField]
	private Transform lowerPoint;
	[SerializeField]
	private Transform upperX;
	[SerializeField]
	private Transform upperY;
	[SerializeField]
	private Transform upperZ;

	// Initialize the two above lists of Transforms
	void Start() {

		// Find all of the yellow points
		GameObject[] pointObjs = GameObject.FindGameObjectsWithTag("Point");
		// Sanity check - see if we found them all
		if (pointObjs.Length != 8) {
			Debug.LogError ("Couldn't find 8 points in the array, found " + pointObjs.Length);
		}
		// Extract the transforms from each
		foreach (GameObject curr in pointObjs) {
			points.Add (curr.transform);
		}

		//-------------------------------------------------------------------------------------
		// Find all of the gameobjects with the "DOM" tag
		GameObject[] gameDoms = GameObject.FindGameObjectsWithTag ("DOM");
		// Sanity check - see if we found them all
		if (gameDoms.Length != 125) {
			Debug.LogError ("Size incorrect, found " + gameDoms.Length + " doms");
		}
		// Extract the transform from each
		foreach (GameObject curr in gameDoms) {
			doms.Add (curr.transform);
		}
	}

	// Update the positions of all individual doms in the array based on the yellow point positions
	public void AlignAllDoms() {
		foreach (Transform curr in doms) {
			curr.GetComponent<AlignDom> ().setPosition (upperX.localPosition.x, upperY.localPosition.y, upperZ.localPosition.z, lowerPoint.localPosition);
		}
	}

	// Get the centerpoint of the array, used for camera look direction
	public Vector3 getArrayCenterpoint() {
		Vector3 averages = new Vector3 (0, 0, 0);
		averages.x = (upperX.localPosition.x + lowerPoint.localPosition.x) / 2;
		averages.y = (upperY.localPosition.y + lowerPoint.localPosition.y) / 2;
		averages.z = (upperZ.localPosition.z + lowerPoint.localPosition.z) / 2;
		return this.transform.TransformPoint(averages);
	}
}
