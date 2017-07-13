
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleLineAdjuster : MonoBehaviour {

	private Vector3 currentPathStart;
	private Vector3 currentPathEnd;

	[SerializeField]
	private GameObject startNode;
	[SerializeField]
	private GameObject endNode;

	public Camera puzzleCamera;

	private Vector3 puzzleOffset;

	[SerializeField]
	private LineRenderer lineRen;

	// Use this for initialization
	void Start () {
		puzzleOffset = new Vector3(3000f, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {

		if (startNode.activeInHierarchy && endNode.activeInHierarchy) {
			Vector3[] lineRendPositions = new Vector3[2];
			lineRendPositions [0] = startNode.transform.position;
			lineRendPositions [1] = endNode.transform.position;

			// Do color calculation

			lineRen.startColor = CalculateColor ();
			lineRen.endColor = lineRen.startColor;
			lineRen.SetPositions (lineRendPositions);
		}

	}
		
	public void SetupLine(Vector3 pathStart, Vector3 pathEnd, Vector3 swipeStart, Vector3 swipeEnd) {

		// Remember to use the puzzle array offset of 3000f, so the nodes get placed in the second array
		currentPathStart = pathStart + puzzleOffset;
		currentPathEnd = pathEnd + puzzleOffset;

		// Move the start and end nodes to the swipe positions, estimated in world coordinates
		Vector3 pathCenterPos = (currentPathEnd + currentPathStart) / 2f;

		// Also need to apply offset to the swipe endpoints, since they correspond to the world coordinates in the origninal array
		swipeStart += puzzleOffset;
		swipeEnd += puzzleOffset;

		// Now move the two nodes to these positions to begin the adjustment game
		startNode.transform.position = swipeStart;
		endNode.transform.position = swipeEnd;

		EnableLine ();

	}

	public void DisableLine() {
		startNode.SetActive (false);
		endNode.SetActive (false);
		lineRen.SetPositions (new Vector3[2]);

	}

	private void EnableLine() {
		startNode.SetActive (true);
		endNode.SetActive (true);

		// Don't worry about setting positions, the Update function above will update the lineRenderer every frame
	}

	private Color CalculateColor() {

		Vector3 lineDirection = endNode.transform.position - startNode.transform.position;
		lineDirection = lineDirection.normalized;
		Vector3 actualDirection = currentPathEnd - currentPathStart;
		actualDirection = actualDirection.normalized;

		// Should be between 0 and 1
		float angleCheck = Vector3.Dot (lineDirection, actualDirection);
		angleCheck = Mathf.Abs (angleCheck);

		if (angleCheck > 1f) {
			Debug.LogError ("AngleCheck was over 1");
		}
		// distance check - check the distance between the centerpoint of the lineDirection and the closest point to it on the actualDirection line
		Vector3 lineCenterpoint = (endNode.transform.position + startNode.transform.position) / 2f;
		Vector3 offsetDirection = lineCenterpoint - currentPathStart;


		float angle = Vector3.Angle (actualDirection, offsetDirection);
		float magnitude = Mathf.Cos (angle) * Vector3.Magnitude (offsetDirection);

		Vector3 closestPoint = currentPathStart + magnitude * actualDirection;

		float distanceCheck = Vector3.Distance (closestPoint, lineCenterpoint) / 100f;
		if (distanceCheck > 1f) {
			distanceCheck = 1f;
		}

		distanceCheck = 1f - distanceCheck;

		// We have values between 0 and 1 for both angle and distance, now find a weighted average to be used for interpolation
		// I weight distance slightly above the angle, since we need to ensure the position is good
		float interpolationValue = 0.5f * distanceCheck + 0.5f * angleCheck;

		// Now get the color
		Color newColor = Color.Lerp(Color.red, Color.green, interpolationValue);


		return newColor;
	}
}
