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

	private Transform movingTransform = null;

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

		if (movingTransform != null) {
			if (pressGesture.ActivePointers.Count > 0) {
				Debug.LogError ("pressGestures was nonempty");
				TouchScript.Pointers.Pointer currPointer = pressGesture.ActivePointers [0];
				Vector2 pos = currPointer.Position;
				Vector3 temp = new Vector3 (pos.x, pos.y, puzzleCamera.WorldToScreenPoint (this.transform.position).z);
				Vector3 nextPos = puzzleCamera.ScreenToWorldPoint(temp);
				movingTransform.position = nextPos;
			} else if (releaseGesture.ActivePointers.Count > 0) {
				Vector3 movingTransformCameraPos = puzzleCamera.transform.InverseTransformPoint (movingTransform.transform.position);
				TouchScript.Pointers.Pointer currPointer = releaseGesture.ActivePointers [0];
				Vector2 pos = currPointer.Position;
				Vector3 temp = new Vector3 (pos.x, pos.y, movingTransformCameraPos.z);
				Vector3 nextPos = puzzleCamera.ScreenToWorldPoint(temp);
				movingTransform.position = nextPos;
			}
		} else {
			Debug.Log ("Transform went null");
		}
//		//Debug.Log ("Size of Press pointers: " + pressGesture.ActivePointers.Count + "\nSize of Release pointers: " + releaseGesture.ActivePointers.Count); 
//		foreach (PressTransformPair curr in currentDragGestures) {
//
//			Vector2 screenPos = curr.touchObject.Position;
//			Vector3 nextPos = new Vector3 (screenPos.x, screenPos.y, 0);
//			nextPos = puzzleCamera.ScreenToWorldPoint (nextPos);
//			nextPos.z = Vector3.Distance (puzzleCamera.transform.position, curr.transformMoved.position);
//
//			curr.transformMoved.position = nextPos;
//		}
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

		movingTransform = colliderHit.transform;
//		PressTransformPair newPair = new PressTransformPair ();
//		newPair.touchObject = pressGesture.ActivePointers [0];
//		newPair.transformMoved = nodeTransform;
//
//		currentDragGestures.Add (new PressTransformPair ());



	}

	private void releaseHandler(object sender, System.EventArgs e) {
		
	}
}
