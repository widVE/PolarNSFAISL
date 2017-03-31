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
	}

	public void setEnd(Vector3 worldPosition) {
		points[1] = worldPosition;
	}

	public void enterTraceMode() {
			
		ren.numPositions = 2;
		ren.SetPositions (points);
		//Debug.Log ("Start Position: " + points [0]);
		//Debug.Log ("End Position: " + points [1]);

		//calculateRect ();

	}

	public Vector3 getTrailMid() {

		Vector3 start = Camera.main.WorldToScreenPoint (points [0]);
		Vector3 end = Camera.main.WorldToScreenPoint (points [1]);
		start.z = 0;
		end.z = 0;
		Vector3 changeVector = (end - start);
		Vector3 mid = (start + (0.5f) * (changeVector));
		Debug.Log ("Start was + " + start + ", and end was " + end + ", making the mid " + mid);
		return mid;
	}

	public float getTrailAngle() {

		Vector3 start = Camera.main.WorldToScreenPoint (points [0]);
		Vector3 end = Camera.main.WorldToScreenPoint (points [1]);
	
		Vector3 changeVector = (end - start);
		float angle = Mathf.Atan2 (changeVector.y, changeVector.x);

		return angle;
	}

	public float getTrailLength() {
		Vector3 changeVector = (points [1] - points [0]);
		return Vector3.Magnitude (changeVector);
	}
}
