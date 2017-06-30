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

	[SerializeField]
	// A reference to the UI interfaces to get their values for camera adjustments
	private GameObject UIObjects;

	// Individual UI elements extracted from the UICollection object
	// Slider to adjust dom scale
	private Slider sizeSlider;
	// Slider to rotate the camera around the currentTarget
	private Slider rotateSlider;
	// Slider to adjust camera's distance from currentTarget
	private Slider zoomSlider;
	// Toggle to turn on/off the line renderer for the currentPath
	private Toggle showLineToggle;

	// For use with the rotateSlider, to remember our last-used degree value
	private float currDegree = 0f;

	// Offset where the puzzle array sits relative to the main array - X AXIS
	private Vector3 puzzleArrayOffset = new Vector3(3000f, 0, 0);

	// The default target the camera should sit at if there is no event being viewed
	private Vector3 defaultCameraTarget = new Vector3 (3000, -1000, -2000);

	// The default distance the camera should be placed from the view target
	private Vector3 defaultCameraViewDistance = new Vector3 (0, 0, 1000f);


	// The EventInfo object associated with the current event
	// NOTE: The EventInfo object stores a snapshot list, which is essentially a list of doms (and their states) that are activated by this event
	//       This structure is PASS BY REFERENCE, so if an event is swiped in the middle of playing, this list is also updated
	//       as the event continues playing! So it is necessary to update the puzzle array doms constantly to ensure we view the final
	//       event result, not just the state the array was in when we captured it
	private EventInfo currentEventInfo = null;

	// ----------END VARIABLES----------

	/// <summary>
	/// Start - used to setup references to UI and other constant values
	/// </summary>
	void Start() {
		linRen = this.gameObject.GetComponent<LineRenderer> ();
		facingDir = Quaternion.LookRotation(new Vector3(0,0,1f));

		if (UIObjects == null) {
			Debug.LogError ("No reference to the UI elements");
		} else {
			sizeSlider = UIObjects.transform.Find ("DomSizeSlider").GetComponent<Slider> ();
			rotateSlider = UIObjects.transform.Find ("RotateSlider").GetComponent<Slider> ();
			zoomSlider = UIObjects.transform.Find ("ZoomSlider").GetComponent<Slider> ();
			showLineToggle = UIObjects.transform.Find ("ShowLineToggle").GetComponent<Toggle> ();
		}
	}


	/// <summary>
	/// Update - used to adjust camera when moving, and to update camera with UI settings
	/// </summary>
	void Update () {

		// If the camera is currently moving, check to see if we should keep moving
		if (isMoving) {
			// If we're close enough to the target, stop
			if (Vector3.Distance(this.transform.position, destPos) > 10f) {
				Vector3 translationVector = destPos - this.transform.position;
				this.transform.Translate (translationVector * Time.deltaTime, Space.World);
				this.transform.rotation = Quaternion.Slerp (this.transform.rotation, facingDir, Time.deltaTime);
			} else {
				isMoving = false;
			}
		} 

		// If we are currently viewing an event, update which doms are turned on
		if (currentEventInfo != null) {
			foreach (VisualizeEvent.DomSnapShot curr in currentEventInfo.getSnapshots()) {
				// If there is a dom that is in our snapshots list that is not on, turn it on
				// See the above NOTE for an explaination on why this happens
				if (!curr.Dom.GetComponent<DOMController>().on) {
					curr.Dom.GetComponent<DOMController> ().TurnOn (curr.timeFrac, curr.charge);
				}
			}

			// If we are viewing an event AND the camera isn't moving, then apply the UI adjustments
			// This is to avoid relative movements/rotations from becoming mixed up and chaotic
			if (!isMoving) {
				ApplyUIValues ();
			}

			// If we are viewing an event and the toggle to show the line is on, then draw the line
			if (showLineToggle.isOn) {
				Vector3[] pathPositions = new Vector3[2];
				pathPositions [0] = currentEventInfo.getStart () + puzzleArrayOffset;
				pathPositions [1] = currentEventInfo.getEnd () + puzzleArrayOffset;
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
			foreach (VisualizeEvent.DomSnapShot curr in currentEventInfo.getSnapshots()) {
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
		} else {
			// else calculate our current targetPosition...
			currentTarget = CalculatePathCenterPos ();

			//...and start turing on Doms in the currentEvent's snapshotlist
			foreach (VisualizeEvent.DomSnapShot curr in currentEventInfo.getSnapshots()) {
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

		// Update camera angle
		float rotateValue = rotateSlider.value;
		float rotateDiff = rotateValue - currDegree;
		transform.RotateAround (currentTarget, Vector3.up, rotateDiff);
		transform.LookAt (currentTarget);

		// Update old values for the next iteration
		currDegree = rotateValue;
		currDistance = positionOffset;

		// Lastly, update dom scales
		foreach (VisualizeEvent.DomSnapShot curr in currentEventInfo.getSnapshots()) {
			curr.Dom.transform.localScale = (new Vector3 (1f, 1f, 1f) * sizeSlider.value); 
		}
	}

	/// <summary>
	/// Sets the slider values to default
	/// </summary>
	private void ResetSliders() {
		sizeSlider.value = 1.0f;
		rotateSlider.value = 0f;
		zoomSlider.value = 1000f;
	}

	/// <summary>
	/// Calculates the current event's center position in the puzzle array using the event's start and end points
	/// </summary>
	/// <returns>The event center position.</returns>
	private Vector3 CalculatePathCenterPos() {
		if (currentEventInfo == null) {
			return;
		}

		Vector3 vStart = currentEventInfo.getStart();
		Vector3 vEnd = currentEventInfo.getEnd ();

		Vector3 lookPosition = (vStart + vEnd) / 2f;

		// Be sure to apply the offset!
		return lookPosition + puzzleArrayOffset;

	}
}
