
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to manage the adjustable line used in the puzzle array/camera setup
/// Players drag the endpoints of the line to match the neutrino path, to which it
/// changes color based on accuracy
/// </summary>
public class PuzzleLineAdjuster : MonoBehaviour {

	// ----------VARIABLES----------

	// The start position of the neutrino path of the current displayed event
	private Vector3 currentPathStart;

	// The end position of the neutrino path of the current displayed event
	private Vector3 currentPathEnd;

	// The sphere game object at the start of the line
	[SerializeField]
	private GameObject startNode;

	// The sphere game object at the end of the line
	[SerializeField]
	private GameObject endNode;

	// The camera viewing the puzzle array
	public Camera puzzleCamera;

	// The 3D offset that the puzzle array sit relative to the main array
	private Vector3 puzzleOffset;

	// Line renderer comprising this adjustable line
	[SerializeField]
	private LineRenderer lineRen;

	/// <summary>
	/// Start - used to initialize variables 
	/// </summary>
	void Start () {
		puzzleOffset = new Vector3(3000f, 0, 0);
	}
	
	/// <summary>
	/// Update - used to update the line color
	/// Movement of the end nodes is handled in the DragRecognizer class itself 
	/// </summary>
	void Update () {

		// If both nodes are active (i.e. we are viewing an event), then render the line with the
		// correct color
		if (startNode.activeInHierarchy && endNode.activeInHierarchy) {

			// Get the positions
			Vector3[] lineRendPositions = new Vector3[2];
			lineRendPositions [0] = startNode.transform.position;
			lineRendPositions [1] = endNode.transform.position;

			// Do color calculation
			lineRen.startColor = CalculateColor ();
			lineRen.endColor = lineRen.startColor;

			// Update line positions
			lineRen.SetPositions (lineRendPositions);
		}

	}
		
	/// <summary>
	/// Function to initialize the line when a new event is displayed
	/// </summary>
	/// <param name="pathStart">Start of the neutrino path of this event</param>
	/// <param name="pathEnd">End of the neutrino path of this event</param>
	/// <param name="swipeStart">Start of the player's swipe used to capture this event, in world coordinates</param>
	/// <param name="swipeEnd">End of the player's swipe used to capture this event, in world coordinates</param>
	public void SetupLine(Vector3 pathStart, Vector3 pathEnd, Vector3 swipeStart, Vector3 swipeEnd) {

		// Remember to apply the puzzle array offset, so the nodes get placed in the second array, not in the main array
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

		// Render the line
		EnableLine ();

	}

	/// <summary>
	/// Disables the line endpoints and line renderer
	/// </summary>
	public void DisableLine() {
		startNode.SetActive (false);
		endNode.SetActive (false);
		lineRen.SetPositions (new Vector3[2]);

	}

	/// <summary>
	/// Enables the line endpoints and line renderer
	/// </summary>
	private void EnableLine() {
		startNode.SetActive (true);
		endNode.SetActive (true);

		// Don't worry about setting line positions here, as the Update function above will update the lineRenderer every frame already
	}

	/// <summary>
	/// Calculates what color the line should be based on the position and angle of the adjustable line relative
	/// to the actual neutrino path
	/// </summary>
	/// <returns>The color the line should be set to</returns>
	private Color CalculateColor() {

		// Direction adjustable line is currently pointing
		Vector3 lineDirection = endNode.transform.position - startNode.transform.position;
		// normalize it for linear algebra computation
		lineDirection = lineDirection.normalized;

		// Actual direction the path is moving
		// TODO: Note that the startEnd of this path is set waaaaay back in swipeRecognizer, and they are set backwards
		// Since my computation is indifferent to actual direction (only checks how parallel they are), it doesn't really matter
		// But if things look unclear later, it's because currentPathEnd is really the start and vice versa
		Vector3 actualDirection = currentPathEnd - currentPathStart;
		// Again normalize for computation
		actualDirection = actualDirection.normalized;

		// Use the dot product to check how parallel they are
		float angleCheck = Vector3.Dot (lineDirection, actualDirection);

		// take absolute value to ignore backwards direction angles
		angleCheck = Mathf.Abs (angleCheck);

		// Since the magnitudes are normalized, the value of anglecheck should be between 0 and 1, so this is just a sanity check
		if (angleCheck > 1f) {
			Debug.LogError ("AngleCheck was over 1");
		}


		// distance check - check the distance between the centerpoint of the lineDirection and the closest point to it on the actualDirection line
		// (i.e., find the magnitude of the line that is perpendicular to actualDirection that ends at the lineCenterpoint)
		Vector3 lineCenterpoint = (endNode.transform.position + startNode.transform.position) / 2f;
		Vector3 offsetDirection = lineCenterpoint - currentPathStart;

		// Using some trigonometry for this
		float angle = Vector3.Angle (actualDirection, offsetDirection);
		float magnitude = Mathf.Cos (angle) * Vector3.Magnitude (offsetDirection);
		Vector3 closestPoint = currentPathStart + magnitude * actualDirection;
		float distanceCheck = Vector3.Distance (closestPoint, lineCenterpoint) / 50f;

		// We divide by 100 to signify that if the distance is greater than 100, we just clamp the value to 1
		// Other wise we get a value between 0 and 1
		if (distanceCheck > 1f) {
			distanceCheck = 1f;
		}

		// We want 0 to represent bad, 1 for good, so we flip it here
		distanceCheck = 1f - distanceCheck;

		// We have values between 0 and 1 for both angle and distance, now find a weighted average to be used for interpolation
		// Currently we evenly weight the two values
		float interpolationValue = 0.5f * distanceCheck + 0.5f * angleCheck;

		// Now get the color and return it
		Color newColor = Color.Lerp(Color.red, Color.green, interpolationValue);

		return newColor;
	}
}
