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

		// Position Check - done by checking midpoints of the two vectors
		Vector3 trailMid = trail.getTrailMid();
		Vector3 swipeMid = swipePoints[0] + (0.5f) * (swipePoints [1] - swipePoints [0]);
		swipeMid.z = Camera.main.nearClipPlane;

		float positionDiff = Vector3.Distance (trailMid, swipeMid);
		Debug.Log ("Trail mid located at: " + trailMid);
		Debug.Log ("Swipe mid located at: " + swipeMid);
		if (positionDiff <= 10) {
			Debug.Log ("Distance Check Passed, distance was: " + positionDiff);
		} else {
			Debug.Log ("Distance Check failed, distance apart was: " + positionDiff);
			return;
		}

		// Direction Check - check to see if the directions of the two vectors are similar

		// NOTE: Angles are in Degrees
		float swipeAngle = Mathf.Atan2(swipeVector.y, swipeVector.x) * Mathf.Rad2Deg;
		float trailAngle = trail.getTrailAngle() * Mathf.Rad2Deg;

		float angleDiff = Mathf.DeltaAngle (swipeAngle, trailAngle);

		// Give 20-degree lenience
		if (angleDiff < 20) {
			Debug.Log ("Angle Check Passed, angle difference was: " + angleDiff);
		} else {
			Debug.Log ("Angle Checked Failed, angle difference was: " + angleDiff);
			return;
		}

		// Length Check - just see if the swipe is similar (for the most part, as long as the flick vector isn't too short)
		float swipeLength = Vector3.Magnitude(swipeVector);
		float trailLength = trail.getTrailLength ();

		float lengthDiff = Mathf.Abs(trailLength - swipeLength);

		// As long as the swipeVector swipes 1/3 of the trail vector, it passes
		if (lengthDiff < (2f*trailLength/3f)) {
			Debug.Log ("Length Check Passed, length difference was: " + lengthDiff);
		} else {
			Debug.Log ("Length Checked Failed, length difference was: " + lengthDiff);
			return;
		}

	}
}
