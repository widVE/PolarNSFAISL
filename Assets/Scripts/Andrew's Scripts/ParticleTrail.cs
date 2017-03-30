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
		
		Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
		Vector3 endPos = Camera.main.ScreenToWorldPoint (new Vector3 (screenPos.x, screenPos.y, Camera.main.nearClipPlane + 1));
		Debug.Log ("Trail End Position: " + endPos);
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
		
		Vector3 changeVector = (points [1] - points [0]);
		Vector3 mid = (points [0] + (0.5f) * (changeVector));
		return mid;
	}

	public float getTrailAngle() {
		Vector3 changeVector = (points [1] - points [0]);

		float angle = Mathf.Atan2 (changeVector.y, changeVector.x);

		return angle;
	}

	public float getTrailLength() {
		Vector3 changeVector = (points [1] - points [0]);
		return Vector3.Magnitude (changeVector);
	}
}
