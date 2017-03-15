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
		points [1] = worldPosition;
	}

	public void enterTraceMode() {

		foreach (Vector3 pos in points) {
			Debug.Log ("Point: " + pos);
		}
			
		ren.numPositions = 2;
		ren.SetPositions (points);

		calculateRect ();

	}

	private void calculateRect() {

		rect [0] = Camera.main.WorldToScreenPoint (points [0]) + new Vector3 (0, 1, 0);
		rect [1] = Camera.main.WorldToScreenPoint (points [0]) - new Vector3 (0, 1, 0);

		rect [2] = Camera.main.WorldToScreenPoint (points [1]) + new Vector3 (0, 1, 0);
		rect [3] = Camera.main.WorldToScreenPoint (points [1]) - new Vector3 (0, 1, 0);

	}

	public bool withinRect(Vector3 startPos, Vector3 endPos) {
		if (startPos.x < rect[0].x  || startPos.x > rect[2].x) {
			return false;
		}

		if (startPos.y < rect[1].y  || startPos.x > rect[0].y) {
			return false;
		}

		if (endPos.x < rect[0].x  || endPos.x > rect[2].x) {
			return false;
		}

		if (endPos.y < rect[1].y  || endPos.x > rect[0].y) {
			return false;
		}

		return true;
			
	}
}
