using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Gestures.TransformGestures;
using TouchScript.Gestures;
using TouchScript;

public class DragRecognizer : MonoBehaviour {

	public PressGesture pressGesture;
	public ReleaseGesture releaseGesture;

	public Camera puzzleCamera;

	private List<PressTransformPair> currentDragGestures;

	public struct PressTransformPair
	{
		public Transform transformMoved;
		public TouchScript.Pointers.Pointer touchObject;
	};

	// Use this for initialization
	void Start () {
		currentDragGestures = new List<PressTransformPair> ();
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log ("Size of Press pointers: " + pressGesture.ActivePointers.Count + "\nSize of Release pointers: " + releaseGesture.ActivePointers.Count); 
		foreach (PressTransformPair curr in currentDragGestures) {

			Vector2 screenPos = curr.touchObject.Position;
			Vector3 nextPos = new Vector3 (screenPos.x, screenPos.y, 0);
			nextPos = puzzleCamera.ScreenToWorldPoint (nextPos);
			nextPos.z = Vector3.Distance (puzzleCamera.transform.position, curr.transformMoved.position);

			curr.transformMoved.position = nextPos;
		}
	}

	private void OnEnable() {
		pressGesture.Pressed += pressHandler;
		releaseGesture.Released += releaseHandler;
	}

	private void OnDisable() {
		pressGesture.Pressed -= pressHandler;
		releaseGesture.Released -= releaseHandler;
	}

	private void pressHandler(object sender, System.EventArgs e) {
		Debug.Log ("Press Handler hit");


		// Find the new press
//		TouchScript.Pointers.Pointer newPointer = null;
//		foreach (TouchScript.Pointers.Pointer currPointer in pressGesture.ActivePointers) {
//			bool isFound = false;
//			foreach (PressTransformPair currPair in currentDragGestures) {
//				if (currPair.touchObject.Equals (currPointer)) {
//					isFound = true;
//					break;
//				} 
//			}
//			if (isFound) {
//				continue;
//			} else {
//				newPointer = currPointer;
//				break;
//			}
//		}
//
//		if (newPointer == null) {
//			Debug.LogError ("Pointer Error in DragRecognizer");
//		}

		// Get the hit distance using raycast
		// Get Raycast, check if we clicked a point
		TouchScript.Pointers.Pointer newPointer = pressGesture.ActivePointers[0];

		if (newPointer.Position.x < Screen.width/2f) { 
			Debug.Log ("returned");
			return; 
		}
		Vector3 screenPos = new Vector3 (newPointer.Position.x, newPointer.Position.y, 0);

		Ray InputLocation = puzzleCamera.ScreenPointToRay (screenPos);
		RaycastHit colliderHit;
		if (Physics.Raycast (InputLocation, out colliderHit)) {
			if (colliderHit.transform.gameObject.tag != "LineNode") {
				Debug.Log ("Raycast out, didn't hit a node");
				return;
			}
			Debug.Log ("Raycast hit a node");
		}

		Transform nodeTransform = colliderHit.transform;
		PressTransformPair newPair = new PressTransformPair ();
		newPair.touchObject = releaseGesture.ActivePointers [0];
		newPair.transformMoved = nodeTransform;
		currentDragGestures.Add (new PressTransformPair ());

	}

	private void releaseHandler(object sender, System.EventArgs e) {
		Debug.Log ("Release Handler hit");

		// Find which touch input released, and stop moving that object
		foreach (PressTransformPair curr in currentDragGestures) {
			if (releaseGesture.ActivePointers.Contains(curr.touchObject)) {
				currentDragGestures.Remove (curr);
			}
		}
	}
}
