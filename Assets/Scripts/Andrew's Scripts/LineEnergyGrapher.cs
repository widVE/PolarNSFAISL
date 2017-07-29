using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineEnergyGrapher : MonoBehaviour {

	private LineRenderer linRen;
	private float zDistance;
	private Vector3[] points;
	private float xOffset;
	private float yOffset;

	[SerializeField]
	private bool randomizeData = false;

	private EventPlayer visEvent;
	// Use this for initialization
	void Start () {
		xOffset = 4f;
		yOffset = -4.5f;
		zDistance = Camera.main.nearClipPlane + 10;
		InitializePoints ();

		GameObject array = GameObject.Find ("DOMArrayProcedural");
		if (array != null) {
			visEvent = array.GetComponent<EventPlayer>();
		}
			
		linRen = GetComponent<LineRenderer> ();
		linRen.material = new Material (Shader.Find ("Particles/Additive"));
	}
	
	// Update is called once per frame
	void Update () {
		UpdatePoints ();

		linRen.SetPositions (points);
		


	}

	private void InitializePoints() {
		points = new Vector3[100];
		for (int i = 0; i < 100; i++) {
			float x = (i / 100f);
			points [i] = this.transform.TransformPoint (new Vector3 (x + xOffset, yOffset, zDistance));
		}
			
	}

	private void UpdatePoints() {
		//Debug.Log ("Update Points started");
		for (int i = 0; i < points.Length - 1; i++) {
			points [i].y = points [i + 1].y;
		}
		//Debug.Log ("Update loop finished");
		// Either randomize or use VisualizeEvent totalEnergy
		if (randomizeData) {
			points[points.Length - 1].y = Random.value + yOffset;
		} else {

			float newValue = visEvent.totalEnergy*0.001f + yOffset;
			if (newValue > 1) {
				newValue = 1;
			}
			points [points.Length - 1].y = newValue;

		}
		//Debug.Log ("Update Points finished");
	}
}
