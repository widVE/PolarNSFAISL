using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class used to control the puzzle (secondary) camera on the table top
/// This camera views a second array that displays swiped events statically.
/// Various other scripts call MoveCamera to update this camera's view
/// </summary>
public class PuzzleCameraController : MonoBehaviour {

	// ---------- VARIIABLES ----------

	// speed factor the camera translates at
	public float translationSpeed = 2.0f;

	// Line renderer used to draw the path in the secondary array, used for debugging (feature later?)
	private LineRenderer linRen;

	// What the puzzle camera is currently fixated on - for events, it's the centerpoint of the event's path
	private Vector3 currentTarget;

	// flag used to tell when the camera is translating, to avoid timing/sync issues
	private bool isMoving = false;

	// When translating, this is the position the camera is translating to
	private Vector3 destPos;

	// The direction to face once the camera reaches the destination
	// Note that the camera rotates to facingDir while translating to destPos
	private Quaternion facingDir;

	// Individual UI elements extracted from the UICollection object
	// Slider to adjust dom scale
	public Slider sizeSlider;
	// Slider to rotate the camera horizontally around the currentTarget
	public Slider rotateHorizontalSlider;
	// Slider to rotate the camera vertically around the currentTarget
	public Slider rotateVerticalSlider;
	// Slider to adjust camera's distance from currentTarget
	public Slider zoomSlider;
	// Toggle to turn on/off the line renderer for the currentPath
	public Toggle showLineToggle;

	// For use with the rotateHorizontalSlider, to remember our last-used degree value
	private float currHorizontalDegree = 0f;
	// For use with the rotateVerticalSlider, to remember our last-used degree value
	private float currVerticalDegree = 0f;

	// Offset where the puzzle array sits relative to the main array - X AXIS
	private Vector3 puzzleArrayOffset = new Vector3(3000f, 0, 0);

	// The default target the camera should sit at if there is no event being viewed
	private Vector3 defaultCameraTarget = new Vector3 (3000, -1000, -2000);

	// The default distance the camera should be placed from the view target
	private Vector3 defaultCameraViewDistance = new Vector3 (0, 0, 2000f);


	// The EventInfo object associated with the current event
	// NOTE: The EventInfo object stores a DomState list, which is essentially a list of doms (and their states) that are activated by this event
	//       This structure is PASS BY REFERENCE, so if an event is swiped in the middle of playing, this list is also updated
	//       as the event continues playing! So it is necessary to update the puzzle array doms constantly to ensure we view the final
	//       event result, not just the state the array was in when we captured it
	private EventInfo currentEventInfo = null;

	// Enumeration used for snap locations
	public enum SnapPosition {Top, Side, Front};

	// Current Snap setting of the camera
	private SnapPosition currentSnapPosition;

	// SnapToggleManager used to control the 4 toggles for camera snapping
	[SerializeField]
	SnapToggleManager snapToggleMan;

	// The adjustable line manager so we can move our adjustable line
	[SerializeField]
	private PuzzleLineAdjuster puzzleLineAdjuster;

	// ----------END VARIABLES----------

	/// <summary>
	/// Start - used to setup references to UI and other constant values
	/// </summary>
	void Start() {
		linRen = this.gameObject.GetComponent<LineRenderer> ();
		facingDir = Quaternion.LookRotation(new Vector3(0,0,1f));

		// Set the snap position to default front position (camera facing positive z direction)
		currentSnapPosition = SnapPosition.Front;
	}


	/// <summary>
	/// Update - used to adjust camera when moving, and to update camera with UI settings
	/// </summary>
	void Update () {

		SnapPosition newSnapPosition = snapToggleMan.GetSnapToggleSetting ();
		if (!newSnapPosition.Equals(currentSnapPosition)) {
			currentSnapPosition = newSnapPosition;
			if (currentEventInfo != null) {
				isMoving = true;
				ResetSliders ();
			}
		}

		// Update destPos based on the currentSnapPosition
		switch (currentSnapPosition) {
		case SnapPosition.Top:
			facingDir = Quaternion.LookRotation (new Vector3 (0f, -1f, 0f), new Vector3 (1, 0, 0));
			destPos = currentTarget + (new Vector3 (0f, 1000f, 0f));
			break;
		case SnapPosition.Side:
			facingDir = Quaternion.LookRotation (new Vector3 (1f, 0f, 0f), new Vector3 (0, 1f, 0));
			destPos = currentTarget - (new Vector3 (1000f, 0f, 0f));
			break;
		case SnapPosition.Front:
			facingDir = Quaternion.LookRotation (new Vector3 (0f, 0f, 1f), new Vector3 (0, 1, 0));
			destPos = currentTarget - (new Vector3 (0f, 0f, 1000f));
			break;
		default:
			break;
		}


		// If the camera is currently moving, check to see if we should keep moving
		if (isMoving) {
			// If we're close enough to the target, stop
			if (Vector3.Distance(this.transform.position, destPos) > 10f) {
				Vector3 translationVector = destPos - this.transform.position;
				this.transform.Translate (translationVector * Time.deltaTime * translationSpeed, Space.World);
				this.transform.rotation = Quaternion.Slerp (this.transform.rotation, facingDir, Time.deltaTime);
			} else {
				isMoving = false;
			}
		} 

		// If we are currently viewing an event, update which doms are turned on
		if (currentEventInfo != null) {
			foreach (VisualizeEvent.DomState curr in currentEventInfo.getDomStates()) {
				// If there is a dom that is in our DomStates list that is not on, turn it on
				// See the above NOTE for an explaination on why this happens
				if (!curr.Dom.GetComponent<DOMController>().on) {
					curr.Dom.GetComponent<DOMController> ().TurnOn (curr.timeFrac, curr.charge);
				}
			}

			// If we are viewing an event AND the camera isn't moving AND the SnapPosition is custom, then apply the UI adjustments
			// This is to avoid relative movements/rotations from becoming mixed up and chaotic
			if (!isMoving) {
				ApplyUIValues ();
			}

			// If we are viewing an event and the toggle to show the line is on, then draw the line
			if (showLineToggle.isOn) {
				Vector3[] pathPositions = new Vector3[2];
				pathPositions [0] = currentEventInfo.getNeutrinoPathStart () + puzzleArrayOffset;
				pathPositions [1] = currentEventInfo.getNeutrinoPathEnd () + puzzleArrayOffset;
				linRen.SetPositions (pathPositions);
			} else {
				// else clear the line renderer
				linRen.SetPositions (new Vector3[2]);
			}
		}
	}

	/// <summary>
	/// Function to move the camera to view an event in the puzzle array
	/// </summary>
	/// <param name="newEventInfo"> The EventInfo object representing the event to be viewed. If newEventInfo is null, then the camera is moved to the default location (viewing the whole array)</param>
	public void MoveCamera(EventInfo newEventInfo) {

		// Reset the array back to "blank" state by turning off all doms that are currently on (and only if we were currently viewing an event)
		if (currentEventInfo != null) {
			foreach (VisualizeEvent.DomState curr in currentEventInfo.getDomStates()) {
				curr.Dom.GetComponent<DOMController> ().TurnOff ();
			}
		}

		// Reset the slider values, so we switch to the new event with default values
		ResetSliders ();

		// Update our currentEventInfo with the new event data
		currentEventInfo = newEventInfo;

		// If the currentEventInfo is now null, move to default position
		if (currentEventInfo == null) {
			currentTarget = defaultCameraTarget;
			puzzleLineAdjuster.DisableLine ();
		} else {
			// else calculate our current targetPosition...
			currentTarget = CalculatePathCenterPos ();

			// Set the adjustable line in the array based on the swipe's start/end (so you can "fine tune" your line)
			puzzleLineAdjuster.SetupLine(currentEventInfo.getNeutrinoPathStart(), currentEventInfo.getNeutrinoPathEnd(), currentEventInfo.getSwipePathStart(), currentEventInfo.getSwipePathEnd());

			//...and start turing on Doms in the currentEvent's DomState list
			foreach (VisualizeEvent.DomState curr in currentEventInfo.getDomStates()) {
				curr.Dom.GetComponent<DOMController> ().TurnOn (curr.timeFrac, curr.charge);
			}
		}

		// Set the new destination to translate the camera to and tell the camera to start moving
		destPos = (currentTarget - defaultCameraViewDistance);
		isMoving = true;
	}

	/// <summary>
	/// Applies the UI slider/toggle values to the camera, adjusting position, rotation, and dom attributes
	/// </summary>
	private void ApplyUIValues() {

		// Update distance to current target
		float currDistance = Vector3.Distance (this.transform.position, currentTarget);
		float positionOffset = currDistance - zoomSlider.value;
		transform.Translate (this.transform.forward * positionOffset, Space.World);

		// Update camera angle horizontally
		float rotateHorizontalValue = rotateHorizontalSlider.value;
		float rotateHorizontalDiff = rotateHorizontalValue - currHorizontalDegree;
		transform.RotateAround (currentTarget, Vector3.up, rotateHorizontalDiff);
		transform.LookAt (currentTarget);

		// Update camera angle vertically
		float rotateVerticalValue = rotateVerticalSlider.value;
		float rotateVerticalDiff = rotateVerticalValue - currVerticalDegree;
		transform.RotateAround (currentTarget, this.transform.right, rotateVerticalDiff);
		transform.LookAt (currentTarget);

		// Update old values for the next iteration
		currHorizontalDegree = rotateHorizontalValue;
		currVerticalDegree = rotateVerticalValue;
		currDistance = positionOffset;

		// Lastly, update dom scales
		foreach (VisualizeEvent.DomState curr in currentEventInfo.getDomStates()) {
			curr.Dom.transform.localScale = (new Vector3 (1f, 1f, 1f) * sizeSlider.value); 
		}
	}

	/// <summary>
	/// Sets the slider values to default
	/// </summary>
	private void ResetSliders() {
		sizeSlider.value = 1.0f;
		rotateHorizontalSlider.value = 0f;
		rotateVerticalSlider.value = 0f;
		zoomSlider.value = 1000f;
	}

	/// <summary>
	/// Calculates the current event's center position in the puzzle array using the event's start and end points
	/// </summary>
	/// <returns>The event center position.</returns>
	private Vector3 CalculatePathCenterPos() {
		if (currentEventInfo == null) {
			return new Vector3(-1,-1,-1);
		}

		Vector3 vStart = currentEventInfo.getNeutrinoPathStart();
		Vector3 vEnd = currentEventInfo.getNeutrinoPathEnd ();

		Vector3 lookPosition = (vStart + vEnd) / 2f;

		// Be sure to apply the offset!
		return lookPosition + puzzleArrayOffset;

	}
}
