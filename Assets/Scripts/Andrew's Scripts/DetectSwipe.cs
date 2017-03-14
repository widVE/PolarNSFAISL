using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Gestures;

public class DetectSwipe : MonoBehaviour {

	public FlickGesture swipeGesture;
	public bool showLine = true;
	public LineRenderer ren;

	void Start () {
		if (swipeGesture == null) {
			Debug.LogError ("No Flick Gesture assigned for DetectSwipe component on " + this.gameObject.name);
		}
		ren.material = new Material (Shader.Find ("Particles/Additive"));
	}

	private void OnEnable() {
		swipeGesture.Flicked += swipeHandler;
	}

	private void OnDisable() {
		swipeGesture.Flicked -= swipeHandler;
	}

	private void swipeHandler(object sender, System.EventArgs e) {
		Vector2 prev = swipeGesture.PreviousScreenPosition;
		Vector2 vector = swipeGesture.ScreenFlickVector;
		Vector2 start = prev - vector;
		Debug.Log ("Swipe Detected - Direction: " + vector + " End: " + prev);
		Debug.Log ("Start: " + start);
		if (showLine) {
			Vector3[] array = new Vector3[2];
			array [0] = Camera.main.ScreenToWorldPoint(new Vector3(start.x, start.y, 10));
			array [1] = Camera.main.ScreenToWorldPoint(new Vector3(prev.x, prev.y, 10));
			Debug.Log ("Line Drawn: " + array [0] + " to " + array [1]);

			ren.SetPositions (array);
		}


	}
}
