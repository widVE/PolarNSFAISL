using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTrail : MonoBehaviour {

	private Vector3[] points;
	private LineRenderer ren;
	private Vector3[] rect;
	// Use this for initialization
	void Start () {
		points = new Vector3[2];
		rect = new Vector3[4];
		ren = this.GetComponent<LineRenderer> ();
		ren.material = new Material (Shader.Find ("Particles/Additive"));
	}
	
	public void setStart(Vector3 worldPosition) {		
		points[0] = worldPosition;
		//points[0].z = Camera.main.nearClipPlane + 1;

	}

	public void setEnd(Vector3 worldPosition) {
		points[1] = worldPosition;
		//points[1].z = Camera.main.nearClipPlane + 1;
	}

	public void enterTraceMode() {
			
		ren.numPositions = 2;
		ren.SetPositions (points);
		//Debug.Log ("Start Position: " + points [0]);
		//Debug.Log ("End Position: " + points [1]);

		//calculateRect ();

	}

	// Returns mid of World-space vector
	public Vector3 getTrailMid() {

		Vector3 start = points [0];
		Vector3 end = points [1];

		Vector3 changeVector = (end - start);
		Vector3 mid = (start + (0.5f) * (changeVector));
		Debug.Log ("TRAIL: Start was + " + start + ", and end was " + end + ", making the mid " + mid);
//		GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
//		cube.transform.position = mid;
//		cube.name = "TrailMid";
//		cube.transform.localScale = 0.1f * Vector3.one;
		return mid;
	}

	public float getTrailAngle() {
		/*
		Vector3 changeVector = (0.01f) * (points[1] - points[0]);
		Vector3 tempStart = (0.01f) * points [0];
		Vector3 tempEnd = tempStart + changeVector;
		Vector3 smallVector = tempEnd - tempStart;

		Vector3 screenStart = Camera.main.WorldToScreenPoint (tempStart);
		Vector3 screenEnd = Camera.main.WorldToScreenPoint (tempEnd);
		Vector3 screenVector = screenEnd - screenStart;

		float angle = Mathf.Atan2 (screenVector.y, screenVector.x);
		*/

		Vector3 start = Camera.main.WorldToScreenPoint (((0.1f) * points [0]));
		Vector3 end =  Camera.main.WorldToScreenPoint (((0.1f) * points [1]));

		Vector3 change = end - start;
		float angle = Mathf.Atan2 (change.y, change.x);
		return angle;
	}

	public float getTrailLength() {
		Vector3 changeVector = (points [1] - points [0]);
		return Vector3.Magnitude (changeVector);
	}
}
