using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Gestures.TransformGestures;
using TouchScript.Gestures;
using TouchScript;

/// <summary>
/// The TouchScript event handler used for dragging the adjustable line endpoints
/// It uses both a press and release gesture, however it only uses the press gesture to
/// start the event, and the release gesture ActivePointers list to track the current press
/// NOTE: At the moment only one press can be tracked at a time
/// </summary>
public class DragRecognizer : MonoBehaviour {

	// References to the gesture components to use
	public PressGesture pressGesture;
	public ReleaseGesture releaseGesture;

	// Reference to the puzzle camera
	public Camera puzzleCamera;

	// Reference to the node that is currently being dragged
	private Transform movingTransform = null;
	
	/// <summary>
	/// Update - used for moving the transform based on the gesture list if there is one currently being dragged
	/// </summary>
	void Update () {

		// If there is a transform being moved and there is an ActivePointer, assume the two are associated
		if (movingTransform != null && releaseGesture.ActivePointers.Count > 0) {

			// The the transform position in camera coordinates
			Vector3 movingTransformCameraPos = puzzleCamera.transform.InverseTransformPoint (movingTransform.transform.position);
			// Get the current active pointer
			TouchScript.Pointers.Pointer currPointer = releaseGesture.ActivePointers [0];

			// Find the position in camera coordinates represented by the pointer; we set the z value equal to the movingTransform's z
			Vector2 pos = currPointer.Position;
			Vector3 temp = new Vector3 (pos.x, pos.y, movingTransformCameraPos.z);

			// Convert back to world coordinates 
			Vector3 nextPos = puzzleCamera.ScreenToWorldPoint(temp);
			movingTransform.position = nextPos;
		}
	}

	/// <summary>
	/// OnEnable - when this script is enabled be sure to assign the handler for this event
	/// </summary>
	private void OnEnable() {
		pressGesture.Pressed += pressHandler;
	}

	/// <summary>
	/// OnDisable - when this script is disabled be sure to remove the handler for this event
	/// </summary>
	private void OnDisable() {
		pressGesture.Pressed -= pressHandler;
	}

	/// <summary>
	/// Handler that is called when a press is detected by TouchScript.
	/// It raycasts out from the input position and detects if the press was on an endpoint of the
	/// adustable line. If so, it begins to drag it, otherwise this handler ignores it
	/// Both parameters are unused
	/// </summary>
	/// <param name="sender">Sender of the event</param>
	/// <param name="e">Unity Event argument object</param>
	private void pressHandler(object sender, System.EventArgs e) {

		// Get the pointer that was just detected
		TouchScript.Pointers.Pointer newPointer = pressGesture.ActivePointers[0];

		// If the press was on the left hand side of the screen, ignore it, as that's not the puzzle side
		if (newPointer.Position.x < Screen.width/2f) { 
			return; 
		}

		// Get the screen position of the pointer
		Vector3 screenPos = new Vector3 (newPointer.Position.x, newPointer.Position.y, 0);

		// Raycast out from that screenposition
		Ray InputLocation = puzzleCamera.ScreenPointToRay (screenPos);
		RaycastHit colliderHit;
		if (Physics.Raycast (InputLocation, out colliderHit)) {
			// If the raycast hit the collider of anything not an endnode, return
			if (colliderHit.transform.gameObject.tag != "LineNode") {
				return;
			}
		}

		// We hit an endnode, so set moving transform to begin moving it in update
		movingTransform = colliderHit.transform;
	}

}
