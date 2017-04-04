using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Gestures;

public class DetectSwipe : MonoBehaviour {

	public FlickGesture swipeGesture;
	public bool showLine = true;
	public LineRenderer ren;
	private ParticleTrail trail;
	private Vector3[] swipePoints = new Vector3[2];
	private Vector2 swipeVector;

	void Start () {
		if (swipeGesture == null) {
			Debug.LogError ("No Flick Gesture assigned for DetectSwipe component on " + this.gameObject.name);
		}
		ren.material = new Material (Shader.Find ("Particles/Additive"));
		trail = GameObject.Find ("ParticleSpawner 1").GetComponent<ParticleTrail> ();
	}

	private void OnEnable() {
		swipeGesture.Flicked += swipeHandler;
	}

	private void OnDisable() {
		swipeGesture.Flicked -= swipeHandler;
	}

	private void swipeHandler(object sender, System.EventArgs e) {
		Vector2 prev = swipeGesture.PreviousScreenPosition;
		swipeVector = swipeGesture.ScreenFlickVector;
		Vector2 start = prev - swipeVector;
		Debug.Log ("Swipe Detected - Direction: " + swipeVector + " End: " + prev);
		Debug.Log ("Start: " + start);
		if (showLine) {
			Vector3[] array = new Vector3[2];
			swipePoints [0] = Camera.main.ScreenToWorldPoint(new Vector3(start.x, start.y, Camera.main.nearClipPlane + 1));
			swipePoints [1] = Camera.main.ScreenToWorldPoint(new Vector3(prev.x, prev.y, Camera.main.nearClipPlane + 1));
			ren.SetPositions (swipePoints);
		}


		// Now we can try to detect a swipe!
		if (swipedEvent()) {
			Debug.Log ("Event captured!");
			GameObject.Find ("EventPanel").GetComponent<EventCanvasManager> ().addEvent ("Event");
		} else {
			Debug.Log ("Event Missed, try again!");
		}


	}

	private bool swipedEvent() {


		//-----Position Check - done by checking midpoints of the two vectors-----
		Vector3 trailMid = trail.getTrailMid();
		Vector3 swipeMid = swipePoints[0] + (0.5f) * (swipePoints [1] - swipePoints [0]);


//		GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
//		cube.transform.position = swipeMid;
//		cube.name = "SwipeMid";
//		cube.transform.localScale = 0.1f * Vector3.one;


		// Both midpoints are in world coordinates, convert them to screen coordinates to compare them
		Vector3 swipeMidScreen = Camera.main.WorldToScreenPoint(swipeMid);
		Vector3 trailMidScreen = Camera.main.WorldToScreenPoint (trailMid);
		swipeMidScreen.z = 0;
		trailMidScreen.z = 0;

		// Calculate the difference between the mid points
		float positionDiff = Vector3.Distance(swipeMidScreen, trailMidScreen);

		// TODO: If they are within a certain distance, it passes (this may change based on the display, may need adjusting for tabletop)
		if (positionDiff <= Mathf.Min((Screen.height / 2f), (Screen.width) / 2f)) {
			Debug.Log ("Distance Check Passed, distance was: " + positionDiff);
		} else {
			Debug.Log ("Distance Check failed, distance apart was: " + positionDiff);
			return false;
		}

		//-----Direction Check - check to see if the directions of the two vectors are similar-----

		// NOTE: Angles are in Degrees
		float swipeAngle = Mathf.Atan2(swipeVector.y, swipeVector.x) * Mathf.Rad2Deg;
		Debug.Log ("Swipe angle: " + swipeAngle);
		float trailAngle = trail.getTrailAngle() * Mathf.Rad2Deg;
		Debug.Log ("Trail Angle: " + trailAngle);

		float angleDiff = Mathf.Abs(Mathf.DeltaAngle (swipeAngle, trailAngle));

		// Give 20-degree lenience
		if (angleDiff < 45) {
			Debug.Log ("Angle Check Passed, angle difference was: " + angleDiff);
		} else {
			Debug.Log ("Angle Checked Failed, angle difference was: " + angleDiff);
			return false;
		}

		// Length Check - just see if the swipe is long enough
		float swipeLength = Vector3.Magnitude(swipeVector);

		// As long as the swipeVector swipes 1/2 the screen, it passes
		// TODO: This may need to be adjusted based on the display used
		if (swipeLength > Screen.width / 2f) {
			Debug.Log ("Length Check Passed, length was: " + swipeLength);
		} else {
			Debug.Log ("Length Checked Failed, length was: " + swipeLength);
			return false;
		}

		return true;
	}
}
